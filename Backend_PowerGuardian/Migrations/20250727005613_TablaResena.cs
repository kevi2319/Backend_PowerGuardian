using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class TablaResena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductoUnidadId1",
                table: "Dispositivos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Resenas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoUnidadId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resenas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resenas_ProductoUnidades_ProductoUnidadId",
                        column: x => x.ProductoUnidadId,
                        principalTable: "ProductoUnidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_ProductoUnidadId1",
                table: "Dispositivos",
                column: "ProductoUnidadId1",
                unique: true,
                filter: "[ProductoUnidadId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_ProductoUnidadId",
                table: "Resenas",
                column: "ProductoUnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_UsuarioId",
                table: "Resenas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositivos_ProductoUnidades_ProductoUnidadId1",
                table: "Dispositivos",
                column: "ProductoUnidadId1",
                principalTable: "ProductoUnidades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositivos_ProductoUnidades_ProductoUnidadId1",
                table: "Dispositivos");

            migrationBuilder.DropTable(
                name: "Resenas");

            migrationBuilder.DropIndex(
                name: "IX_Dispositivos_ProductoUnidadId1",
                table: "Dispositivos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ProductoUnidadId1",
                table: "Dispositivos");
        }
    }
}
