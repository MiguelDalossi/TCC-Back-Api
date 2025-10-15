using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultorioMedico.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicoIdToControleFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MedicoId",
                table: "ControleFinanceiro",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_ControleFinanceiro_MedicoId",
                table: "ControleFinanceiro",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ControleFinanceiro_Medicos_MedicoId",
                table: "ControleFinanceiro",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControleFinanceiro_Medicos_MedicoId",
                table: "ControleFinanceiro");

            migrationBuilder.DropIndex(
                name: "IX_ControleFinanceiro_MedicoId",
                table: "ControleFinanceiro");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "ControleFinanceiro");
        }

    }
}
