using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quorum.Api.Dados;
using Quorum.Api.Servicos;

var builder = WebApplication.CreateBuilder(args);
var caminhoBanco = Path.Combine(builder.Environment.ContentRootPath, "quorum.db");

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(
        new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Verifique os campos da solicitação."
        });
});

builder.Services.AddDbContext<QuorumDbContext>((servicos, options) =>
{
    var configuracao = servicos.GetRequiredService<IConfiguration>();
    var conexao = new SqliteConnectionStringBuilder(configuracao.GetConnectionString("Quorum") ?? "")
    {
        ForeignKeys = true
    };

    if (string.IsNullOrWhiteSpace(conexao.DataSource))
    {
        conexao.DataSource = caminhoBanco;
    }

    options.UseSqlite(conexao.ToString());
});

builder.Services.AddScoped<IControleDemandaService, ControleDemandaService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "Quorum | API de gestão de demanda",
    Version = "v1",
    Description = "Cadastro de unidades e carregadores, medições de demanda e simulação de alocação de potência."
}));

var app = builder.Build();

await using (var escopo = app.Services.CreateAsyncScope())
{
    var banco = escopo.ServiceProvider.GetRequiredService<QuorumDbContext>();
    await banco.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
await app.RunAsync();

public partial class Program { }
