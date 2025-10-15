using System.ComponentModel.DataAnnotations;
using System;
using ConsultorioMedico.Api.Models;
public enum StatusConsultaDto { Agendada, EmAndamento, Concluida, Cancelada }

namespace ConsultorioMedico.Api.Dtos
{
    public record ConsultaCreateDto(
    Guid PacienteId,
    Guid MedicoId,
    DateTime Inicio,
    DateTime Fim,
    TipoAtendimento TipoAtendimento,
    decimal ValorConsulta,
    string? NumeroCarteirinha,
    string? GuiaConvenio
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
    StatusPagamento StatusPagamento,
    TipoAtendimento TipoAtendimento,
    decimal ValorConsulta,
    string? NumeroCarteirinha,
    string? GuiaConvenio,
    ProntuarioDetailDto? Prontuario,
    List<PrescricaoItemDto> Prescricoes
);

    public class ConsultaUpdateDto
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public StatusConsultaDto Status { get; set; }
        public TipoAtendimento TipoAtendimento { get; set; }
        public decimal ValorConsulta { get; set; }
        public string? GuiaConvenio { get; set; }
        public string? NumeroCarteirinha { get; set; }
        public StatusPagamento StatusPagamento { get; set; } 
        public ProntuarioUpsertDto? Prontuario { get; set; }
        public List<PrescricaoItemUpsertDto>? Prescricoes { get; set; }
    }


    public class ConsultaHorarioUpdateDto
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
    }
}