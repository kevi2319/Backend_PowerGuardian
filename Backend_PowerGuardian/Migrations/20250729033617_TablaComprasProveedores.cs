using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class TablaComprasProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComprasProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasProveedor_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCompraProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraProveedorId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCompraProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesCompraProveedor_ComprasProveedor_CompraProveedorId",
                        column: x => x.CompraProveedorId,
                        principalTable: "ComprasProveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCompraProveedor_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProveedor_ProveedorId",
                table: "ComprasProveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompraProveedor_CompraProveedorId",
                table: "DetallesCompraProveedor",
                column: "CompraProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompraProveedor_ProductoId",
                table: "DetallesCompraProveedor",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesCompraProveedor");

            migrationBuilder.DropTable(
                name: "ComprasProveedor");
        }
    }
}
