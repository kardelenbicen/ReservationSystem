using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ReservationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
            return View(reservation);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveConfirmed(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            reservation.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("Pending");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int? id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            return View(reservation);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id, string rejectMessage)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
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
        public IActionResult CalendarData()
        {
            var events = _context.Reservations
                .Where(r => r.Status != "İptal Edildi")               
                .Select(r => new {
                title = r.EventName,
                start = r.StartTime,
                end = r.EndTime,
                description = r.Description,
                isfull = true
            }).ToList();
            return Json(events);
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
