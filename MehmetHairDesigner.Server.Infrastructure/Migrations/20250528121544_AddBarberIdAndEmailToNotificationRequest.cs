using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MehmetHairDesigner.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberIdAndEmailToNotificationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "NotificationRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "BarberId",
                table: "NotificationRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NotificationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberId",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NotificationRequests");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "NotificationRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
