namespace ConsultorioMedico.Api.Models
{
    public class Medico
    {
        public Guid Id { get; set; }

        // Relacionamento com o usuário do Identity
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = default!;

        // Dados específicos do médico
        public string CRM { get; set; } = default!;
        public string UF { get; set; } = default!;
        public string Especialidade { get; set; } = default!;

        // Relacionamento com consultas
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    }
}
