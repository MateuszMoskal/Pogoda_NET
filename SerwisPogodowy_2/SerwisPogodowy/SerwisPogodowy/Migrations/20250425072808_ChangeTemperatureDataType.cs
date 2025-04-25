using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerwisPogodowy.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTemperatureDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Temperature",
                table: "WeatherData",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "WeatherData",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
