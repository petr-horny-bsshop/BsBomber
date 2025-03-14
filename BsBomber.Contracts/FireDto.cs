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

    /// <summary>
    /// Intenzita ohně. S každou iterací se snižuje o 1. Pokud dosáhne 0, oheň nebude zraňovat, pouze zůstane chvíli vizuál (záporné hodnoty).
    /// </summary>
    [JsonPropertyName("intensity")]
    public required int Intensity { get; init; }

}