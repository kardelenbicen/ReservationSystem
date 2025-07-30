using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models;

public class MeetingRoom
{
    public int Id { get; set; }

    [Display(Name = "isim")]
    public string? Name { get; set; }

    [Display(Name = "Kapasite")]
    public int Capacity { get; set; }

    [Display(Name = "Konum")]
    public string? Location { get; set; }

    [Display(Name = "Cihazlar")]
    public string? Devices { get; set; }

    [Display(Name = "Açıklama")]
    public string? Description { get; set; }
    
    [Display(Name = "Saatlik Ücret (₺)")]
    [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0'dan büyük olmalıdır.")]
    public decimal HourlyRate { get; set; }
    
    public string RoomType { get; set; }
    public ICollection<MeetingRoomImage> Images { get; set; }
}