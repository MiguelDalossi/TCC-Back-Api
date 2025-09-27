namespace ConsultorioMedico.Api.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public DateTime DataUtc { get; set; } = DateTime.UtcNow;
        public Guid? UserId { get; set; }
        public string Acao { get; set; } = default!; // ex.: VIEW_PRONTUARIO, CREATE_CONSULTA
        public string Recurso { get; set; } = default!; // ex.: Consulta:GUID, Paciente:GUID
        public string? Ip { get; set; }
        public string? Detalhes { get; set; }
    }
}