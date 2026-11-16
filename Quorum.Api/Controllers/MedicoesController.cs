using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quorum.Api.Contratos;
using Quorum.Api.Dados;
using Quorum.Api.Dominio;

namespace Quorum.Api.Controllers;

[ApiController]
[Route("api/unidades/{id:int}/medicoes")]
public class MedicoesController(QuorumDbContext banco) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ResumoMedicoes>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumoMedicoes>> Listar(
        int id, [FromQuery] DateTimeOffset? inicio, [FromQuery] DateTimeOffset? fim, CancellationToken cancellationToken)
    {
        if (inicio > fim)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "A data inicial deve ser anterior ou igual à data final.");
        }

        if (!await banco.Unidades.AnyAsync(u => u.Id == id, cancellationToken))
        {
            return NotFound();
        }

        var consulta = banco.Medicoes.AsNoTracking().Where(m => m.UnidadeConsumidoraId == id);
        if (inicio.HasValue)
        {
            consulta = consulta.Where(m => m.RegistradaEm >= inicio.Value.UtcDateTime);
        }

        if (fim.HasValue)
        {
            consulta = consulta.Where(m => m.RegistradaEm <= fim.Value.UtcDateTime);
        }

        var medicoes = await consulta.OrderByDescending(m => m.RegistradaEm).ThenByDescending(m => m.Id)
            .Select(m => new MedicaoResposta(m.Id, m.UnidadeConsumidoraId, m.DemandaBaseKw, m.RegistradaEm))
            .ToListAsync(cancellationToken);

        double? mediaKw = medicoes.Count == 0 ? null : medicoes.Average(m => (double)m.DemandaBaseKw);
        decimal? maximaKw = medicoes.Max(m => (decimal?)m.DemandaBaseKw);
        return Ok(new ResumoMedicoes(medicoes.Count, mediaKw, maximaKw, medicoes));
    }

    [HttpPost]
    [ProducesResponseType<MedicaoResposta>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicaoResposta>> Registrar(int id, MedicaoEntrada entrada, CancellationToken cancellationToken)
    {
        if (!await banco.Unidades.AnyAsync(u => u.Id == id, cancellationToken))
        {
            return NotFound();
        }

        DateTime registradaEm = entrada.RegistradaEm?.UtcDateTime ?? DateTime.UtcNow;
        var medicao = new MedicaoDemanda(id, entrada.DemandaBaseKw, registradaEm);
        banco.Medicoes.Add(medicao);
        await banco.SaveChangesAsync(cancellationToken);

        var resposta = new MedicaoResposta(medicao.Id, id, medicao.DemandaBaseKw, medicao.RegistradaEm);
        return StatusCode(StatusCodes.Status201Created, resposta);
    }
}
