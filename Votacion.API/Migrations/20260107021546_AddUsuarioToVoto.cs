using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votacion.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioToVoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Usuarios_UsuarioId",
                table: "Votos");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Votos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Usuarios_UsuarioId",
                table: "Votos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Usuarios_UsuarioId",
                table: "Votos");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Votos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Usuarios_UsuarioId",
                table: "Votos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");
        }
    }
}
