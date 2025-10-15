using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultorioMedico.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveValorConsultaPadraoFromMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorConsultaPadrao",
                table: "Medicos");

            migrationBuilder.AddColumn<string>(
                name: "GuiaConvenio",
                table: "ControleFinanceiro",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCarteirinha",
                table: "ControleFinanceiro",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuiaConvenio",
                table: "ControleFinanceiro");

            migrationBuilder.DropColumn(
                name: "NumeroCarteirinha",
                table: "ControleFinanceiro");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorConsultaPadrao",
                table: "Medicos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
