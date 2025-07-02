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
        [Authorize]
        public IActionResult Create(Reservation r)
        {
            bool conflict = _context.Reservations.Any(x => x.MeetingRoomId == r.MeetingRoomId && x.Status != "Rejected" && ((r.StartTime >= x.StartTime && r.StartTime < x.EndTime) || (r.EndTime > x.StartTime && r.EndTime <= x.EndTime) || (r.StartTime <= x.StartTime && r.EndTime >= x.EndTime)));
            if (conflict)
            {
                ModelState.AddModelError("StartTime", "Seçilen saat aralığında başka bir rezervasyon var.");
                ViewBag.RoomId = r.MeetingRoomId;
                return View(r);
            }
            r.UserId = _userManager.GetUserId(User);
            r.Status = "Pending";
            _context.Reservations.Add(r);
            _context.SaveChanges();
            return RedirectToAction("Index", "MeetingRooms");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Pending()
        {
            var pending = _context.Reservations.Where(r => r.Status == "Pending").OrderBy(r => r.StartTime).ToList();
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
            var myReservations = _context.Reservations.Where(r => r.UserId == userId).ToList();
            return View(myReservations);
        }
    }
}
