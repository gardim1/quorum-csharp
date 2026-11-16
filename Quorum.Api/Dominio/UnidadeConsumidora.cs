namespace Quorum.Api.Dominio;

public class UnidadeConsumidora
{
    public int Id { get; init; }
    public string Nome { get; private set; } = string.Empty;
    public decimal LimiteDemandaKw { get; private set; }
    public List<Carregador> Carregadores { get; } = [];
    public List<MedicaoDemanda> Medicoes { get; } = [];

    public UnidadeConsumidora(string nome, decimal limiteDemandaKw)
    {
        Atualizar(nome, limiteDemandaKw);
    }

    public void Atualizar(string nome, decimal limiteDemandaKw)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limiteDemandaKw);

        Nome = nome.Trim();
        LimiteDemandaKw = limiteDemandaKw;
    }
}
