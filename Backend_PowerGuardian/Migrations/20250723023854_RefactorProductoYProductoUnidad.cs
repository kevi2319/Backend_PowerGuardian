using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductoYProductoUnidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Usado",
                table: "Productos");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Productos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProductoUnidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Usado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoUnidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoUnidades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoUnidades_ProductoId",
                table: "ProductoUnidades",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoUnidades");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Productos");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Usado",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
