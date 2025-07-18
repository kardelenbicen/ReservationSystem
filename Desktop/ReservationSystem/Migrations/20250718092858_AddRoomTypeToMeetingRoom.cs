using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypeToMeetingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "MeetingRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "MeetingRooms");
        }
    }
}
