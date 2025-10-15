using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultorioMedico.Api.Dtos
{
    public record PacienteCreateDto(
    [Required, StringLength(120)] string Nome,
    [Required] DateTime DataNascimento,
    [StringLength(14)] string? Cpf,
    [StringLength(20)] string? Telefone,
    [EmailAddress] string? Email,
    string? Observacoes,
    string? Sexo,         // Novo campo
    string? Convenio,     // Novo campo
    string? Cidade,
    string? Estado,
    string? Bairro,
    string? Rua,
    string? Numero,
    string? Cep,
    string? Complemento
);

    public record PacienteUpdateDto(
     [Required, StringLength(120)] string Nome,
     [Required] DateTime DataNascimento,
     [StringLength(14)] string? Cpf,
     [StringLength(20)] string? Telefone,
     [EmailAddress] string? Email,
     string? Observacoes,
     string? Sexo,         // Novo campo
     string? Convenio,     // Novo campo
     string? Cidade,
     string? Estado,
     string? Bairro,
     string? Rua,
     string? Numero,
     string? Cep,
     string? Complemento
 );

    public record PacienteListDto(
    Guid Id,
    string Nome,
    DateTime DataNascimento,
    string? Telefone,
    string? Sexo,         // Novo campo
    string? Convenio,     // Novo campo
    string? Cidade,
    string? Estado
);

    public record PacienteDetailDto(
    Guid Id, string Nome, DateTime DataNascimento, string? Cpf,
    string? Telefone, string? Email, string? Observacoes,
    string? Sexo,         // Novo campo
    string? Convenio,     // Novo campo
    string? Cidade, string? Estado, string? Bairro, string? Rua, string? Numero, string? Cep, string? Complemento
);
}