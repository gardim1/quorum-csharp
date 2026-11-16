namespace Quorum.Api.Dominio;

public class MedicaoDemanda
{
    public int Id { get; init; }
    public int UnidadeConsumidoraId { get; private set; }
    public decimal DemandaBaseKw { get; private set; }
    public DateTime RegistradaEm { get; private set; }

    public MedicaoDemanda(int unidadeConsumidoraId, decimal demandaBaseKw, DateTime registradaEm)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(demandaBaseKw);

        UnidadeConsumidoraId = unidadeConsumidoraId;
        DemandaBaseKw = demandaBaseKw;
        RegistradaEm = registradaEm.ToUniversalTime();
    }
}
