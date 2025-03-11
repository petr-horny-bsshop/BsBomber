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
    /// Jakmile je hodnota 0, tak v příští iteraci bomba exploduje.
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
    public void Detonate(Board board)
    {
        // vzdálenost exploze na každou stranu
        const int explosionRange = 5;
        
        // Exploze na pravou stranu
        for (var x = Position.X; x < Position.X + explosionRange; x++)
        {
            // Je tam překážka, za kterou se exploze nedostane
            if (board.Obstacles.Any(o => o.X == x && o.Y == Position.Y)) break;
            
            AddFire(x, Position.Y);
        }

        // Exploze na levou stranu
        for (var x = Position.X; x > Position.X - explosionRange; x--)
        {
            // Je tam překážka, za kterou se exploze nedostane
            if (board.Obstacles.Any(o => o.X == x && o.Y == Position.Y)) break;

            AddFire(x, Position.Y);
        }

        // Exploze nahoru
        for (var y = Position.Y; y < Position.Y + explosionRange; y++)
        {
            // Je tam překážka, za kterou se exploze nedostane
            if (board.Obstacles.Any(o => o.X == Position.X && o.Y == y)) break;
            AddFire(Position.X, y);
        }

        // Exploze dolů
        for (var y = Position.Y; y > Position.Y - explosionRange; y--)
        {
            // Je tam překážka, za kterou se exploze nedostane
            if (board.Obstacles.Any(o => o.X == Position.X && o.Y == y)) break;
            AddFire(Position.X, y);
        }

        return;    

        void AddFire(int x, int y)
        {
            var fire = new Fire
            {
                BomberId = BomberId,
                Position = new Coordinate(x, y)
            };
            board.Fires.Add(fire);
        }
    }
}