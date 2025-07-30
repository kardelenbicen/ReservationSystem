using System;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models;

public class CartItem
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int MeetingRoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    [MaxLength(30, ErrorMessage = "Etkinlik adı en fazla 30 karakter olabilir.")]
    public string? EventName { get; set; }
    
    [MaxLength(30, ErrorMessage = "Açıklama en fazla 30 karakter olabilir.")]
    public string? Description { get; set; }
    
    public decimal TotalAmount { get; set; }
    public double DurationHours { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public MeetingRoom? MeetingRoom { get; set; }
    public ApplicationUser? User { get; set; }
}