using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<MeetingRoom> MeetingRooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<MeetingRoomImage> MeetingRoomImages { get; set; }
} 