using System.ComponentModel.DataAnnotations;

namespace Quorum.Api.Contratos;

public class CarregadorEntrada
{
    [Required(ErrorMessage = "Informe o nome do carregador.")]
    public required string Nome { get; init; }

    [Range(0.0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "A potência máxima deve ser maior que zero.")]
    public required decimal PotenciaMaximaKw { get; init; }

    public required bool Disponivel { get; init; }
}

public record CarregadorResposta(int Id, int UnidadeConsumidoraId, string Nome, decimal PotenciaMaximaKw, bool Disponivel);
