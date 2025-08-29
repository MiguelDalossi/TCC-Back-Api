using Microsoft.AspNetCore.Identity;
using System;

namespace ConsultorioMedico.Api.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? CrmUf { get; set; }
        public bool IsActive { get; set; } = true;
    }
}