using BsBomber.Contracts;
using BsBomber.Core.Model;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously


/// <inheritdoc />
public class AiBomberEngine : IBomberEngine
{
    private const int SafeDistance = 2; // Bezpečná vzdálenost od exploze
    private const int BombTimer = 3; // Časovač pro výbuch bomby
    private const int BombRange = 6; // Rozsah bomby

    /// <inheritdoc />
    public Task InitAsync(GameDto gameDto, CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // Není třeba inicializace
    }

    /// <inheritdoc />
    public async Task<ResponseDto> MoveAsync(GameDto game, CancellationToken cancellationToken)
    {
        var player = game.You;
        var board = game.Board;

        // Zjistíme, kde se nacházejí ostatní hráči
        var otherPlayers = game.Board.Bombers.Where(b => b.Id != player.Id && b.Alive).ToList();

        // Zjistíme, kde je jídlo
        var food = board.Mines.ToList();

        // Zjistíme pozice ohně (místo, kde hrozí výbuch)
        var fires = board.Fires.Select(f => f.Position).ToList();

        // Pokud je jídlo v dosahu bomby, položme bombu místo chůze
        var foodInBombRange = food.Where(f => IsInBombRange(player.Position, f)).ToList();
        if (foodInBombRange.Any())
        {
            return new ResponseDto { BomberAction = BomberAction.PutBomb, Argument = BombTimer };
        }

        // Zkusíme najít nejlepší pohyb k jídlu, pokud není možnost sbírat potravu pomocí bomby
        var bestMove = ChooseBestMove(player, food, fires, otherPlayers);

        // Pokud máme jít do určitého směru, vrátíme akci
        if (bestMove != BomberAction.None)
        {
            return new ResponseDto { BomberAction = bestMove };
        }

        // Pokud je dostatečná bezpečnostní vzdálenost od všech soupeřů a nejsou ohně poblíž, můžeme položit bombu
        if (CanPlaceBomb(player, fires))
        {
            return new ResponseDto { BomberAction = BomberAction.PutBomb, Argument = BombTimer };
        }

        // Pokud ne, jednoduše nic neuděláme
        return new ResponseDto { BomberAction = BomberAction.None };
    }

    private bool IsInBombRange(CoordinateDto playerPosition, CoordinateDto foodPosition)
    {
        var distance = GetManhattanDistance(playerPosition, foodPosition);
        return distance <= BombRange;
    }

    private BomberAction ChooseBestMove(BomberDto player, List<CoordinateDto> food, List<CoordinateDto> fires, List<BomberDto> otherPlayers)
    {
        // Pokud je jídlo v okolí, snažíme se k němu přiblížit
        var closestFood = food.OrderBy(f => GetManhattanDistance(player.Position, f)).FirstOrDefault();
        if (closestFood != null && !fires.Contains(closestFood))
        {
            var pathToFood = FindPath(player.Position, closestFood, fires);
            if (pathToFood.Any())
            {
                return GetDirectionToMove(player.Position, pathToFood.First());
            }
        }

        // Pokud je hráč příliš blízko ohně, je třeba utéct
        var fireNearby = fires.FirstOrDefault(f => GetManhattanDistance(player.Position, f) < SafeDistance);
        if (fireNearby != null)
        {
            var escapePath = EscapeFromFire(player, fires);
            if (escapePath.Any())
            {
                return GetDirectionToMove(player.Position, escapePath.First());
            }
        }

        // Pokud jsou poblíž ostatní hráči, může se rozhodnout podle jejich polohy
        foreach (var enemy in otherPlayers)
        {
            var distToEnemy = GetManhattanDistance(player.Position, enemy.Position);
            if (distToEnemy < SafeDistance)
            {
                var escapePath = EscapeFromEnemy(player, enemy);
                if (escapePath.Any())
                {
                    return GetDirectionToMove(player.Position, escapePath.First());
                }
            }
        }

        // Pokud není žádná jiná volba, zůstaneme na místě
        return BomberAction.None;
    }

    private int GetManhattanDistance(CoordinateDto a, CoordinateDto b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private List<CoordinateDto> FindPath(CoordinateDto start, CoordinateDto goal, List<CoordinateDto> obstacles)
    {
        // A* algoritmus pro hledání cesty
        var openSet = new List<CoordinateDto> { start };
        var cameFrom = new Dictionary<CoordinateDto, CoordinateDto>();
        var gScore = new Dictionary<CoordinateDto, int> { { start, 0 } };
        var fScore = new Dictionary<CoordinateDto, int> { { start, GetManhattanDistance(start, goal) } };

        while (openSet.Any())
        {
            // Najdeme bod s nejnižším fScore
            var current = openSet.OrderBy(c => fScore.GetValueOrDefault(c, int.MaxValue)).First();
            if (current.Equals(goal))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach (var neighbor in GetNeighbors(current))
            {
                // Ignorujeme překážky a ohně
                if (obstacles.Contains(neighbor))
                {
                    continue;
                }

                var tentativeGScore = gScore.GetValueOrDefault(current, int.MaxValue) + 1;
                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + GetManhattanDistance(neighbor, goal);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<CoordinateDto>(); // Pokud není cesta
    }

    private List<CoordinateDto> ReconstructPath(Dictionary<CoordinateDto, CoordinateDto> cameFrom, CoordinateDto current)
    {
        var path = new List<CoordinateDto> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private List<CoordinateDto> GetNeighbors(CoordinateDto position)
    {
        var directions = new List<CoordinateDto>
        {
            new CoordinateDto(position.X + 1, position.Y), // Right
            new CoordinateDto(position.X - 1, position.Y), // Left
            new CoordinateDto(position.X, position.Y + 1), // Up
            new CoordinateDto(position.X, position.Y - 1), // Down
        };
        return directions;
    }

    private BomberAction GetDirectionToMove(CoordinateDto playerPos, CoordinateDto targetPos)
    {
        if (targetPos.X > playerPos.X)
            return BomberAction.GoRight;
        if (targetPos.X < playerPos.X)
            return BomberAction.GoLeft;
        if (targetPos.Y > playerPos.Y)
            return BomberAction.GoUp;
        return BomberAction.GoDown;
    }

    private List<CoordinateDto> EscapeFromFire(BomberDto player, List<CoordinateDto> fires)
    {
        // Zkusíme se vzdálit od ohně, takže zvolíme směr, který není směrem k ohni
        var safeDirections = new List<BomberAction> { BomberAction.GoUp, BomberAction.GoDown, BomberAction.GoLeft, BomberAction.GoRight };
        var path = new List<CoordinateDto>();

        // Zkoušíme najít bezpečné místo
        return path; // Pokud máme bezpečné místo, vrátíme cestu
    }

    private List<CoordinateDto> EscapeFromEnemy(BomberDto player, BomberDto enemy)
    {
        // Snažíme se utéct před soupeřem
        return new List<CoordinateDto>(); // Pokud máme cestu na útěk, vrátíme ji
    }

    private bool CanPlaceBomb(BomberDto player, List<CoordinateDto> fires)
    {
        // Zkontrolujeme, zda je v bezpečné vzdálenosti od ohně
        return fires.All(f => GetManhattanDistance(player.Position, f) > SafeDistance);
    }
}
