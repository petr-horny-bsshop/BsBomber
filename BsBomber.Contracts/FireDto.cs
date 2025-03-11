using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Informace o ohni.
/// </summary>
public record FireDto
{
    /// <summary>
    /// Pozice ohně.
    /// </summary>
    [JsonPropertyName("position")]
    public required CoordinateDto Position { get; init; }

    /// <summary>
    /// Identifikátor hráče, který bombu položil.
    /// </summary>
    [JsonPropertyName("bomberId")]
    public required string BomberId { get; init; }
}