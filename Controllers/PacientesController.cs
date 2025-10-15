using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using ConsultorioMedico.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Controllers

{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PacientesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteListDto>>> List()
            => await _db.Pacientes
                .OrderBy(p => p.Nome)
                .Select(p => new PacienteListDto(
                p.Id,
                p.Nome,
                p.DataNascimento,
                p.Telefone,
                p.Sexo,         // Novo campo
                p.Convenio,     // Novo campo
                p.Cidade,
                p.Estado
                ))
                .ToListAsync();

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PacienteDetailDto>> Get(Guid id)
        {
            var p = await _db.Pacientes.FindAsync(id);
            if (p is null) return NotFound();
            return new PacienteDetailDto(
    p.Id, p.Nome, p.DataNascimento, p.Cpf, p.Telefone, p.Email, p.Observacoes,
    p.Sexo,         // Novo campo
    p.Convenio,     // Novo campo
    p.Cidade, p.Estado, p.Bairro, p.Rua, p.Numero, p.Cep, p.Complemento
);
        }

        [HttpPost]
        [Authorize(Roles = "Recepcao,Admin,MedicoOnly, Medico")]
        public async Task<IActionResult> Create([FromBody] PacienteCreateDto dto)
        {
            var p = new Paciente
            {
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                Cpf = dto.Cpf,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Observacoes = dto.Observacoes,
                Sexo = dto.Sexo,             // Novo campo
                Convenio = dto.Convenio,     // Novo campo
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Bairro = dto.Bairro,
                Rua = dto.Rua,
                Numero = dto.Numero,
                Cep = dto.Cep,
                Complemento = dto.Complemento
            };
            _db.Pacientes.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = p.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Recepcao,Admin,MedicoOnly, Medico")]
        public async Task<IActionResult> Update(Guid id, PacienteUpdateDto dto)
        {
            var p = await _db.Pacientes.FindAsync(id);
            if (p is null) return NotFound();
            p.Nome = dto.Nome;
            p.DataNascimento = dto.DataNascimento;
            p.Cpf = dto.Cpf;
            p.Telefone = dto.Telefone;
            p.Email = dto.Email;
            p.Observacoes = dto.Observacoes;
            p.Sexo = dto.Sexo;             // Novo campo
            p.Convenio = dto.Convenio;     // Novo campo
            p.Cidade = dto.Cidade;
            p.Estado = dto.Estado;
            p.Bairro = dto.Bairro;
            p.Rua = dto.Rua;
            p.Numero = dto.Numero;
            p.Cep = dto.Cep;
            p.Complemento = dto.Complemento;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Recepcao,Admin,MedicoOnly, Medico")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var paciente = await _db.Pacientes
                .Include(p => p.Consultas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente is null)
                return NotFound();

            // Remove todas as consultas vinculadas (canceladas ou concluídas)
            var consultasParaRemover = paciente.Consultas
                .Where(c => c.Status == StatusConsulta.Cancelada)
                .ToList();

            _db.Consultas.RemoveRange(consultasParaRemover);

            _db.Pacientes.Remove(paciente);
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
