using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Odpověď hráče v aktuální iteraci.
/// </summary>
public record ResponseDto
{
    /// <summary>
    /// Akce, kterou chce hráč provést.
    /// </summary>
    [JsonPropertyName("bomberAction")]
    public BomberAction BomberAction { get; set; }

    /// <summary>
    /// Argument akce.
    /// V případě položení bomby je to doba do výbuchu bomby.
    /// </summary>
    [JsonPropertyName("argument")]
    public int? Argument { get; set; }
}