using System.ComponentModel.DataAnnotations;
using System;
public enum StatusConsultaDto { Agendada, EmAndamento, Concluida, Cancelada }


namespace ConsultorioMedico.Api.Dtos
{
    public record ConsultaCreateDto(
        [property: Required] Guid PacienteId,
        [property: Required] Guid MedicoId,
        [property: Required] DateTime Inicio,
        [property: Required] DateTime Fim
    );

    public record ConsultaUpdateHorarioDto(
        [property: Required] DateTime Inicio,
        [property: Required] DateTime Fim
    );

    public record ConsultaStatusUpdateDto(
        [property: Required] StatusConsultaDto Status
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
}
