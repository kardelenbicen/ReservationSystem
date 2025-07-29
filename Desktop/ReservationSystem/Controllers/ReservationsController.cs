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
            reservation.UserId = _userManager.GetUserId(User);
            reservation.Status = "Pending";
            reservation.RejectMessage = "";
            ModelState.Remove("User");
            ModelState.Remove("MeetingRoom");
            ModelState.Remove("Status");
            ModelState.Remove("UserId");
            ModelState.Remove("RejectMessage");
            if (reservation.EndTime <= reservation.StartTime)
            {
                return Json(new { sucess = false, message = "Bitiş zamanı başlangıçtan önce olamaz" });
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
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = " Rezervasyon başarıyla oluşturuldu" });
            }
            return Json(new { success = false, message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Pending()
        {
            var pending = _context.Reservations.Include(r => r.MeetingRoom).Include(r => r.User).Where(r => r.Status == "Pending").OrderBy(r => r.StartTime).ToList();
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
        public IActionResult My()
        {
            var userId = _userManager.GetUserId(User);
            var myReservations = _context.Reservations.Include(r => r.MeetingRoom).Where(r => r.UserId != null && r.UserId == userId).ToList();
            return View(myReservations);
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
                .Where(r => r.EndTime > r.StartTime)
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

            var result = merged.Select(g => new
            {
                start = g.Min(x => x.StartTime),
                end = g.Max(x => x.EndTime),
                title = g.FirstOrDefault()?.MeetingRoom?.Name ?? "Salon",
                backgroundColor = "#e53935",
                borderColor = "#e53935",
                textColor = "#fff",
                extendedProps = new
                {
                    meetings = g.Select(x => new
                    {
                        room = x.MeetingRoom?.Name ?? "Salon",
                        title = x.EventName,
                        start = x.StartTime,
                        end = x.EndTime,
                        user = x.User != null ? x.User.UserName : null
                    }).ToList()
                }
            }).ToList();

            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound();
            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
            reservation.Status = "İptal Edildi";
            _context.SaveChanges();
            return RedirectToAction("My");
        }
    }
}
