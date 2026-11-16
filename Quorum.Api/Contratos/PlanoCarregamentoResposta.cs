using Quorum.Api.Dominio;

namespace Quorum.Api.Contratos;

public record PlanoCarregamentoResposta(
    int UnidadeConsumidoraId,
    int MedicaoId,
    DateTime RegistradaEm,
    decimal LimiteDemandaKw,
    decimal DemandaBaseKw,
    decimal FolgaKw,
    decimal PotenciaAlocadaKw,
    bool ExcedeLimite,
    IReadOnlyList<PotenciaCarregador> Carregadores);
