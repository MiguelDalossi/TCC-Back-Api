using System;
using ConsultorioMedico.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Consulta> Consultas => Set<Consulta>();
        public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
        public DbSet<PrescricaoItem> Prescricoes => Set<PrescricaoItem>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Medico> Medicos => Set<Medico>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Consulta 1—1 Prontuario
            b.Entity<Consulta>()
                .HasOne(c => c.Prontuario)
                .WithOne(p => p.Consulta)
                .HasForeignKey<Prontuario>(p => p.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Consulta 1—N PrescricaoItem
            b.Entity<Consulta>()
                .HasMany(c => c.Prescricoes)
                .WithOne(p => p.Consulta)
                .HasForeignKey(p => p.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Consulta N—1 Paciente
            b.Entity<Consulta>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Consultas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict); // evita apagar paciente e levar consultas junto

            // Consulta N—1 Medico
            b.Entity<Consulta>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Consultas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Medico 1—1 AppUser
            b.Entity<Medico>()
                .HasOne(m => m.User)
                .WithMany() // sem navegação inversa em AppUser
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            b.Entity<Paciente>()
                .HasIndex(p => p.Cpf)
                .IsUnique(false);

            b.Entity<Medico>()
                .HasIndex(m => new { m.CRM, m.UF })
                .IsUnique(); // CRM+UF único
        }
    }
}