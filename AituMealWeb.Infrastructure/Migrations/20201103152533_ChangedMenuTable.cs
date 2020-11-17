using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AituMealWeb.Infrastructure.Migrations
{
    public partial class ChangedMenuTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weekday",
                table: "Menu");

            migrationBuilder.AddColumn<DateTime>(
                name: "MenuForDay",
                table: "Menu",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuForDay",
                table: "Menu");

            migrationBuilder.AddColumn<int>(
                name: "Weekday",
                table: "Menu",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
