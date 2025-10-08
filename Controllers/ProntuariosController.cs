using System;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using ConsultorioMedico.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/prontuarios")]
    [Authorize(Roles = "Admin,MedicoOnly, Medico")]
    public class ProntuariosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PdfService _pdfService;

        public ProntuariosController(AppDbContext db, PdfService pdfService)
        {
            _db = db;
            _pdfService = pdfService;
        }

        [HttpPost("{consultaId:guid}")]
        public async Task<IActionResult> Upsert(Guid consultaId, ProntuarioUpsertDto dto)
        {
            var c = await _db.Consultas.Include(x => x.Prontuario).FirstOrDefaultAsync(x => x.Id == consultaId);
            if (c is null) return NotFound();

            if (c.Prontuario is null)
                c.Prontuario = new Models.Prontuario { ConsultaId = consultaId, CriadoEm = DateTime.UtcNow };


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

        [HttpGet("{consultaId:guid}/pdf")]
        public async Task<IActionResult> GetProntuarioPdf(Guid consultaId)
        {
            var consulta = await _db.Consultas
                .Include(x => x.Paciente)
                .Include(x => x.Medico).ThenInclude(m => m.User)
                .Include(x => x.Prontuario)
                .FirstOrDefaultAsync(x => x.Id == consultaId);

            if (consulta is null || consulta.Prontuario is null) return NotFound();

            var prontuarioDto = new ProntuarioDetailDto(
                consulta.Prontuario.Id,
                consulta.Prontuario.QueixaPrincipal,
                consulta.Prontuario.Hda,
                consulta.Prontuario.Antecedentes,
                consulta.Prontuario.ExameFisico,
                consulta.Prontuario.HipotesesDiagnosticas,
                consulta.Prontuario.Conduta,
                consulta.Prontuario.CriadoEm,
                consulta.Prontuario.AtualizadoEm
            );

            var pdf = await _pdfService.GerarProntuarioAsync(
                consulta.Paciente.Nome,
                consulta.Medico.User.FullName ?? "",
                prontuarioDto
            );

            return File(pdf, "application/pdf", $"prontuario_{consultaId}.pdf");
        }

    }
}