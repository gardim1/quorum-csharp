namespace Quorum.Api.Dominio;

public record PotenciaCarregador(
    int CarregadorId, string Nome, bool Disponivel, decimal PotenciaMaximaKw, decimal PotenciaAlocadaKw);

public record DistribuicaoPotencia(
    decimal FolgaKw, decimal PotenciaAlocadaKw, bool ExcedeLimite, IReadOnlyList<PotenciaCarregador> Carregadores);

public static class DistribuidorPotencia
{
    public static DistribuicaoPotencia Calcular(decimal limiteDemandaKw, decimal demandaBaseKw, IEnumerable<Carregador> carregadores)
    {
        // A demanda base exclui os carregadores gerenciados.
        decimal folgaKw = Math.Max(0, limiteDemandaKw - demandaBaseKw);
        decimal restanteKw = folgaKw;
        var alocacoes = new List<PotenciaCarregador>();

        foreach (var carregador in carregadores.OrderBy(c => c.Id))
        {
            decimal alocadaKw = carregador.Disponivel
                ? Math.Min(restanteKw, carregador.PotenciaMaximaKw)
                : 0;

            alocacoes.Add(new PotenciaCarregador(
                carregador.Id, carregador.Nome, carregador.Disponivel, carregador.PotenciaMaximaKw, alocadaKw));
            restanteKw -= alocadaKw;
        }

        return new DistribuicaoPotencia(
            folgaKw, alocacoes.Sum(c => c.PotenciaAlocadaKw), demandaBaseKw > limiteDemandaKw, alocacoes);
    }
}
