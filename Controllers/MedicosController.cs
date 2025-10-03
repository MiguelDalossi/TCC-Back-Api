using System;
using System.Linq;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using ConsultorioMedico.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

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
        [Authorize(Roles = "Admin,Recepcao,MedicoOnly, Medico")]
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
    m.Especialidade,
    m.CPF ?? "",
    m.User.Email ?? "",
    m.Telefone // Adicionado
))
                .ToListAsync();

            return Ok(new { total, page, pageSize, itens });
        }

        // GET /api/medicos/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Recepcao,MedicoOnly, Medico")]
        public async Task<ActionResult<MedicoDetailDto>> Get(Guid id)
        {
            var medico = await _db.Medicos
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (medico is null) return NotFound();

            return new MedicoDetailDto(
    medico.Id,
    medico.UserId,
    medico.User.FullName ?? "",
    medico.User.Email ?? "",
    medico.CRM,
    medico.UF,
    medico.Especialidade,
    medico.CPF ?? "",
    medico.Email ?? "",
    medico.Telefone // Adicionado
);
        }

        // POST /api/medicos
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MedicoCreateDto dto, [FromServices] UserManager<AppUser> userMgr)
        {
            // Verifica se já existe usuário com esse e-mail
            var userExist = await userMgr.FindByEmailAsync(dto.Email);
            if (userExist != null)
                return Conflict("Já existe um usuário com esse e-mail.");

            // Cria o usuário
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                FullName = dto.Nome
            };

            var result = await userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // Adiciona à role "Medico"
            await userMgr.AddToRoleAsync(user, "Medico");

            // Cria o médico vinculado ao usuário
            var crmDuplicado = await _db.Medicos.AnyAsync(x => x.CRM == dto.CRM && x.UF == dto.UF);
            if (crmDuplicado)
                return Conflict("Já existe um médico com esse CRM/UF.");

            var medico = new Medico
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CRM = dto.CRM.Trim(),
                UF = dto.UF.Trim().ToUpper(),
                Especialidade = dto.Especialidade.Trim(),
                Nome = dto.Nome.Trim(),
                CPF = dto.CPF.Trim(),
                Email = dto.Email.Trim(),
                Telefone = dto.Telefone.Trim() // Adicionado
            };

            _db.Medicos.Add(medico);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = medico.Id }, null);
        }

        // PUT /api/medicos/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,MedicoOnly, Medico")]
        public async Task<IActionResult> Update(Guid id, MedicoUpdateDto dto)
        {
            var medico = await _db.Medicos
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (medico is null) return NotFound();

            var novoCRM = dto.CRM.Trim();
            var novaUF = dto.UF.Trim().ToUpper();

            var conflito = await _db.Medicos
                .AnyAsync(x => x.Id != id && x.CRM.ToLower() == novoCRM.ToLower() && x.UF.ToUpper() == novaUF);
            if (conflito) return Conflict("Já existe um médico com esse CRM/UF.");

            medico.CRM = novoCRM;
            medico.UF = novaUF;
            medico.Especialidade = dto.Especialidade?.Trim() ?? medico.Especialidade;
            medico.Nome = dto.Nome?.Trim() ?? medico.Nome;
            medico.CPF = dto.CPF?.Trim() ?? medico.CPF;
            medico.Email = dto.Email?.Trim() ?? medico.Email;
            medico.Telefone = dto.Telefone?.Trim() ?? medico.Telefone;

            // Atualiza também o usuário, se necessário
            if (medico.User != null)
            {
                medico.User.FullName = medico.Nome;
                medico.User.Email = medico.Email;
                medico.User.UserName = medico.Email;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/medicos/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, [FromServices] UserManager<AppUser> userMgr)
        {
            var medico = await _db.Medicos
                .Include(x => x.Consultas)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (medico is null) return NotFound();

            if (medico.Consultas.Any())
                return Conflict("Não é possível excluir: há consultas vinculadas a este médico.");

            // Remove o médico
            _db.Medicos.Remove(medico);
            await _db.SaveChangesAsync();

            // Remove o usuário vinculado
            if (medico.User != null)
                await userMgr.DeleteAsync(medico.User);

            return NoContent();
        }
    }
}