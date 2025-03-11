using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Informace o bombě.
/// </summary>
public record BombDto
{
    /// <summary>
    /// Pozice bomby.
    /// </summary>
    [JsonPropertyName("position")]
    public required CoordinateDto Position { get; init; }

    /// <summary>
    /// Počet iterací do výbuchu bomby.
    /// Jakmile je hodnota 0, tak v příští iteraci bomba exploduje.
    /// </summary>
    [JsonPropertyName("timer")]
    public required int Timer { get; init; }

    /// <summary>
    /// Identifikátor hráče, který bombu položil.
    /// </summary>
    [JsonPropertyName("bomberId")]
    public required string BomberId { get; init; }
}