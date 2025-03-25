using BsBomber.Contracts;
using System.Text.Json.Serialization;

namespace BsBomber.Core.Model;

/// <summary>
/// Informace o bombě.
/// </summary>
public record Bomb
{
    /// <summary>
    /// Pozice bomby.
    /// </summary>
    public required Coordinate Position { get; init; }

    /// <summary>
    /// Počet iterací do výbuchu bomby.
    /// Jakmile je hodnota 1, tak v příští iteraci bomba exploduje.
    /// </summary>
    public required int Timer { get; set; }

    /// <summary>
    /// Identifikátor hráče, který bombu položil.
    /// </summary>
    public required string BomberId { get; init; }

    /// <summary>
    /// Vrátí DTO bomby.
    /// </summary>
    public BombDto GetDto()
    {
        return new BombDto
        {
            Position = Position.GetDto(),
            Timer = Timer,
            BomberId = BomberId
        };
    }

    /// <summary>
    /// Odpálí bombu.
    /// </summary>
    public void Detonate(Game game)
    {
        var board = game.Board;
        if (!board.Bombs.Remove(this))
        {
            return;
        }

        // vzdálenost exploze na každou stranu
        const int explosionRange = int.MaxValue / 2;

        // Exploze na pravou stranu
        for (var x = Position.X; x < Position.X + explosionRange; x++)
        {
            if (!game.TryAddFire(x, Position.Y, BomberId)) break;
        }

        // Exploze na levou stranu
        for (var x = Position.X; x > Position.X - explosionRange; x--)
        {
            if (!game.TryAddFire(x, Position.Y, BomberId)) break;
        }

        // Exploze nahoru
        for (var y = Position.Y; y < Position.Y + explosionRange; y++)
        {
            if (!game.TryAddFire(Position.X, y, BomberId)) break;
        }

        // Exploze dolů
        for (var y = Position.Y; y > Position.Y - explosionRange; y--)
        {
            if (!game.TryAddFire(Position.X, y, BomberId)) break;
        }
    }
}