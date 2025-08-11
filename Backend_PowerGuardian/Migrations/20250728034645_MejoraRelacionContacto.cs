using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_PowerGuardian.Migrations
{
    /// <inheritdoc />
    public partial class MejoraRelacionContacto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PASO 1: Desconectar la clave foránea y la clave primaria existentes
            // Esto es crucial para poder manipular las columnas afectadas.
            migrationBuilder.DropForeignKey(
                name: "FK_Contactos_Productos_ProductoId",
                table: "Contactos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contactos",
                table: "Contactos");

            // PASO 2: Eliminar la columna 'Id' antigua (que era la clave primaria previa)
            // Dado que la tabla ha sido vaciada, esta operación debería ser segura.
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Contactos");

            // PASO 3: Modificar la columna 'ProductoId'
            // Asegura que 'ProductoId' sea NOT NULL, ya que el modelo ahora lo requiere.
            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Contactos",
                type: "int",
                nullable: false, // Cambiado de true a false
                defaultValue: 0, // Establece un valor por defecto para nuevos registros
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // PASO 4: Recrear 'ContactoId' para asegurar la propiedad IDENTITY
            // El problema previo era que 'AlterColumn' no podía cambiar 'IDENTITY'.
            // Forzamos un Drop y Add para una recreación explícita con IDENTITY.
            // Esto asume que no hay datos en ContactoId que deban ser preservados de una forma no-IDENTITY.
            // Dado que la tabla está vacía, es seguro.
            migrationBuilder.DropColumn( // Asegúrate de que no haya una ContactoId vieja sin IDENTITY
                name: "ContactoId",
                table: "Contactos");

            migrationBuilder.AddColumn<int>(
                name: "ContactoId",
                table: "Contactos",
                type: "int",
                nullable: false,
                defaultValue: 0) // Define un valor por defecto
                .Annotation("SqlServer:Identity", "1, 1"); // Aplica la propiedad IDENTITY

            // PASO 5: Establecer 'ContactoId' como la nueva clave primaria
            migrationBuilder.AddPrimaryKey(
                name: "PK_Contactos",
                table: "Contactos",
                column: "ContactoId");

            // PASO 6: Reestablecer la clave foránea
            // Conecta 'ProductoId' con la tabla 'Productos'.
            migrationBuilder.AddForeignKey(
                name: "FK_Contactos_Productos_ProductoId",
                table: "Contactos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // Considera si 'Cascade' es el comportamiento deseado al borrar un producto.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir los cambios en el orden inverso del método Up()
            // Esto es crucial para poder deshacer la migración (dotnet ef database update <previous_migration_name>).

            migrationBuilder.DropForeignKey(
                name: "FK_Contactos_Productos_ProductoId",
                table: "Contactos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contactos",
                table: "Contactos");

            // Revertir 'ContactoId': Eliminar la nueva columna IDENTITY y recrear la versión anterior si existía
            // O simplemente quitar la anotación IDENTITY si ya existía sin ella.
            // Para ser robustos en el Down(), es mejor recrear el estado anterior.
            migrationBuilder.DropColumn( // Eliminar la ContactoId con IDENTITY
                name: "ContactoId",
                table: "Contactos");

            migrationBuilder.AddColumn<int>( // Recrear ContactoId como era antes (si no era IDENTITY)
                name: "ContactoId",
                table: "Contactos",
                type: "int",
                nullable: false, // Ajusta a cómo estaba antes si es diferente
                defaultValue: 0); // Ajusta a cómo estaba antes

            // Revertir 'ProductoId' a su estado anterior (nullable)
            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Contactos",
                type: "int",
                nullable: true, // Revertir a nullable
                oldClrType: typeof(int),
                oldType: "int");

            // Recrear la columna 'Id' con su propiedad IDENTITY original
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Contactos",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Restablece   r la clave primaria en 'Id'
            migrationBuilder.AddPrimaryKey(
                name: "PK_Contactos",
                table: "Contactos",
                column: "Id");

            // Restablecer la clave foránea a su estado original (nullable)
            migrationBuilder.AddForeignKey(
                name: "FK_Contactos_Productos_ProductoId",
                table: "Contactos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // O el onDelete que tuviera antes (probablemente Restrict)
        }
    }
}