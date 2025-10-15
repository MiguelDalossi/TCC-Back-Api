using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultorioMedico.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddControleFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorConsultaPadrao",
                table: "Medicos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GuiaConvenio",
                table: "Consultas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCarteirinha",
                table: "Consultas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusPagamento",
                table: "Consultas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoAtendimento",
                table: "Consultas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorConsulta",
                table: "Consultas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ControleFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControleFinanceiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControleFinanceiro_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControleFinanceiro_ConsultaId",
                table: "ControleFinanceiro",
                column: "ConsultaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControleFinanceiro");

            migrationBuilder.DropColumn(
                name: "ValorConsultaPadrao",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "GuiaConvenio",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "NumeroCarteirinha",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "StatusPagamento",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "TipoAtendimento",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "ValorConsulta",
                table: "Consultas");
        }
    }
}
