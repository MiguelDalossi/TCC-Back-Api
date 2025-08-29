using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultorioMedico.Api.Dtos
{
    public record MedicoCreateDto(
    [property: Required] Guid UserId,
    [property: Required, StringLength(20)] string CRM,
    [property: Required, StringLength(2, MinimumLength = 2)] string UF,
    [property: Required, StringLength(80)] string Especialidade
);

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