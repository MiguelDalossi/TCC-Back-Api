using System.ComponentModel.DataAnnotations;
using System;
namespace ConsultorioMedico.Api.Dtos
{
    public record ProntuarioUpsertDto(
    [property: Required] string QueixaPrincipal,
    string Hda,
    string Antecedentes,
    string ExameFisico,
    string HipotesesDiagnosticas,
    string Conduta
);

    public record ProntuarioDetailDto(
        Guid Id,
        string QueixaPrincipal,
        string Hda,
        string Antecedentes,
        string ExameFisico,
        string HipotesesDiagnosticas,
        string Conduta,
        DateTime CriadoEm,
        DateTime? AtualizadoEm
    );
}