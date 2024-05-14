using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barbearia.API.Data;
using Barbearia.API.DTO;
using Barbearia.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgendamentoController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AgendamentoController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var agendamentos = await _dbContext
                                        .Agendamentos
                                        .Include(a => a.Cliente)
                                        .Include(b => b.Servicos)
                                        .ToListAsync();
            return Ok(agendamentos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Agendamento>> GetById(int id)
        {
            var agendamento = await _dbContext.Agendamentos.FindAsync(id);
            if(agendamento == null)
            {
                return NotFound();
            }
            return Ok(agendamento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgendamentoDTO agendamentoDTO)
        {
            if(ModelState.IsValid)
            {
                Agendamento agendamento = new Agendamento();
                agendamento.ClienteId = agendamentoDTO.ClienteId;
                agendamento.Observacoes = agendamentoDTO.Observacoes;
                agendamento.DataHora = agendamentoDTO.DataHora;
                agendamento.Status = agendamentoDTO.Status;
                agendamento.Servicos = new List<Servico>();
                agendamento.Cliente = _dbContext.Clientes.Find(agendamento.ClienteId);

                foreach (var item in agendamentoDTO.Servicos)
                {
                    var servico = _dbContext.Servicos.Find(item.Id);
                    if(servico == null)
                    {
                        return BadRequest();
                    }
                    agendamento.Servicos.Add(servico);
                }

                _dbContext.Agendamentos.Add(agendamento);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = agendamento.AgendamentoID }, agendamento);
            }
            return BadRequest();
        }        

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, Agendamento agendamentoDTO)
        {
            if(id != agendamentoDTO.AgendamentoID)
            {
                return BadRequest();
            }

            if(ModelState.IsValid)
            {
                var agendamento = _dbContext
                                    .Agendamentos
                                    .Include(x => x.Servicos)
                                    .Include(b => b.Cliente)
                                    .FirstOrDefault(x => x.AgendamentoID == agendamentoDTO.AgendamentoID);

                if(agendamento == null)
                {
                    return NotFound();
                }

                agendamento.ClienteId = agendamentoDTO.ClienteId;
                agendamento.Observacoes = agendamentoDTO.Observacoes;
                agendamento.DataHora = agendamentoDTO.DataHora;
                agendamento.Status = agendamentoDTO.Status;
                agendamento.Servicos = new List<Servico>();
                agendamento.Cliente = _dbContext.Clientes.Find(agendamento.ClienteId);

                foreach (var item in agendamentoDTO.Servicos)
                {
                    var servico = _dbContext.Servicos.Find(item.Id);
                    if(servico == null)
                    {
                        return BadRequest();
                    }
                    agendamento.Servicos.Add(servico);
                }

                _dbContext.Agendamentos.Update(agendamento);
                await _dbContext.SaveChangesAsync();
                return Ok(agendamento);
            }
            return BadRequest();
        }        

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var agendamento = await _dbContext
                                    .Agendamentos
                                    .Include(x => x.Servicos)
                                    .Include(x => x.Cliente)
                                    .FirstOrDefaultAsync(x => x.AgendamentoID == id);

            if(agendamento == null)
            {
                return NotFound();
            }
            _dbContext.Agendamentos.Remove(agendamento);
            await _dbContext.SaveChangesAsync();
            return Ok(agendamento);
        }


    }
}