using System.ComponentModel.DataAnnotations;
using System;
public enum StatusConsultaDto { Agendada, EmAndamento, Concluida, Cancelada }

namespace ConsultorioMedico.Api.Dtos
{
    public record ConsultaCreateDto(
        [Required] Guid PacienteId,
        [Required] Guid MedicoId,
        [Required] DateTime Inicio,
        [Required] DateTime Fim
    );

    public record ConsultaUpdateHorarioDto(
        [Required] DateTime Inicio,
        [Required] DateTime Fim
    );

    public record ConsultaStatusUpdateDto(
        [Required] StatusConsultaDto Status
    );

    public record ConsultaListDto(
        Guid Id, DateTime Inicio, DateTime Fim, string PacienteNome, string MedicoNome, string Status
    );

    public record ConsultaDetailDto(
        Guid Id,
        DateTime Inicio,
        DateTime Fim,
        string Status,
        Guid PacienteId,
        string PacienteNome,
        Guid MedicoId,
        string MedicoNome,
        ProntuarioDetailDto? Prontuario,
        List<PrescricaoItemDto> Prescricoes
    );

    public class ConsultaUpdateDto
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public string Status { get; set; }
        public ProntuarioUpsertDto? Prontuario { get; set; }
        public List<PrescricaoItemUpsertDto>? Prescricoes { get; set; }
    }

    public class ConsultaHorarioUpdateDto
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
    }
}