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
    [Route("api/consultas")]
    public class ConsultasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ConsultasController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IEnumerable<ConsultaListDto>> List()
        {
            return await _db.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico).ThenInclude(m => m.User)
                .OrderBy(c => c.Inicio)
                .Select(c => new ConsultaListDto(c.Id, c.Inicio, c.Fim, c.Paciente.Nome, c.Medico.User.FullName ?? "", c.Status.ToString()))
                .ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ConsultaDetailDto>> Get(Guid id)
        {
            var c = await _db.Consultas
                .Include(x => x.Paciente)
                .Include(x => x.Medico).ThenInclude(m => m.User)
                .Include(x => x.Prontuario)
                .Include(x => x.Prescricoes)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c is null) return NotFound();

            return new ConsultaDetailDto(
                c.Id, c.Inicio, c.Fim, c.Status.ToString(),
                c.PacienteId, c.Paciente.Nome, c.MedicoId, c.Medico.User.FullName ?? "",
                c.Prontuario is null ? null :
                    new ProntuarioDetailDto(c.Prontuario.Id, c.Prontuario.QueixaPrincipal, c.Prontuario.Hda, c.Prontuario.Antecedentes,
                                            c.Prontuario.ExameFisico, c.Prontuario.HipotesesDiagnosticas, c.Prontuario.Conduta,
                                            c.Prontuario.CriadoEm, c.Prontuario.AtualizadoEm),
                c.Prescricoes.Select(p => new PrescricaoItemDto(p.Id, p.Medicamento, p.Posologia, p.Orientacoes)).ToList()
            );
        }

        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "Recepcao,Admin,Medico")]
        public async Task<IActionResult> Create(ConsultaCreateDto dto)
        {
            // conflito básico na agenda do médico
            var conflito = await _db.Consultas.AnyAsync(c =>
                c.MedicoId == dto.MedicoId && c.Status != StatusConsulta.Cancelada &&
                ((dto.Inicio >= c.Inicio && dto.Inicio < c.Fim) || (dto.Fim > c.Inicio && dto.Fim <= c.Fim)));

            if (conflito) return Conflict("Conflito de agenda para o médico.");

            var cst = new Consulta
            {
                PacienteId = dto.PacienteId,
                MedicoId = dto.MedicoId,
                Inicio = dto.Inicio,
                Fim = dto.Fim
            };
            _db.Consultas.Add(cst);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = cst.Id }, null);
        }

        [HttpPatch("{id:guid}/status")]
        [AllowAnonymous]
        [Authorize(Roles = "Medico,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, ConsultaStatusUpdateDto dto)
        {
            var c = await _db.Consultas.FindAsync(id);
            if (c is null) return NotFound();
            c.Status = Enum.Parse<StatusConsulta>(dto.Status.ToString());
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}