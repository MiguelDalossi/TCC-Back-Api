using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultorioMedico.Api.Dtos
{
    public record PrescricaoItemDto(
        Guid Id,
        [Required, StringLength(120)] string Medicamento,
        [Required, StringLength(200)] string Posologia,
        string? Orientacoes
    );

    // Removido os campos individuais, mantido apenas a lista de itens
    public record PrescricaoUpsertDto(
        [Required] List<PrescricaoItemUpsertDto> Itens
    );

    public record PrescricaoItemUpsertDto(
    [Required, StringLength(120)] string Medicamento,
    [Required, StringLength(200)] string Posologia,
    string? Orientacoes
    );
  
}
