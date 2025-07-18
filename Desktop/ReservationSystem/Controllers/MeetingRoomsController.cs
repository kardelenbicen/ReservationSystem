using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var meetingRoom = await _context.MeetingRooms
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

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
        public IActionResult Create(MeetingRoom meetingRoom, string[] RoomTypes, List<IFormFile> images)
        {
            meetingRoom.RoomType = string.Join(",", RoomTypes);
            _context.MeetingRooms.Add(meetingRoom);
            _context.SaveChanges();

            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine("wwwroot/images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        var roomImage = new MeetingRoomImage
                        {
                            MeetingRoomId = meetingRoom.Id,
                            ImagePath = "/images/" + fileName
                        };
                        _context.MeetingRoomImages.Add(roomImage);
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var meetingRoom = await _context.MeetingRooms
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meetingRoom == null)
                return NotFound();

            return View(meetingRoom);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(MeetingRoom meetingRoom, string[] RoomTypes)
        {
            meetingRoom.RoomType = string.Join(",", RoomTypes);
            _context.Update(meetingRoom);
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var roomImage = new MeetingRoomImage
                {
                    MeetingRoomId = id,
                    ImagePath = "/images/" + fileName
                };
                _context.MeetingRoomImages.Add(roomImage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int roomId)
        {
            var image = await _context.MeetingRoomImages.FindAsync(imageId);
            if (image != null)
            {
                var filePath = Path.Combine("wwwroot", image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.MeetingRoomImages.Remove(image);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = roomId });
        }
    }
}