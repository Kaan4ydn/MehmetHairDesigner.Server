using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MehmetHairDesigner.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "Barbers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "Barbers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Barbers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Barbers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "Barbers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateAt",
                table: "Appointments",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteAt",
                table: "Appointments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateAt",
                table: "Appointments",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Appointments");
        }
    }
}
