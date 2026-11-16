using System.ComponentModel.DataAnnotations;

namespace Quorum.Api.Contratos;

public class UnidadeEntrada
{
    [Required(ErrorMessage = "Informe o nome da unidade.")]
    public required string Nome { get; init; }

    [Range(0.0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "O limite de demanda deve ser maior que zero.")]
    public required decimal LimiteDemandaKw { get; init; }
}

public record UnidadeResposta(int Id, string Nome, decimal LimiteDemandaKw);
