using System;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models;

public class Reservation
{
    public int Id { get; set; }
    public int MeetingRoomId { get; set; }
    public string? UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    [MaxLength(30, ErrorMessage = "Etkinlik adı en fazla 30 karakter olabilir.")]
    public string? EventName { get; set; }
    
    [MaxLength(30, ErrorMessage = "Açıklama en fazla 30 karakter olabilir.")]
    public string? Description { get; set; }
    
    public string? Status { get; set; }
    public string? RejectMessage { get; set; }
    
    public decimal TotalAmount { get; set; }
    public double DurationHours { get; set; }
    public bool IsPaid { get; set; }
    public int? PaymentId { get; set; }
    
    public MeetingRoom? MeetingRoom { get; set; }
    public ReservationSystem.Models.ApplicationUser? User { get; set; }
    public Payment? Payment { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Approved,
    Rejected
}