using Microsoft.AspNetCore.Identity;
using System;

namespace ReservationSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string ProfileImagePath { get; set; }
    }
} 