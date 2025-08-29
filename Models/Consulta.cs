namespace ConsultorioMedico.Api.Models;
public enum StatusConsulta { Agendada, EmAndamento, Concluida, Cancelada }

public class Consulta
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; } = default!;


    public Guid MedicoId { get; set; } // AppUser.Id
    public Medico Medico { get; set; } = default!;


    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
    public StatusConsulta Status { get; set; } = StatusConsulta.Agendada;


    public Prontuario? Prontuario { get; set; } // 1–1
    public ICollection<PrescricaoItem> Prescricoes { get; set; } = new List<PrescricaoItem>();
}

