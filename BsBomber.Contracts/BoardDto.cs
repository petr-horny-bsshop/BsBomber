using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Informace o hrací ploše.
/// </summary>
public record BoardDto
{
    /// <summary>
    /// Výška hrací plochy.
    /// </summary>
    [JsonPropertyName("height")]
    public required int Height { get; init; }

    /// <summary>
    /// Šířka hrací plochy.
    /// </summary>
    [JsonPropertyName("width")]
    public required int Width { get; init; }

    /// <summary>
    /// Informace o políčkách s jídlem.
    /// </summary>
    [JsonPropertyName("food")]
    public required CoordinateDto[] Food { get; init; }

    /// <summary>
    /// Informace o políčkách s překážkami.
    /// </summary>
    [JsonPropertyName("obstacles")]
    public required CoordinateDto[] Obstacles { get; init; }
    
    /// <summary>
    /// Informace o políčkách s ohněm (po detonaci bomby.).
    /// </summary>
    [JsonPropertyName("fire")]
    public required FireDto[] Fires { get; init; }

    /// <summary>
    /// Informace o bombách.
    /// </summary>
    [JsonPropertyName("bombs")]
    public required BombDto[] Bombs { get; init; }

    /// <summary>
    /// Informace o všech hráčích na hrací ploše.
    /// Jsou zde i hráči, kteří již zemřeli.
    /// Je zde i aktuální hráč.
    /// </summary>
    [JsonPropertyName("bombers")]
    public required BomberDto[] Bombers { get; init; }
}