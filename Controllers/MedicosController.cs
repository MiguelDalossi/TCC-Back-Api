using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using ConsultorioMedico.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/medicos")]
    public class MedicosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MedicosController(AppDbContext db) => _db = db;

        // GET /api/medicos?q=cardio&page=1&pageSize=20
        [HttpGet]
        [Authorize(Roles = "Admin,Recepcao,MedicoOnly")]
        public async Task<ActionResult<object>> List([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize is <= 0 or > 100 ? 20 : pageSize;

            var query = _db.Medicos
                .Include(m => m.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(m =>
                    m.CRM.ToLower().Contains(term) ||
                    m.UF.ToLower().Contains(term) ||
                    m.Especialidade.ToLower().Contains(term) ||
                    (m.User.FullName ?? "").ToLower().Contains(term) ||
                    (m.User.Email ?? "").ToLower().Contains(term));
            }

            var total = await query.CountAsync();

            var itens = await query
                .OrderBy(m => m.User.FullName)
                .ThenBy(m => m.CRM)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MedicoListDto(
                    m.Id,
                    m.User.FullName ?? "",
                    m.CRM,
                    m.UF,
                    m.Especialidade
                ))
                .ToListAsync();

            return Ok(new { total, page, pageSize, itens });
        }

        // GET /api/medicos/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Recepcao,MedicoOnly")]
        public async Task<ActionResult<MedicoDetailDto>> Get(Guid id)
        {
            var m = await _db.Medicos
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m is null) return NotFound();

            return new MedicoDetailDto(
                m.Id,
                m.UserId,
                m.User.FullName ?? "",
                m.User.Email ?? "",
                m.CRM,
                m.UF,
                m.Especialidade
            );
        }

        // POST /api/medicos
        [HttpPost]
        [Authorize(Roles = "Admin,MedicoOnly")]
        public async Task<IActionResult> Create(MedicoCreateDto dto)
        {
            // Tenta obter o UserId do token JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized("Token de autenticação inválido ou ausente.");

            // Verifica se já existe médico para este usuário
            var jaVinculado = await _db.Medicos.AnyAsync(x => x.UserId == userId);
            if (jaVinculado)
                return Conflict("Este usuário já está vinculado a um médico.");

            // Unique CRM+UF
            var crmDuplicado = await _db.Medicos.AnyAsync(x => x.CRM == dto.CRM && x.UF == dto.UF);
            if (crmDuplicado)
                return Conflict("Já existe um médico com esse CRM/UF.");

            var medico = new Medico
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CRM = dto.CRM.Trim(),
                UF = dto.UF.Trim().ToUpper(),
                Especialidade = dto.Especialidade.Trim()
            };

            _db.Medicos.Add(medico);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = medico.Id }, null);
        }

        // PUT /api/medicos/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,MedicoOnly")]
        public async Task<IActionResult> Update(Guid id, MedicoUpdateDto dto)
        {
            var m = await _db.Medicos.FindAsync(id);
            if (m is null) return NotFound();

            var novoCRM = dto.CRM.Trim();
            var novaUF = dto.UF.Trim().ToUpper();

            // Unique CRM+UF (excluindo o próprio registro)
            var conflito = await _db.Medicos
                .AnyAsync(x => x.Id != id && x.CRM == novoCRM && x.UF == novaUF);
            if (conflito) return Conflict("Já existe um médico com esse CRM/UF.");

            m.CRM = novoCRM;
            m.UF = novaUF;
            m.Especialidade = dto.Especialidade.Trim();

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/medicos/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var m = await _db.Medicos
                .Include(x => x.Consultas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m is null) return NotFound();

            // Regra de negócio: impedir exclusão se houver consultas vinculadas
            if (m.Consultas.Any())
                return Conflict("Não é possível excluir: há consultas vinculadas a este médico.");

            _db.Medicos.Remove(m);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
