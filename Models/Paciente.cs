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
        public string? Observacoes { get; set; }

        // Novos campos
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Bairro { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Cep { get; set; }
        public string? Complemento { get; set; }

        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    }
}