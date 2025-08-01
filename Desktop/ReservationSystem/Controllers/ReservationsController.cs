using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGeneratedDocument;
using System.Collections.Generic;
using System;

namespace ReservationSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ReservationSystem.Models.ApplicationUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<ReservationSystem.Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int? meetingRoomId, int? clearFilter)
        {
            if (clearFilter.HasValue && clearFilter.Value == 1)
            {
                meetingRoomId = null;
            }

            var reservations = _context.Reservations.Include(r => r.MeetingRoom).AsQueryable();

            if (meetingRoomId.HasValue)
            {
                reservations = reservations.Where(r => r.MeetingRoomId == meetingRoomId.Value);
            }

            var list = reservations.ToList();
            ViewBag.MeetingRooms = _context.MeetingRooms.ToList();
            ViewBag.SelectedMeetingRoomId = meetingRoomId;
            return View(list);
        }

        [Authorize]
        public IActionResult Create(int? roomId)
        {
            ViewBag.RoomId = roomId;
            ViewBag.Rooms = _context.MeetingRooms.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var userId = _userManager.GetUserId(User);

            ModelState.Remove("User");
            ModelState.Remove("MeetingRoom");
            ModelState.Remove("Status");
            ModelState.Remove("UserId");
            ModelState.Remove("RejectMessage");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("DurationHours");
            ModelState.Remove("IsPaid");
            ModelState.Remove("PaymentId");
            ModelState.Remove("Payment");

            if (reservation.EndTime <= reservation.StartTime)
            {
                return Json(new { success = false, message = "Bitiş zamanı başlangıçtan önce olamaz" });
            }

            if (reservation.StartTime < DateTime.Now || reservation.EndTime < DateTime.Now)
            {
                return Json(new { success = false, message = "Geçmiş bir tarihe rezervasyon yapılamaz." });
            }

            if (ModelState.IsValid)
            {
                var conflict = _context.Reservations.Any(r => r.MeetingRoomId == reservation.MeetingRoomId &&
                    ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                    (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime)));

                if (conflict)
                {
                    return Json(new { success = false, message = " Seçilen saat aralığında başka bir rezervasyon var" });
                }

                var room = await _context.MeetingRooms.FindAsync(reservation.MeetingRoomId);
                if (room == null)
                {
                    return Json(new { success = false, message = "Oda bulunamadı." });
                }

                var duration = (reservation.EndTime - reservation.StartTime).TotalHours;
                var totalAmount = (decimal)duration * room.HourlyRate;

                var cartItem = new CartItem
                {
                    UserId = userId,
                    MeetingRoomId = reservation.MeetingRoomId,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    EventName = reservation.EventName,
                    Description = reservation.Description,
                    TotalAmount = totalAmount,
                    DurationHours = duration,
                    CreatedAt = DateTime.Now
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Rezervasyon sepete eklendi!", redirectUrl = "/Cart" });
            }

            return Json(new { success = false, message = "Lütfen tüm alanları doldurun." });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Pending()
        {
            var pending = _context.Reservations
                .Include(r => r.MeetingRoom)
                .ThenInclude(m => m.Images)
                .Include(r => r.User)
                .Where(r => r.Status == "Pending" && r.IsPaid)
                .OrderBy(r => r.StartTime)
                .ToList();

            return View(pending);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int? id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound();
            return View(reservation);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveConfirmed(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound();

            reservation.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("Pending");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int? id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound();
            return View(reservation);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id, string rejectMessage)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound();

            reservation.Status = "Rejected";
            reservation.RejectMessage = rejectMessage;
            _context.SaveChanges();
            return RedirectToAction("Pending");
        }

        [Authorize]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            var reservations = await _context.Reservations
                .Include(r => r.MeetingRoom)
                .ThenInclude(m => m.Images)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return View(reservations);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CleanNullUserId()
        {
            var nullReservations = _context.Reservations.Where(r => r.UserId == null).ToList();
            if (nullReservations.Any())
            {
                _context.Reservations.RemoveRange(nullReservations);
                _context.SaveChanges();
            }
            return Content($"Silinen kayıt sayısı: {nullReservations.Count}");
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public JsonResult CalendarData()
        {
            var reservations = _context.Reservations
                .Include(r => r.MeetingRoom)
                .Include(r => r.User)
                .Where(r => r.EndTime > r.StartTime && r.Status != "İptal Edildi")
                .ToList();

            var intervals = reservations
                .Select(r => new
                {
                    Reservation = r,
                    Start = r.StartTime,
                    End = r.EndTime
                })
                .OrderBy(i => i.Start)
                .ToList();

            var merged = new List<List<Reservation>>();

            foreach (var interval in intervals)
            {
                bool found = false;
                foreach (var group in merged)
                {
                    if (group.Any(r => r.EndTime > interval.Start && r.StartTime < interval.End))
                    {
                        group.Add(interval.Reservation);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    merged.Add(new List<Reservation> { interval.Reservation });
                }
            }

            bool mergedAny;
            do
            {
                mergedAny = false;
                for (int i = 0; i < merged.Count; i++)
                {
                    for (int j = i + 1; j < merged.Count; j++)
                    {
                        if (merged[i].Any(r1 => merged[j].Any(r2 => r1.EndTime > r2.StartTime && r1.StartTime < r2.EndTime)))
                        {
                            merged[i].AddRange(merged[j]);
                            merged.RemoveAt(j);
                            mergedAny = true;
                            break;
                        }
                    }
                    if (mergedAny) break;
                }
            } while (mergedAny);

            var events = new List<object>();
            foreach (var group in merged)
            {
                var firstReservation = group.OrderBy(r => r.StartTime).First();
                var lastReservation = group.OrderBy(r => r.EndTime).Last();

                var meetings = group.Select(r => new
                {
                    title = r.EventName,
                    start = r.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = r.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    room = r.MeetingRoom?.Name,
                    description = r.Description,
                    userName = r.User?.Email,
                    status = r.Status
                }).ToList();

                events.Add(new
                {
                    id = firstReservation.Id,
                    title = $"{firstReservation.MeetingRoom?.Name} - Detay Göster",
                    start = firstReservation.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = lastReservation.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    meetings = meetings
                });
            }

            return Json(events);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id && r.UserId == userId);

            if (reservation == null)
                return NotFound();

            reservation.Status = "İptal Edildi";
            _context.SaveChanges();

            return RedirectToAction("My");
        }
    }
}
