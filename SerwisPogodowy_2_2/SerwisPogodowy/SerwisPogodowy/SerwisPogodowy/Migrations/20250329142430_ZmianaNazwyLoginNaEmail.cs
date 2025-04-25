using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerwisPogodowy.Migrations
{
    /// <inheritdoc />
    public partial class ZmianaNazwyLoginNaEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Users",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Login");
        }
    }
}
