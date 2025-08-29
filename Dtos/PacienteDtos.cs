using System.ComponentModel.DataAnnotations;
using System;

namespace ConsultorioMedico.Api.Dtos
{
    public record PacienteCreateDto(
    [property: Required, StringLength(120)] string Nome,
    [property: Required] DateTime DataNascimento,
    [property: StringLength(14)] string? Cpf,
    [property: StringLength(20)] string? Telefone,
    [property: EmailAddress] string? Email,
    string? Endereco,
    string? Observacoes
);

    public record PacienteUpdateDto(
        [property: Required, StringLength(120)] string Nome,
        [property: Required] DateTime DataNascimento,
        [property: StringLength(14)] string? Cpf,
        [property: StringLength(20)] string? Telefone,
        [property: EmailAddress] string? Email,
        string? Endereco,
        string? Observacoes
    );

    public record PacienteListDto(
        Guid Id, string Nome, DateTime DataNascimento, string? Telefone
    );

    public record PacienteDetailDto(
        Guid Id, string Nome, DateTime DataNascimento, string? Cpf,
        string? Telefone, string? Email, string? Endereco, string? Observacoes
    );
}