using System;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models;

public class Payment
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; }
    
    public string? CardNumber { get; set; }
    public string? ExpiryDate { get; set; }
    public string? CVV { get; set; }
    
    public ApplicationUser? User { get; set; }
}