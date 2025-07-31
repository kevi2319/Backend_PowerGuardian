using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class AddPzemReadingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PzemReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Voltaje = table.Column<float>(type: "real", nullable: false),
                    Corriente = table.Column<float>(type: "real", nullable: false),
                    Potencia = table.Column<float>(type: "real", nullable: false),
                    FactorPotencia = table.Column<float>(type: "real", nullable: false),
                    Frecuencia = table.Column<float>(type: "real", nullable: false),
                    Energia = table.Column<float>(type: "real", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PzemReadings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PzemReadings");
        }
    }
}
