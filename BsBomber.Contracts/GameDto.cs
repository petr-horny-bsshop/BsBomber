using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Obsahuje všechny informace o aktuálním stavu hry.
/// </summary>
public record GameDto
{
    /// <summary>
    /// Obsahuje informace o hrací ploše.
    /// </summary>
    [JsonPropertyName("board")]
    public required BoardDto Board { get; init; }

    /// <summary>
    /// Obsahuje informaci o hráči.
    /// Ideální by bylo mít objekt GameDto rozdělen na dvě části jednu bez vlastnosti You a druhou s vlastností You. Tato varianta je však kompromis mezi jednoduchostí a přehledností.
    /// </summary>
    [JsonPropertyName("you")]
    public required BomberDto You { get; init; }

    /// <summary>
    /// Aktuální iterace hry.
    /// </summary>
    [JsonPropertyName("iteration")]
    public required int Iteration { get; init; }
}