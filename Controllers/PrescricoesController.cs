using System;
using System.Linq;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/prescricoes")]
    [Authorize(Policy = "MedicoOnly")]
    public class PrescricoesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PrescricoesController(AppDbContext db) => _db = db;

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
    }
}