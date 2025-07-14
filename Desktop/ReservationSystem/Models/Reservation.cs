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
    public MeetingRoom? MeetingRoom { get; set; }
    public Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Approved,
    Rejected
} 