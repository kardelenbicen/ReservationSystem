using System.Collections.Generic;

namespace ReservationSystem.Models;

public class MeetingRoom
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public string? Devices { get; set; }
    public string? Description { get; set; }
} 