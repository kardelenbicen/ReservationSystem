using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ReservationSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(c => c.MeetingRoom)
                .ThenInclude(m => m.Images)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            ViewBag.TotalAmount = cartItems.Sum(c => c.TotalAmount);
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Ürün sepetten kaldırıldı." });
            }

            return Json(new { success = false, message = "Ürün bulunamadı." });
        }

        public IActionResult Payment()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = _context.CartItems
                .Include(c => c.MeetingRoom)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            ViewBag.TotalAmount = cartItems.Sum(c => c.TotalAmount);
            ViewBag.CartItems = cartItems;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentMethod, string cardNumber, string expiryDate, string cvv)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(c => c.MeetingRoom)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Sepetiniz boş." });
            }

            var totalAmount = cartItems.Sum(c => c.TotalAmount);

            var payment = new Payment
            {
                UserId = userId,
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.Now,
                Status = "Completed"
            };

            if (paymentMethod == "CreditCard")
            {
                payment.CardNumber = cardNumber?.Substring(cardNumber.Length - 4);
                payment.ExpiryDate = expiryDate;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var reservation = new Reservation
                {
                    MeetingRoomId = cartItem.MeetingRoomId,
                    UserId = cartItem.UserId,
                    StartTime = cartItem.StartTime,
                    EndTime = cartItem.EndTime,
                    EventName = cartItem.EventName,
                    Description = cartItem.Description,
                    Status = "Pending",
                    TotalAmount = cartItem.TotalAmount,
                    DurationHours = cartItem.DurationHours,
                    IsPaid = true,
                    PaymentId = payment.Id
                };

                _context.Reservations.Add(reservation);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            string message = paymentMethod == "CreditCard" ? "Ödeme başarılı!" : "Nakit ödeme seçildi. Rezervasyonlarınız admin onayına gönderildi.";
            return Json(new { success = true, message = message });
        }
    }
}