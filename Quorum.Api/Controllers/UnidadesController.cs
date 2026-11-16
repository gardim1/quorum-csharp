using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quorum.Api.Contratos;
using Quorum.Api.Dados;
using Quorum.Api.Dominio;
using Quorum.Api.Servicos;

namespace Quorum.Api.Controllers;

[ApiController]
[Route("api/unidades")]
public class UnidadesController(QuorumDbContext banco, IControleDemandaService controleDemanda) : ControllerBase
{
    [HttpGet]
    public async Task<List<UnidadeResposta>> Listar(CancellationToken cancellationToken)
    {
        return await banco.Unidades.AsNoTracking().OrderBy(u => u.Id)
            .Select(u => new UnidadeResposta(u.Id, u.Nome, u.LimiteDemandaKw))
            .ToListAsync(cancellationToken);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<UnidadeResposta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeResposta>> Obter(int id, CancellationToken cancellationToken)
    {
        var unidade = await banco.Unidades.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UnidadeResposta(u.Id, u.Nome, u.LimiteDemandaKw))
            .SingleOrDefaultAsync(cancellationToken);

        return unidade is null ? NotFound() : Ok(unidade);
    }

    [HttpPost]
    [ProducesResponseType<UnidadeResposta>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UnidadeResposta>> Criar(UnidadeEntrada entrada, CancellationToken cancellationToken)
    {
        var unidade = new UnidadeConsumidora(entrada.Nome, entrada.LimiteDemandaKw);
        banco.Unidades.Add(unidade);
        await banco.SaveChangesAsync(cancellationToken);

        var resposta = new UnidadeResposta(unidade.Id, unidade.Nome, unidade.LimiteDemandaKw);
        return CreatedAtAction(nameof(Obter), new { id = unidade.Id }, resposta);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, UnidadeEntrada entrada, CancellationToken cancellationToken)
    {
        var unidade = await banco.Unidades.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (unidade is null)
        {
            return NotFound();
        }

        unidade.Atualizar(entrada.Nome, entrada.LimiteDemandaKw);
        await banco.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        int excluidas = await banco.Unidades.Where(u => u.Id == id).ExecuteDeleteAsync(cancellationToken);
        return excluidas == 0 ? NotFound() : NoContent();
    }

    [HttpGet("{id:int}/plano-carregamento")]
    [ProducesResponseType<PlanoCarregamentoResposta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlanoCarregamentoResposta>> ObterPlano(int id, CancellationToken cancellationToken)
    {
        var (unidadeExiste, plano) = await controleDemanda.ObterPlanoAsync(id, cancellationToken);

        if (!unidadeExiste)
        {
            return NotFound();
        }

        if (plano is null)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "A unidade ainda não possui medições.");
        }

        return Ok(plano);
    }
}
