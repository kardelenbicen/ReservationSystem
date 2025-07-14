<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using System.Linq;

namespace ReservationSystem.Controllers
{
    public class MeetingRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MeetingRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search, int? capacity, string location, string devices)
        {
            var rooms = _context.MeetingRooms.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                rooms = rooms.Where(r => r.Name != null && r.Name.Contains(search));
            if (capacity.HasValue)
                rooms = rooms.Where(r => r.Capacity >= capacity.Value);
            if (!string.IsNullOrEmpty(location))
                rooms = rooms.Where(r => r.Location != null && r.Location.Contains(location));
            if (!string.IsNullOrEmpty(devices))
                rooms = rooms.Where(r => r.Devices != null && r.Devices.Contains(devices));
            return View(rooms.ToList());
        }
        public IActionResult Details(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Add(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Update(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            _context.MeetingRooms.Remove(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using System.Linq;

namespace ReservationSystem.Controllers
{
    public class MeetingRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MeetingRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search, int? capacity, string location, string devices)
        {
            var rooms = _context.MeetingRooms.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                rooms = rooms.Where(r => r.Name != null && r.Name.Contains(search));
            if (capacity.HasValue)
                rooms = rooms.Where(r => r.Capacity >= capacity.Value);
            if (!string.IsNullOrEmpty(location))
                rooms = rooms.Where(r => r.Location != null && r.Location.Contains(location));
            if (!string.IsNullOrEmpty(devices))
                rooms = rooms.Where(r => r.Devices != null && r.Devices.Contains(devices));
            return View(rooms.ToList());
        }
        public IActionResult Details(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Add(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Update(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            return View(meetingRoom);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var meetingRoom = _context.MeetingRooms.FirstOrDefault(m => m.Id == id);
            if (meetingRoom == null)
                return NotFound();
            _context.MeetingRooms.Remove(meetingRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
>>>>>>> 190b34c23ca54e16e21e59afed68a08214d58036
