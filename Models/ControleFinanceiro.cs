using ConsultorioMedico.Api.Models;

public class ControleFinanceiro
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public Guid ConsultaId { get; set; }
    public Consulta Consulta { get; set; }
    public Guid MedicoId { get; set; } 
    public Medico Medico { get; set; } 

    public string? GuiaConvenio { get; set; } 
    public string? NumeroCarteirinha { get; set; } 
}
