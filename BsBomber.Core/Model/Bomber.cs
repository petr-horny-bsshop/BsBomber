using System.Diagnostics;
using BsBomber.Contracts;
using BsBomber.Core.BomberEngines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BsBomber.Core.Model;

/// <summary>
/// Definice hráče.
/// </summary>
public class Bomber
{
    private readonly IBomberEngine _engine;
    private TimeSpan _timeSpent;
    private int _requests;

    /// <summary>
    /// Jedinečný identifikátor instance hráče.
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Název hráče.
    /// </summary>
    public string Name { get; private init; }
    
    /// <summary>
    /// Url adresa hráče (pokud se jedná o vzdáleného hráče).
    /// </summary>
    public string? Url => (_engine as RemoteBomberEngine)?.Url;

    /// <summary>
    /// Zda je hráč živý.
    /// </summary>
    public bool Alive { get; private set; } = true;

    /// <summary>
    /// Skóre.
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// Příčina smrti hráče.
    /// </summary>
    public string? DeathCause { get; private set; }

    /// <summary>
    /// Iterace, ve které došlo ke smrti hráče.
    /// </summary>
    public int? DeathIteration { get; private set; }

    /// <summary>
    /// Průměrná latence při komunikaci s hráčem.
    /// </summary>
    public TimeSpan Latency => _requests > 0 ? _timeSpent / _requests : TimeSpan.Zero;

    /// <summary>
    /// Souřadnice hráče.
    /// </summary>
    public Coordinate Position { get; private set; } = new Coordinate(0, 0);

    /// <summary>
    /// Barva hráče.
    /// Jedná se o html kód barvy.
    /// </summary>
    public string Color { get; private set; }

    /// <summary>
    /// Vytvoří novou instanci hráče.
    /// </summary>
    /// <param name="positionPosition">Pozice hlavy.</param>
    /// <param name="engine">Engine, který ovládá hráče.</param>
    /// <param name="name">Název hráče.</param>
    /// <param name="color">Html barva hráče.</param>
    public Bomber(Coordinate positionPosition, IBomberEngine engine, string name, string color)
    {
        _engine = engine;
        Position = positionPosition;
        Color = color;
        Name = name;
    }

    /// <summary>
    /// Zabije hráče.
    /// </summary>
    /// <param name="cause">Příčina smrti hráče.</param>
    /// <param name="iteration">Iterace, ve které došlo ke smrti hráče.</param>
    public void Kill(string cause, int iteration)
    {
        if (!Alive) return;
        
        Alive = false;
        DeathCause = cause;
        DeathIteration = iteration;
    }

    /// <summary>
    /// Provede inicializaci hráče.
    /// </summary>
    public async Task InitAsync(Game game, CancellationToken cancellationToken)
    {
        var gameDto = game.GetDto(this);
        await _engine.InitAsync(gameDto, cancellationToken);
    }

    /// <summary>
    /// Provede tah hráče.
    /// </summary>
    public async Task MoveAsync(Game game, CancellationToken cancellationToken)
    {
        var gameDto = game.GetDto(this);

        var sw = Stopwatch.StartNew();
        
        var response = await _engine.MoveAsync(gameDto, cancellationToken);
        
        AddResponseTime(sw.Elapsed);

        //var json = JsonSerializer.Serialize(response, new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        lock (game._iterationLock)
        {
            // Jedná se o pohyb
            if (response.BomberAction.IsMovement())
            {
                var newPosition = Position.Move(response.BomberAction);

                // Nejde se na novou pozici, protože je mimo hrací plochu
                if (!game.IsOnBoard(newPosition)) return;

                // Na nové pozici je jiný hráč, takže se nikam nejde
                if (game.Board.AliveBombers.Any(b => b.Position == newPosition)) return;

                Position = newPosition;
            }
            else if (response.BomberAction.IsBombPlacing())
            {
                if (game.Board.Bombs.Any(b => b.Position == Position)) return;

                var timer = response.Argument.GetValueOrDefault(3);
                if (timer is < 2 or > 3) return;
                var bomb = new Bomb
                           {
                               BomberId = Id,
                               Position = Position,
                               Timer = timer + 1 // přidáme +1 protože ještě v této iteraci u bomby proběhne ponížení timeru
                };
                game.Board.Bombs.AddLast(bomb);
            }
        }
    }

    /// <summary>
    /// Simuluje tah hráče, tzn. dotáže se hráče na jeho další směr, ale pozici hráče ve hře neaktualizuje.
    /// Určeno pro ladění hráče.
    /// </summary>
    public async Task<BomberAction> SimulateMoveAsync(GameDto game, CancellationToken cancellationToken)
    {
        var response = await _engine.MoveAsync(game, cancellationToken);
        return response.BomberAction;
    }

    /// <summary>
    /// Přidá hráči skóre za zničenou minu.
    /// </summary>
    public void AddScore(int count = 1)
    {
        Score += count;
    }

    /// <summary>
    /// Vrátí DTO hráče.
    /// </summary>
    public BomberDto GetDto()
    {
        var dto = new BomberDto
        {
            Id = Id,
            Name = Name,
            Url = Url,
            Position = Position.GetDto(),
            Score = Score,
            Color = Color,
            Alive = Alive,
            DeathCause = DeathCause,
            DeathIteration = DeathIteration,
            Latency = (int)Latency.TotalMilliseconds
        };
        return dto;
    }

    /// <summary>
    /// Přidá čas do průměrné latence.
    /// </summary>
    public void AddResponseTime(TimeSpan duration)
    {
        _timeSpent += duration;
        _requests++;
    }
}