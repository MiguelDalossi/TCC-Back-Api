using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioMedico.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControleFinanceiroController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ControleFinanceiroController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControleFinanceiroDto>>> Listar()
        {
            var lista = await _db.ControleFinanceiro
                .Include(f => f.Consulta)
                    .ThenInclude(c => c.Paciente)
                .Include(f => f.Medico)
                .Select(f => new ControleFinanceiroDto(
                    f.Id,
                    f.Data,
                    f.Valor,
                    f.ConsultaId,
                    f.Consulta.Paciente.Nome,
                    f.Medico.Nome,
                    f.MedicoId
                ))
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ControleFinanceiroDto>> Detalhe(Guid id)
        {
            var f = await _db.ControleFinanceiro
                .Include(f => f.Consulta)
                    .ThenInclude(c => c.Paciente)
                .Include(f => f.Medico)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (f == null) return NotFound();

            return new ControleFinanceiroDto(
                f.Id,
                f.Data,
                f.Valor,
                f.ConsultaId,
                f.Consulta.Paciente.Nome,
                f.Medico.Nome,
                f.MedicoId
            );
        }

        [HttpGet("medico/{medicoId:guid}")]
        public async Task<ActionResult<IEnumerable<ControleFinanceiroDto>>> ListarPorMedico(Guid medicoId)
        {
            var lista = await _db.ControleFinanceiro
                .Where(f => f.MedicoId == medicoId)
                .Include(f => f.Consulta)
                    .ThenInclude(c => c.Paciente)
                .Include(f => f.Medico)
                .Select(f => new ControleFinanceiroDto(
                    f.Id,
                    f.Data,
                    f.Valor,
                    f.ConsultaId,
                    f.Consulta.Paciente.Nome,
                    f.Medico.Nome,
                    f.MedicoId
                ))
                .ToListAsync();

            return Ok(lista);
        }
    }
}
