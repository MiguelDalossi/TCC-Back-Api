namespace ConsultorioMedico.Api.Models
{
    public class Prontuario
    {
        public Guid Id { get; set; }
        public Guid ConsultaId { get; set; }
        public Consulta Consulta { get; set; } = default!;


        public string QueixaPrincipal { get; set; } = string.Empty;
        public string Hda { get; set; } = string.Empty; // História da Doença Atual
        public string Antecedentes { get; set; } = string.Empty;
        public string ExameFisico { get; set; } = string.Empty;
        public string HipotesesDiagnosticas { get; set; } = string.Empty;
        public string Conduta { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }
    }
}
