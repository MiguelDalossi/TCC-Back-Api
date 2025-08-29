using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultorioMedico.Api.Dtos
{
    public sealed class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required, StringLength(120)]
        public string FullName { get; set; } = null!;

        // "Medico", "Recepcao", "Admin" (opcional; defina padrão no controller)
        public string? Role { get; set; }
    }

    public sealed class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public sealed class AuthResultDto
    {
        public string Token { get; init; } = null!;
        public DateTime ExpiresAt { get; init; }
        public string FullName { get; init; } = "";
        public string Email { get; init; } = "";
        public string Role { get; init; } = "";
    }
}