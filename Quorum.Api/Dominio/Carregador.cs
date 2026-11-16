namespace Quorum.Api.Dominio;

public class Carregador
{
    public int Id { get; init; }
    public int UnidadeConsumidoraId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal PotenciaMaximaKw { get; private set; }
    public bool Disponivel { get; private set; }

    public Carregador(int unidadeConsumidoraId, string nome, decimal potenciaMaximaKw, bool disponivel)
    {
        UnidadeConsumidoraId = unidadeConsumidoraId;
        Atualizar(nome, potenciaMaximaKw, disponivel);
    }

    public void Atualizar(string nome, decimal potenciaMaximaKw, bool disponivel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(potenciaMaximaKw);

        Nome = nome.Trim();
        PotenciaMaximaKw = potenciaMaximaKw;
        Disponivel = disponivel;
    }
}
