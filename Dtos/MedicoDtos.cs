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
    [property: Required, StringLength(20)] string CRM,
    [property: Required, StringLength(2, MinimumLength = 2)] string UF,
    [property: Required, StringLength(80)] string Especialidade,
    [property: Required, StringLength(120)] string Nome,
    [property: Required, StringLength(14)] string CPF,
    [property: Required, EmailAddress] string Email,
    [property: Required, StringLength(20)] string Telefone
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