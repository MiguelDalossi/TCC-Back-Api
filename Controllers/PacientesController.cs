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
                .Select(p => new PacienteListDto(p.Id, p.Nome, p.DataNascimento, p.Telefone))
                .ToListAsync();

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PacienteDetailDto>> Get(Guid id)
        {
            var p = await _db.Pacientes.FindAsync(id);
            if (p is null) return NotFound();
            return new PacienteDetailDto(p.Id, p.Nome, p.DataNascimento, p.Cpf, p.Telefone, p.Email, p.Endereco, p.Observacoes);
        }

        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "Recepcao,Admin,Medico")]
        public async Task<IActionResult> Create(PacienteCreateDto dto)
        {
            var p = new Paciente
            {
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                Cpf = dto.Cpf,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Endereco = dto.Endereco,
                Observacoes = dto.Observacoes
            };
            _db.Pacientes.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = p.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Recepcao,Admin,Medico")]
        public async Task<IActionResult> Update(Guid id, PacienteUpdateDto dto)
        {
            var p = await _db.Pacientes.FindAsync(id);
            if (p is null) return NotFound();
            p.Nome = dto.Nome; p.DataNascimento = dto.DataNascimento; p.Cpf = dto.Cpf;
            p.Telefone = dto.Telefone; p.Email = dto.Email; p.Endereco = dto.Endereco; p.Observacoes = dto.Observacoes;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}