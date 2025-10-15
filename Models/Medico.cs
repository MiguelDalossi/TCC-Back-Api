using ConsultorioMedico.Api.Models;

public class Medico
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = default!;
    public string CRM { get; set; } = default!;
    public string UF { get; set; } = default!;
    public string Especialidade { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string CPF { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefone { get; set; } = default!; 
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}