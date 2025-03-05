using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Informace o hráči.
/// </summary>
public record BomberDto
{
    /// <summary>
    /// Jedinečný identifikátor hráče v rámci hry.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Název hráče.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Url adresa hráče.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Pozice hráče.
    /// </summary>
    [JsonPropertyName("position")]
    public required CoordinateDto Position { get; init; }

    /// <summary>
    /// Energie hráče.
    /// Každým krokem se energie snižuje o 1.
    /// </summary>
    [JsonPropertyName("energy")]
    public required int Energy { get; init; }

    /// <summary>
    /// Zda je hráč naživu.
    /// </summary>
    [JsonPropertyName("alive")]
    public required bool Alive { get; init; }

    /// <summary>
    /// Barva hráče.
    /// </summary>
    [JsonPropertyName("color")]
    public required string Color { get; init; }

    /// <summary>
    /// Průměrná latence hráče.
    /// </summary>
    [JsonPropertyName("latency")]
    public required int Latency { get; init; }

    /// <summary>
    /// Důvod smrti hráče.
    /// </summary>
    [JsonPropertyName("deathCause")]
    public required string? DeathCause { get; init; }

    /// <summary>
    /// Iterace, ve které hráč zemřel.
    /// </summary>
    [JsonPropertyName("deathIteration")]
    public required int? DeathIteration { get; init; }
}