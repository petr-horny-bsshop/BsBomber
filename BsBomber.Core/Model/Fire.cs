using BsBomber.Contracts;

namespace BsBomber.Core.Model;

/// <summary>
/// Informace o ohni.
/// </summary>
public record Fire
{
    /// <summary>
    /// Pozice ohně.
    /// </summary>
    public required Coordinate Position { get; init; }

    /// <summary>
    /// Identifikátor hráče, který bombu položil.
    /// </summary>
    public required string BomberId { get; init; }

    /// <summary>
    /// Intenzita ohně. S každou iterací se snižuje o 1. Pokud dosáhne 0, ohně nebude zraňovat, pouze zůstane chvíli vizuál (záporné hodnoty).
    /// </summary>
    public required int Intensity { get; set; }

    /// <summary>
    /// Vrátí DTO ohně.
    /// </summary>
    public FireDto GetDto() => new FireDto
    {
        Position = Position.GetDto(),
        BomberId = BomberId,
        Intensity = Intensity
    };
}