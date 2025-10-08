using System;
using System.Linq;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultorioMedico.Api.Services; // adicione


namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/prescricoes")]
    [Authorize(Roles = "Admin,MedicoOnly, Medico")]
    public class PrescricoesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PdfService _pdfService;

        public PrescricoesController(AppDbContext db, PdfService pdfService)
        {
            _db = db;
            _pdfService = pdfService;
        }

        [HttpPost("{consultaId:guid}")]
        public async Task<IActionResult> Upsert(Guid consultaId, PrescricaoUpsertDto dto)
        {
            var c = await _db.Consultas.Include(x => x.Prescricoes).FirstOrDefaultAsync(x => x.Id == consultaId);
            if (c is null) return NotFound();

            _db.Prescricoes.RemoveRange(c.Prescricoes);
            foreach (var i in dto.Itens)
                _db.Prescricoes.Add(new Models.PrescricaoItem
                {
                    ConsultaId = consultaId,
                    Medicamento = i.Medicamento,
                    Posologia = i.Posologia,
                    Orientacoes = i.Orientacoes ?? string.Empty
                });

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{consultaId:guid}/pdf/{itemId:guid}")]
        [Authorize(Roles = "Recepcao, Admin, MedicoOnly, Medico")]
        public async Task<IActionResult> GetPrescricaoItemPdf(Guid consultaId, Guid itemId)
        {
            var consulta = await _db.Consultas
                .Include(x => x.Paciente)
                .Include(x => x.Medico).ThenInclude(m => m.User)
                .Include(x => x.Prescricoes)
                .FirstOrDefaultAsync(x => x.Id == consultaId);

            if (consulta is null) return NotFound();

            var item = consulta.Prescricoes.FirstOrDefault(x => x.Id == itemId);
            if (item is null) return NotFound();

            var pdf = await _pdfService.GerarPrescricaoAsync(
                consulta.Paciente.Nome,
                consulta.Medico.User.FullName ?? "",
                new List<PrescricaoItemDto>
                {
            new PrescricaoItemDto(item.Id, item.Medicamento, item.Posologia, item.Orientacoes)
                }
            );

            var nomeArquivo = $"prescricao_{consultaId}_{item.Id}.pdf";
            return File(pdf, "application/pdf", nomeArquivo);
        }
    }
}