using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerwisPogodowy.Migrations
{
    /// <inheritdoc />
    public partial class Dodanie_wojewodztwa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "Cities",
                newName: "Voivodeship");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "Voivodeship",
                table: "Cities",
                newName: "CityName");
        }
    }
}
