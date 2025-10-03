using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultorioMedico.Api.Dtos
{

    public record PrescricaoItemDto(
    Guid Id,
    [property: Required, StringLength(120)] string Medicamento,
    [property: Required, StringLength(200)] string Posologia,
    string? Orientacoes
);

    public record PrescricaoUpsertDto(
    [property: Required] string Medicamento,
    [property: Required] string Posologia,
    string? Orientacoes,
    [property: Required] List<PrescricaoItemUpsertDto> Itens
);

    public record PrescricaoItemUpsertDto(
        [property: Required, StringLength(120)] string Medicamento,
        [property: Required, StringLength(200)] string Posologia,
        string? Orientacoes
    );
}