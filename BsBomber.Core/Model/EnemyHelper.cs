using BsBomber.Contracts;

namespace BsBomber.Core.Model;

static class EnemyHelper
{
    public static Coordinate Move(Game game, Coordinate enemyPosition)
    {
        var bombers = game.Board.AliveBombers.Select(b => b.Position).ToArray();
        var obstacles = new HashSet<Coordinate>(game.Board.Mines.Concat(game.Board.Bombs.Select(o => o.Position)));

        // Najdi nejbližší jídlo pomocí BFS
        var nextMove = FindBestMove(enemyPosition, bombers, obstacles, game.Board.Width, game.Board.Height);

        var newPosition = enemyPosition.Move(nextMove);
        return newPosition;
    }

    private static BomberAction FindBestMove(Coordinate start, IReadOnlyCollection<Coordinate> bombers, HashSet<Coordinate> obstacles, int width, int height)
    {
        var directions = new (BomberAction Action, int DX, int DY)[]
        {
            (BomberAction.GoRight, 1, 0),
            (BomberAction.GoLeft, -1, 0),
            (BomberAction.GoUp, 0, 1),
            (BomberAction.GoDown, 0, -1)
        };

        var queue = new Queue<(int X, int Y, List<BomberAction> Path)>();
        var visited = new HashSet<Coordinate>();

        queue.Enqueue((start.X, start.Y, new List<BomberAction>()));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (x, y, path) = queue.Dequeue();

            // Pokud jsme našli jídlo, vrať první krok cesty
            if (bombers.Any(f => f.X == x && f.Y == y) && path.Count > 0)
            {
                return path.First();
            }

            // Přidání sousedů do fronty
            foreach (var (action, dx, dy) in directions)
            {
                int newX = x + dx, newY = y + dy;
                var newCoord = new Coordinate(newX, newY);

                if (newX >= 0 && newX < width && newY >= 0 && newY < height && !obstacles.Contains(newCoord) && !visited.Contains(newCoord))
                {
                    var newPath = new List<BomberAction>(path) { action };
                    queue.Enqueue((newX, newY, newPath));
                    visited.Add(newCoord);
                }
            }
        }

        return BomberAction.None; // Pokud nelze najít cestu k jídlu, nic nedělej
    }
}