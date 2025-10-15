using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultorioMedico.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicoInFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MedicoId",
                table: "ControleFinanceiro",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        /// <inheritdoc />
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
