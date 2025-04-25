using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerwisPogodowy.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatesInWicty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedCurrent",
                table: "Cities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedWeekly",
                table: "Cities",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedCurrent",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "LastUpdatedWeekly",
                table: "Cities");
        }
    }
}
