using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models;

public class MeetingRoom
{
    public int Id { get; set; }

    [Display(Name = "�sim")]
    public string? Name { get; set; }

    [Display(Name = "Kapasite")]
    public int Capacity { get; set; }

    [Display(Name = "Konum")]
    public string? Location { get; set; }

    [Display(Name = "Cihazlar")]
    public string? Devices { get; set; }

    [Display(Name = "A��klama")]
    public string? Description { get; set; }
    public ICollection<MeetingRoomImage> Images { get; set; }
}