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
        public async Task<IEnumerable<ConsultaListDto>> List([FromQuery] Guid? pacienteId)
        {
            IQueryable<Consulta> query = _db.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .ThenInclude(m => m.User);

            if (pacienteId.HasValue)
                query = query.Where(c => c.PacienteId == pacienteId.Value);

            return await query
                .OrderBy(c => c.Inicio)
                .Select(c => new ConsultaListDto(
                    c.Id, c.Inicio, c.Fim, c.Paciente.Nome, c.Medico.User.FullName ?? "", c.Status.ToString()))
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
     c.Id,
     c.Inicio,
     c.Fim,
     c.Status.ToString(),
     c.PacienteId,
     c.Paciente.Nome,
     c.MedicoId,
     c.Medico.User.FullName ?? "",
     c.StatusPagamento,
     c.TipoAtendimento,
     c.ValorConsulta,
     c.NumeroCarteirinha,
     c.GuiaConvenio,
     c.Prontuario is null ? null :
         new ProntuarioDetailDto(
             c.Prontuario.Id,
             c.Prontuario.QueixaPrincipal,
             c.Prontuario.Hda,
             c.Prontuario.Antecedentes,
             c.Prontuario.ExameFisico,
             c.Prontuario.HipotesesDiagnosticas,
             c.Prontuario.Conduta,
             c.Prontuario.CriadoEm,
             c.Prontuario.AtualizadoEm
         ),
     c.Prescricoes.Select(p => new PrescricaoItemDto(p.Id, p.Medicamento, p.Posologia, p.Orientacoes)).ToList()
 );
        }

        [HttpPost]
        [Authorize(Roles = "Recepcao,Admin,MedicoOnly, Medico")]
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
                Fim = dto.Fim,
                TipoAtendimento = dto.TipoAtendimento,
                ValorConsulta = dto.TipoAtendimento == TipoAtendimento.Particular ? dto.ValorConsulta : 0,
                NumeroCarteirinha = dto.TipoAtendimento == TipoAtendimento.Convenio ? dto.NumeroCarteirinha : null,
                GuiaConvenio = dto.TipoAtendimento == TipoAtendimento.Convenio ? dto.GuiaConvenio : null
            };
            _db.Consultas.Add(cst);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = cst.Id }, null);
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Recepcao, MedicoOnly,Admin, Medico")]
        public async Task<IActionResult> UpdateStatus(Guid id, ConsultaStatusUpdateDto dto)
        {
            var c = await _db.Consultas.FindAsync(id);
            if (c is null) return NotFound();
            c.Status = Enum.Parse<StatusConsulta>(dto.Status.ToString());
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:guid}/horario")]
        [Authorize(Roles = "Recepcao, MedicoOnly,Admin, Medico")]
        public async Task<IActionResult> UpdateHorario(Guid id, ConsultaHorarioUpdateDto dto)
        {
            var consulta = await _db.Consultas.FindAsync(id);
            if (consulta is null) return NotFound();
            consulta.Inicio = dto.Inicio;
            consulta.Fim = dto.Fim;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:guid}/pagamento")]
        [Authorize(Roles = "Recepcao ,MedicoOnly, Medico, Admin")]
        public async Task<IActionResult> AtualizarPagamento(Guid id, [FromBody] StatusPagamento novoStatus)
        {
            var consulta = await _db.Consultas.FindAsync(id);
            if (consulta is null) return NotFound();

            if (consulta.StatusPagamento != StatusPagamento.Pago &&
                novoStatus == StatusPagamento.Pago &&
                consulta.TipoAtendimento == TipoAtendimento.Particular)
            {
                var jaExiste = await _db.ControleFinanceiro.AnyAsync(f => f.ConsultaId == consulta.Id);
                if (!jaExiste)
                {
                    _db.ControleFinanceiro.Add(new ControleFinanceiro
                    {
                        Id = Guid.NewGuid(),
                        Data = DateTime.UtcNow,
                        Valor = consulta.ValorConsulta,
                        ConsultaId = consulta.Id,
                        MedicoId = consulta.MedicoId // <-- ALTERAÇÃO AQUI
                    });
                }
            }

            consulta.StatusPagamento = novoStatus;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "MedicoOnly, Medico, Admin")]
        public async Task<IActionResult> Update(Guid id, ConsultaUpdateDto dto)
        {
            var consulta = await _db.Consultas
                .Include(c => c.Prontuario)
                .Include(c => c.Prescricoes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta is null)
                return NotFound();

            consulta.Inicio = dto.Inicio;
            consulta.Fim = dto.Fim;
            consulta.Status = Enum.Parse<StatusConsulta>(dto.Status.ToString());

            // Atualiza tipo de atendimento e campos relacionados
            consulta.TipoAtendimento = dto.TipoAtendimento;
            if (dto.TipoAtendimento == TipoAtendimento.Convenio)
            {
                consulta.GuiaConvenio = dto.GuiaConvenio;
                consulta.NumeroCarteirinha = dto.NumeroCarteirinha;
                consulta.ValorConsulta = 0;
            }
            else
            {
                consulta.GuiaConvenio = null;
                consulta.NumeroCarteirinha = null;
                consulta.ValorConsulta = dto.ValorConsulta;
            }

            // Atualiza status de pagamento
            var statusPagamentoAnterior = consulta.StatusPagamento;
            consulta.StatusPagamento = dto.StatusPagamento;

            // Lança no controle financeiro apenas se mudou para Pago e ainda não existe lançamento
            if (statusPagamentoAnterior != StatusPagamento.Pago &&
                dto.StatusPagamento == StatusPagamento.Pago &&
                consulta.TipoAtendimento == TipoAtendimento.Particular)
            {
                var jaExiste = await _db.ControleFinanceiro.AnyAsync(f => f.ConsultaId == consulta.Id);
                if (!jaExiste)
                {
                    _db.ControleFinanceiro.Add(new ControleFinanceiro
                    {
                        Id = Guid.NewGuid(),
                        Data = DateTime.UtcNow,
                        Valor = consulta.ValorConsulta,
                        ConsultaId = consulta.Id,
                        MedicoId = consulta.MedicoId // <-- ALTERAÇÃO AQUI
                    });
                }
            }

            // Prontuário
            if (dto.Prontuario is not null)
            {
                if (consulta.Prontuario is null)
                {
                    consulta.Prontuario = new Models.Prontuario
                    {
                        ConsultaId = consulta.Id,
                        CriadoEm = DateTime.UtcNow
                    };
                }
                consulta.Prontuario.QueixaPrincipal = dto.Prontuario!.QueixaPrincipal;
                consulta.Prontuario.Hda = dto.Prontuario!.Hda;
                consulta.Prontuario.Antecedentes = dto.Prontuario!.Antecedentes;
                consulta.Prontuario.ExameFisico = dto.Prontuario!.ExameFisico;
                consulta.Prontuario.HipotesesDiagnosticas = dto.Prontuario!.HipotesesDiagnosticas;
                consulta.Prontuario.Conduta = dto.Prontuario!.Conduta;

                consulta.Prontuario.AtualizadoEm = DateTime.UtcNow;
            }

            // Prescrições
            if (dto.Prescricoes != null)
            {
                _db.Prescricoes.RemoveRange(consulta.Prescricoes);
                foreach (var i in dto.Prescricoes)
                {
                    _db.Prescricoes.Add(new Models.PrescricaoItem
                    {
                        ConsultaId = consulta.Id,
                        Medicamento = i.Medicamento,
                        Posologia = i.Posologia,
                        Orientacoes = i.Orientacoes ?? string.Empty
                    });
                }
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }



    }
}