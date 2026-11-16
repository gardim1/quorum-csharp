using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quorum.Api.Contratos;
using Quorum.Api.Dados;
using Quorum.Api.Dominio;

namespace Quorum.Api.Controllers;

[ApiController]
[Route("api/unidades/{id:int}/carregadores")]
public class CarregadoresController(QuorumDbContext banco) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<CarregadorResposta>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CarregadorResposta>>> Listar(int id, CancellationToken cancellationToken)
    {
        if (!await banco.Unidades.AnyAsync(u => u.Id == id, cancellationToken))
        {
            return NotFound();
        }

        var carregadores = await banco.Carregadores.AsNoTracking()
            .Where(c => c.UnidadeConsumidoraId == id).OrderBy(c => c.Id)
            .Select(c => new CarregadorResposta(c.Id, c.UnidadeConsumidoraId, c.Nome, c.PotenciaMaximaKw, c.Disponivel))
            .ToListAsync(cancellationToken);

        return Ok(carregadores);
    }

    [HttpGet("{carregadorId:int}")]
    [ProducesResponseType<CarregadorResposta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarregadorResposta>> Obter(int id, int carregadorId, CancellationToken cancellationToken)
    {
        var carregador = await banco.Carregadores.AsNoTracking()
            .Where(c => c.UnidadeConsumidoraId == id && c.Id == carregadorId)
            .Select(c => new CarregadorResposta(c.Id, c.UnidadeConsumidoraId, c.Nome, c.PotenciaMaximaKw, c.Disponivel))
            .SingleOrDefaultAsync(cancellationToken);

        return carregador is null ? NotFound() : Ok(carregador);
    }

    [HttpPost]
    [ProducesResponseType<CarregadorResposta>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarregadorResposta>> Criar(int id, CarregadorEntrada entrada, CancellationToken cancellationToken)
    {
        if (!await banco.Unidades.AnyAsync(u => u.Id == id, cancellationToken))
        {
            return NotFound();
        }

        var carregador = new Carregador(id, entrada.Nome, entrada.PotenciaMaximaKw, entrada.Disponivel);
        banco.Carregadores.Add(carregador);
        await banco.SaveChangesAsync(cancellationToken);

        var resposta = new CarregadorResposta(carregador.Id, id, carregador.Nome, carregador.PotenciaMaximaKw, carregador.Disponivel);
        return CreatedAtAction(nameof(Obter), new { id, carregadorId = carregador.Id }, resposta);
    }

    [HttpPut("{carregadorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, int carregadorId, CarregadorEntrada entrada, CancellationToken cancellationToken)
    {
        var carregador = await banco.Carregadores.SingleOrDefaultAsync(
            c => c.UnidadeConsumidoraId == id && c.Id == carregadorId, cancellationToken);
        if (carregador is null)
        {
            return NotFound();
        }

        carregador.Atualizar(entrada.Nome, entrada.PotenciaMaximaKw, entrada.Disponivel);
        await banco.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{carregadorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, int carregadorId, CancellationToken cancellationToken)
    {
        int excluidos = await banco.Carregadores
            .Where(c => c.UnidadeConsumidoraId == id && c.Id == carregadorId).ExecuteDeleteAsync(cancellationToken);
        return excluidos == 0 ? NotFound() : NoContent();
    }
}
