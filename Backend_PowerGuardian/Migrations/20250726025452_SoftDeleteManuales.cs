using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteManuales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Manuales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Manuales");
        }
    }
}
