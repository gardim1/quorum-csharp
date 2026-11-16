using Quorum.Api.Dominio;
using Xunit;

namespace Quorum.Tests;

public class DistribuidorPotenciaTests
{
    [Fact]
    public void PriorizaOIdMenorERespeitaAFolga()
    {
        Carregador[] carregadores =
        [
            new(1, "Carregador 22 kW", 22, true) { Id = 2 },
            new(1, "Carregador 11 kW", 11, true) { Id = 1 }
        ];

        var resultado = DistribuidorPotencia.Calcular(50, 32, carregadores);

        Assert.Equal(18m, resultado.FolgaKw);
        Assert.Equal(18m, resultado.PotenciaAlocadaKw);
        Assert.False(resultado.ExcedeLimite);
        Assert.Collection(resultado.Carregadores,
            item =>
            {
                Assert.Equal(1, item.CarregadorId);
                Assert.Equal(11m, item.PotenciaAlocadaKw);
            },
            item =>
            {
                Assert.Equal(2, item.CarregadorId);
                Assert.Equal(7m, item.PotenciaAlocadaKw);
            });
    }

    [Fact]
    public void CarregadorIndisponivelNaoConsomeAFolga()
    {
        Carregador[] carregadores =
        [
            new(1, "Inativo", 11, false) { Id = 1 },
            new(1, "Ativo", 22, true) { Id = 2 }
        ];

        var resultado = DistribuidorPotencia.Calcular(50, 32, carregadores);

        Assert.Equal(new decimal[] { 0, 18 }, resultado.Carregadores.Select(c => c.PotenciaAlocadaKw));
    }

    [Theory]
    [InlineData(50, false)]
    [InlineData(56, true)]
    public void BaseNoLimiteOuAcimaZeraAlocacao(int demandaBase, bool excede)
    {
        Carregador[] carregadores = [new(1, "Carregador 22 kW", 22, true) { Id = 1 }];

        var resultado = DistribuidorPotencia.Calcular(50, demandaBase, carregadores);

        Assert.Equal(0m, resultado.FolgaKw);
        Assert.Equal(0m, resultado.PotenciaAlocadaKw);
        Assert.Equal(excede, resultado.ExcedeLimite);
        Assert.Equal(0m, Assert.Single(resultado.Carregadores).PotenciaAlocadaKw);
    }

    [Fact]
    public void PotenciaAlocadaRespeitaOTetoIndividual()
    {
        Carregador[] carregadores = [new(1, "Carregador 22 kW", 22, true) { Id = 1 }];

        var resultado = DistribuidorPotencia.Calcular(50, 0, carregadores);

        Assert.Equal(22m, resultado.PotenciaAlocadaKw);
        Assert.Equal(22m, Assert.Single(resultado.Carregadores).PotenciaAlocadaKw);
    }
}
