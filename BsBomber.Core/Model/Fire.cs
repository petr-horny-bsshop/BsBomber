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
    /// Vrátí DTO ohně.
    /// </summary>
    public FireDto GetDto() => new FireDto
    {
        Position = Position.GetDto(),
        BomberId = BomberId
    };
}