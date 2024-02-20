using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class refactTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Clientes_ClienteId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ClienteId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Limite",
                table: "Clientes");

            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "Transacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Transacoes");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Transacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Limite",
                table: "Clientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ClienteId",
                table: "Transacoes",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Clientes_ClienteId",
                table: "Transacoes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
