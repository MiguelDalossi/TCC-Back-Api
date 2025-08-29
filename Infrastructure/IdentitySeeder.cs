using System;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultorioMedico.Api.Infrastructure
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var roles = new[] { "Admin", "Medico", "Recepcao" };
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole<Guid>(r));

            // usuário médico demo
            const string email = "medico@demo.com";
            var user = await userMgr.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Médico Demo"
                };
                await userMgr.CreateAsync(user, "SenhaF0rte!");
                await userMgr.AddToRoleAsync(user, "Medico");

                // vincula na tabela Medicos
                var medico = new Medico
                {
                    UserId = user.Id,
                    CRM = "12345",
                    UF = "SP",
                    Especialidade = "Clínica Médica"
                };
                db.Medicos.Add(medico);
                await db.SaveChangesAsync();
            }
        }
    }
}