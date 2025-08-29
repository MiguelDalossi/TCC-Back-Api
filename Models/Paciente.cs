namespace ConsultorioMedico.Api.Models
{
    public class Paciente
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = default!;
        public DateTime DataNascimento { get; set; }
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }
        public string? Observacoes { get; set; }
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    }
}
