using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MehmetHairDesigner.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "WorkingHours",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "WorkingHours",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WorkingHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkingHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "WorkingHours",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "NotificationRequests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "NotificationRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NotificationRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "NotificationRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "NotificationRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "Holidays",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "Holidays",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Holidays",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Holidays",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "Holidays",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "BusySlots",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "BusySlots",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BusySlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BusySlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "BusySlots",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "NotificationRequests");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "BusySlots");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "BusySlots");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BusySlots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BusySlots");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "BusySlots");
        }
    }
}
