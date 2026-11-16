using Microsoft.EntityFrameworkCore;
using Quorum.Api.Contratos;
using Quorum.Api.Dados;
using Quorum.Api.Dominio;

namespace Quorum.Api.Servicos;

public class ControleDemandaService(QuorumDbContext banco) : IControleDemandaService
{
    public async Task<(bool UnidadeExiste, PlanoCarregamentoResposta? Plano)> ObterPlanoAsync(
        int unidadeId, CancellationToken cancellationToken)
    {
        var unidade = await banco.Unidades
            .AsNoTracking()
            .AsSingleQuery()
            .Include(u => u.Carregadores)
            .Include(u => u.Medicoes.OrderByDescending(m => m.RegistradaEm).ThenByDescending(m => m.Id).Take(1))
            .SingleOrDefaultAsync(u => u.Id == unidadeId, cancellationToken);

        if (unidade is null)
        {
            return (false, null);
        }

        var medicao = unidade.Medicoes.SingleOrDefault();
        if (medicao is null)
        {
            return (true, null);
        }

        var distribuicao = DistribuidorPotencia.Calcular(unidade.LimiteDemandaKw, medicao.DemandaBaseKw, unidade.Carregadores);
        var plano = new PlanoCarregamentoResposta(
            unidade.Id,
            medicao.Id,
            medicao.RegistradaEm,
            unidade.LimiteDemandaKw,
            medicao.DemandaBaseKw,
            distribuicao.FolgaKw,
            distribuicao.PotenciaAlocadaKw,
            distribuicao.ExcedeLimite,
            distribuicao.Carregadores);

        return (true, plano);
    }
}
