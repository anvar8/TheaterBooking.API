using Microsoft.EntityFrameworkCore.Migrations;

namespace TheaterBooking.Migrations
{
    public partial class modifiedClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PricePerTicket",
                table: "ShowTimes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketsAvailable",
                table: "ShowTimes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTickets",
                table: "Bookings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerTicket",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "TicketsAvailable",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "NumberOfTickets",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShowTimes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bookings",
                nullable: false,
                defaultValue: false);
        }
    }
}
