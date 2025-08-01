using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReservationSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Now;
            var tomorrow = today.AddHours(24); 
            
            var upcomingReservations = await _context.Reservations
                .Include(r => r.MeetingRoom)
                .ThenInclude(m => m.Images)
                .Where(r => r.UserId == userId && 
                       r.Status == "Approved" && 
                       r.StartTime >= today && 
                       r.StartTime < tomorrow)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
                
            ViewBag.UpcomingReservations = upcomingReservations;
            ViewBag.HasNotifications = upcomingReservations.Count > 0;
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
