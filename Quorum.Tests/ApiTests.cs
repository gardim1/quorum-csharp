using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Quorum.Tests;

public class ApiTests : IAsyncDisposable
{
    private readonly string caminhoBanco = Path.Combine(Path.GetTempPath(), $"quorum-{Guid.NewGuid():N}.db");
    private static CancellationToken Cancelamento => TestContext.Current.CancellationToken;

    [Fact]
    public async Task CarregadoresDoExemploRecebemOnzeESeteKw()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();

        var plano = await cliente.GetFromJsonAsync<JsonElement>("/api/unidades/1/plano-carregamento", Cancelamento);

        Assert.Equal(32m, plano.GetProperty("demandaBaseKw").GetDecimal());
        Assert.Equal(18m, plano.GetProperty("potenciaAlocadaKw").GetDecimal());
        Assert.Equal(11m, plano.GetProperty("carregadores")[0].GetProperty("potenciaAlocadaKw").GetDecimal());
        Assert.Equal(7m, plano.GetProperty("carregadores")[1].GetProperty("potenciaAlocadaKw").GetDecimal());
    }

    [Theory]
    [InlineData(1, "/medicoes?inicio=2026-11-17T00:00:00Z&fim=2026-11-16T00:00:00Z", HttpStatusCode.BadRequest)]
    [InlineData(999, "/medicoes", HttpStatusCode.NotFound)]
    [InlineData(999, "/carregadores", HttpStatusCode.NotFound)]
    [InlineData(999, "/plano-carregamento", HttpStatusCode.NotFound)]
    public async Task ConsultasRetornamCodigoAdequadoQuandoInvalidas(int id, string caminho, HttpStatusCode codigo)
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();

        using var resposta = await cliente.GetAsync($"/api/unidades/{id}{caminho}", Cancelamento);

        Assert.Equal(codigo, resposta.StatusCode);
    }

    [Fact]
    public async Task UnidadeSemMedicaoNaoTemPlano()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();
        var id = await CriarUnidadeAsync(cliente);

        using var resposta = await cliente.GetAsync($"/api/unidades/{id}/plano-carregamento", Cancelamento);

        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task ApiRejeitaPotenciasInvalidas()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();

        using var unidade = await cliente.PostAsJsonAsync("/api/unidades", new { nome = "Unidade", limiteDemandaKw = 0 }, Cancelamento);
        using var carregador = await cliente.PostAsJsonAsync("/api/unidades/1/carregadores", new { nome = "Carregador", potenciaMaximaKw = -1, disponivel = true }, Cancelamento);
        using var medicao = await cliente.PostAsJsonAsync("/api/unidades/1/medicoes", new { demandaBaseKw = -1 }, Cancelamento);

        Assert.Equal(HttpStatusCode.BadRequest, unidade.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, carregador.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, medicao.StatusCode);
    }

    [Fact]
    public async Task FiltroInclusivoCalculaEstatisticasEMantemHorarioUtc()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();
        var id = await CriarUnidadeAsync(cliente);

        await RegistrarMedicaoAsync(cliente, id, 20, "2026-11-14T09:00:00-03:00");
        await RegistrarMedicaoAsync(cliente, id, 50, "2026-11-14T13:00:00Z");
        await RegistrarMedicaoAsync(cliente, id, 70, "2026-11-15T09:00:00Z");

        var resumo = await cliente.GetFromJsonAsync<JsonElement>(
            $"/api/unidades/{id}/medicoes?inicio=2026-11-14T12:00:00Z&fim=2026-11-14T13:00:00Z", Cancelamento);

        Assert.Equal(2, resumo.GetProperty("quantidade").GetInt32());
        Assert.Equal(35m, resumo.GetProperty("mediaKw").GetDecimal());
        Assert.Equal(50m, resumo.GetProperty("maximaKw").GetDecimal());
        Assert.Equal("2026-11-14T12:00:00Z", resumo.GetProperty("medicoes")[1].GetProperty("registradaEm").GetString());
    }

    [Fact]
    public async Task PlanoLeAMedicaoMaisRecenteDepoisDeReabrirBanco()
    {
        int id;

        await using (var api = AbrirApi())
        {
            using var cliente = api.CreateClient();
            id = await CriarUnidadeAsync(cliente);
            await RegistrarMedicaoAsync(cliente, id, 33, "2026-11-16T12:00:00Z");
            await RegistrarMedicaoAsync(cliente, id, 10, "2026-11-15T12:00:00Z");
        }

        await using var apiReaberta = AbrirApi();
        using var outroCliente = apiReaberta.CreateClient();
        var plano = await outroCliente.GetFromJsonAsync<JsonElement>($"/api/unidades/{id}/plano-carregamento", Cancelamento);

        Assert.True(File.Exists(caminhoBanco));
        Assert.Equal(33m, plano.GetProperty("demandaBaseKw").GetDecimal());
        Assert.Equal("2026-11-16T12:00:00Z", plano.GetProperty("registradaEm").GetString());
    }

    [Fact]
    public async Task EdicaoERemocaoDeCarregadorAlteramOPlano()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();
        var id = await CriarUnidadeAsync(cliente);
        await RegistrarMedicaoAsync(cliente, id, 33, "2026-11-16T12:00:00Z");

        using var criado = await cliente.PostAsJsonAsync($"/api/unidades/{id}/carregadores",
            new { nome = "Carregador A", potenciaMaximaKw = 22, disponivel = false }, Cancelamento);
        Assert.Equal(HttpStatusCode.Created, criado.StatusCode);
        Assert.NotNull(criado.Headers.Location);
        using var editado = await cliente.PutAsJsonAsync(criado.Headers.Location,
            new { nome = "Carregador A", potenciaMaximaKw = 22, disponivel = true }, Cancelamento);
        Assert.Equal(HttpStatusCode.NoContent, editado.StatusCode);

        var plano = await cliente.GetFromJsonAsync<JsonElement>($"/api/unidades/{id}/plano-carregamento", Cancelamento);
        Assert.Equal(17m, plano.GetProperty("potenciaAlocadaKw").GetDecimal());

        using var excluido = await cliente.DeleteAsync(criado.Headers.Location, Cancelamento);
        Assert.Equal(HttpStatusCode.NoContent, excluido.StatusCode);
        using var carregador = await cliente.GetAsync(criado.Headers.Location, Cancelamento);
        Assert.Equal(HttpStatusCode.NotFound, carregador.StatusCode);
    }

    [Fact]
    public async Task ExclusaoDeUnidadeRemoveMedicoesECarregadoresVinculados()
    {
        await using var api = AbrirApi();
        using var cliente = api.CreateClient();

        using var exclusao = await cliente.DeleteAsync("/api/unidades/1", Cancelamento);
        Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);

        await using var segundaApi = AbrirApi();
        using var outroCliente = segundaApi.CreateClient();
        var unidades = await outroCliente.GetFromJsonAsync<JsonElement>("/api/unidades", Cancelamento);
        Assert.Empty(unidades.EnumerateArray());
        using var carregadores = await outroCliente.GetAsync("/api/unidades/1/carregadores", Cancelamento);
        Assert.Equal(HttpStatusCode.NotFound, carregadores.StatusCode);
    }

    private WebApplicationFactory<Program> AbrirApi()
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Quorum", $"Data Source={caminhoBanco};Pooling=False"));
    }

    private static async Task<int> CriarUnidadeAsync(HttpClient cliente)
    {
        using var resposta = await cliente.PostAsJsonAsync("/api/unidades",
            new { nome = "Unidade de teste", limiteDemandaKw = 50 }, Cancelamento);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var unidade = await resposta.Content.ReadFromJsonAsync<JsonElement>(Cancelamento);
        return unidade.GetProperty("id").GetInt32();
    }

    private static async Task RegistrarMedicaoAsync(HttpClient cliente, int id, decimal demanda, string data)
    {
        using var resposta = await cliente.PostAsJsonAsync($"/api/unidades/{id}/medicoes",
            new { demandaBaseKw = demanda, registradaEm = data }, Cancelamento);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }

    public ValueTask DisposeAsync()
    {
        File.Delete(caminhoBanco);
        return ValueTask.CompletedTask;
    }
}
