using System.ComponentModel.DataAnnotations;

public class MedicoCreateDto
{
    public string CRM { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty; // Já existe
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public record MedicoUpdateDto(
    [Required, StringLength(20)] string CRM,
    [Required, StringLength(2, MinimumLength = 2)] string UF,
    [Required, StringLength(80)] string Especialidade,
    [Required, StringLength(120)] string Nome,
    [Required, StringLength(14)] string CPF,
    [Required, EmailAddress] string Email,
    [Required, StringLength(20)] string Telefone // Corrigido: validação no parâmetro
);


public record MedicoListDto(
    Guid Id, string Nome, string CRM, string UF, string Especialidade, string CPF, string Email, string Telefone // Adicionado
);

public record MedicoDetailDto(
    Guid Id,
    Guid UserId,
    string Nome,
    string Email,
    string CRM,
    string UF,
    string Especialidade,
    string CPF,
    string EmailMedico,
    string Telefone // Adicionado
);