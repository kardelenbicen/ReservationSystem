namespace ReservationSystem.Models
{
    public class MeetingRoomImage
    {
        public int Id { get; set; }
        public int MeetingRoomId { get; set; }
        public string ImagePath { get; set; }
        public MeetingRoom MeetingRoom { get; set; }
    }
}
