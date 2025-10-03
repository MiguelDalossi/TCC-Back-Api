using System;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/prontuarios")]
    [Authorize(Policy = "MedicoOnly, Medico, Admin")]
    public class ProntuariosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProntuariosController(AppDbContext db) => _db = db;

        [HttpPost("{consultaId:guid}")]
        public async Task<IActionResult> Upsert(Guid consultaId, ProntuarioUpsertDto dto)
        {
            var c = await _db.Consultas.Include(x => x.Prontuario).FirstOrDefaultAsync(x => x.Id == consultaId);
            if (c is null) return NotFound();

            if (c.Prontuario is null)
                c.Prontuario = new Models.Prontuario { ConsultaId = consultaId };

            var p = c.Prontuario;
            p.QueixaPrincipal = dto.QueixaPrincipal;
            p.Hda = dto.Hda;
            p.Antecedentes = dto.Antecedentes;
            p.ExameFisico = dto.ExameFisico;
            p.HipotesesDiagnosticas = dto.HipotesesDiagnosticas;
            p.Conduta = dto.Conduta;
            p.AtualizadoEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}