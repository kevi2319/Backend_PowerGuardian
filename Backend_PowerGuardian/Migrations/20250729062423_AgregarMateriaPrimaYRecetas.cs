using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMateriaPrimaYRecetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateriasPrimas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriasPrimas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecetasProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetasProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecetasProducto_MateriasPrimas_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "MateriasPrimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetasProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecetasProducto_MateriaPrimaId",
                table: "RecetasProducto",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetasProducto_ProductoId",
                table: "RecetasProducto",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecetasProducto");

            migrationBuilder.DropTable(
                name: "MateriasPrimas");
        }
    }
}
