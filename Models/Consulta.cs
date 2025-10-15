namespace ConsultorioMedico.Api.Models;
public enum StatusConsulta { Agendada, EmAndamento, Concluida, Cancelada }
public enum StatusPagamento { NaoPago, Pago, EmProcesso }
public enum TipoAtendimento { Particular, Convenio }

public class Consulta
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; } = default!;
    public Guid MedicoId { get; set; }
    public Medico Medico { get; set; } = default!;
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
    public StatusConsulta Status { get; set; } = StatusConsulta.Agendada;

    // Novos campos
    public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.NaoPago;
    public TipoAtendimento TipoAtendimento { get; set; } = TipoAtendimento.Particular;
    public string? NumeroCarteirinha { get; set; }
    public string? GuiaConvenio { get; set; }
    public decimal ValorConsulta { get; set; }

    public Prontuario? Prontuario { get; set; }
    public ICollection<PrescricaoItem> Prescricoes { get; set; } = new List<PrescricaoItem>();
}
