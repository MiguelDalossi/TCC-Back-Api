using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultorioMedico.Api.Dtos
{
    public class MedicoCreateDto
    {
        public string CRM { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
    }

    public record MedicoUpdateDto(
        [property: Required, StringLength(20)] string CRM,
        [property: Required, StringLength(2, MinimumLength = 2)] string UF,
        [property: Required, StringLength(80)] string Especialidade
    );

    public record MedicoListDto(
        Guid Id, string Nome, string CRM, string UF, string Especialidade
    );

    public record MedicoDetailDto(
        Guid Id, Guid UserId, string Nome, string Email,
        string CRM, string UF, string Especialidade
    );
}