using Quorum.Api.Contratos;

namespace Quorum.Api.Servicos;

public interface IControleDemandaService
{
    Task<(bool UnidadeExiste, PlanoCarregamentoResposta? Plano)> ObterPlanoAsync(
        int unidadeId, CancellationToken cancellationToken);
}
