using System.ComponentModel.DataAnnotations;

namespace Quorum.Api.Contratos;

public class MedicaoEntrada
{
    [Range(0.0, double.MaxValue, ErrorMessage = "A demanda base deve ser maior ou igual a zero.")]
    public required decimal DemandaBaseKw { get; init; }

    public DateTimeOffset? RegistradaEm { get; init; }
}

public record MedicaoResposta(int Id, int UnidadeConsumidoraId, decimal DemandaBaseKw, DateTime RegistradaEm);

public record ResumoMedicoes(int Quantidade, double? MediaKw, decimal? MaximaKw, IReadOnlyList<MedicaoResposta> Medicoes);
