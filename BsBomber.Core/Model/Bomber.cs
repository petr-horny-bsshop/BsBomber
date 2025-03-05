using BsBomber.Contracts;
using BsBomber.Core.BomberEngines;

namespace BsBomber.Core.Model;

/// <summary>
/// Definice hráče.
/// </summary>
public class Bomber
{
    private readonly IBomberEngine _engine;
    private readonly Queue<Coordinate> _body = new Queue<Coordinate>();
    private int _foodToDigest = 0;
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
    /// Energie hráče.
    /// </summary>
    public int Energy { get; private set; }

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
    /// <param name="energy">Počáteční energie.</param>
    /// <param name="engine">Engine, který ovládá hráče.</param>
    /// <param name="name">Název hráče.</param>
    /// <param name="color">Html barva hráče.</param>
    public Bomber(Coordinate positionPosition, int energy, IBomberEngine engine, string name, string color)
    {
        _engine = engine;
        Position = positionPosition;
        Color = color;
        Name = name;
        Energy = energy;
    }

    /// <summary>
    /// Zabije hráče.
    /// </summary>
    /// <param name="cause">Příčina smrti hráče.</param>
    /// <param name="iteration">Iterace, ve které došlo ke smrti hráče.</param>
    public void Kill(string cause, int iteration)
    {
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
        var response = await _engine.MoveAsync(gameDto, cancellationToken);

        var hasBody = _body.Count > 0;

        if (hasBody)
        {
            if (_foodToDigest > 0) _foodToDigest--;
            else _body.Dequeue();
            _body.Enqueue(Position);
        }
        else
        {
            if (_foodToDigest > 0)
            {
                _foodToDigest--;
                _body.Enqueue(Position);
            }
        }

        Position = Position.Move(response.BomberAction);

        Energy--;
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
    /// Informuje hráče, že snědl jídlo se zadanou energií.
    /// </summary>
    /// <param name="count">Počet jídla, který hráč snědl.</param>
    /// <param name="energy">Celková energie získaná snědením daného počtu jídla.</param>
    public void Eat(int count, int energy)
    {
        _foodToDigest += count;
        Energy += energy;
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
            Energy = Energy,
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