using System.Diagnostics;
using BsBomber.Contracts;
using BsBomber.Core.BomberEngines;

namespace BsBomber.Core.Model;

/// <summary>
/// Definuje právě jednu hru.
/// </summary>
public class Game
{
    private readonly GameSettings _settings;
    private readonly List<GameDto> _iterationHistory = new();
    private readonly IReadOnlyList<string> _bomberColors = new []
        {
        "#ff0000",
        "#00ff00",
        "#0000ff",
        "#e3c700",
        "#ff00ff",
        "#00ffff",
        "#000000",
        "#ff6000"
        };
    internal readonly Lock _iterationLock = new();

    /// <summary>
    /// Hrací plocha.
    /// </summary>
    public Board Board { get; private set; }

    /// <summary> ctor. </summary>
    /// <param name="settings">Nastavení hry.</param>
    /// <param name="bombers">Seznam hráčů ve hře.</param>
    public Game(GameSettings settings, IReadOnlyList<BomberDefinition> bombers)
    {
        if (bombers.Count == 0) throw new ArgumentException("Seznam hráčů nesmí být prázdný.", nameof(bombers));

        _settings = settings;
        Board = new Board(settings.BoardWidth, settings.BoardHeight);
        
        // InitObstacles();

        for (var i = 0; i < bombers.Count; i++)
        {
            var bomberDefinition = bombers[i];
            var bomber = new RemoteBomberEngine(bomberDefinition.Url);
            var bomberColor = _bomberColors[i % _bomberColors.Count];
            Board.AddBomber(bomber, bomberDefinition.Name, bomberColor);
        }

        GenerateMines();
    }

    /// <summary>
    /// Aktuální iterace hry.
    /// </summary>
    public int Iteration { get; private set; }

    /// <summary>
    /// Zda byla hra dokončena.
    /// </summary>
    public bool Completed { get; private set; }

    /// <summary>
    /// Text popisující důvod ukončení hry.
    /// </summary>
    public string? CompletedText { get; private set; }

    /// <summary>
    /// Seznam stavů hry pro jednotlivé iterace.
    /// </summary>
    public IReadOnlyList<GameDto> IterationHistory => _iterationHistory;

    /// <summary>
    /// Vrátí DTO hry.
    /// </summary>
    public GameDto GetDto()
    {
        var gameDto = new GameDto
        {
            Board = Board.GetDto(_settings.FireIntensity),
            You = null!,
            Iteration = Iteration
        };
        return gameDto;
    }

    /// <summary>
    /// Vrátí DTO hry pro konkrétního hráče.
    /// </summary>
    public GameDto GetDto(Bomber bomber)
    {
        var gameDto = new GameDto
        {
            You = bomber.GetDto(),
            Board = Board.GetDto(_settings.FireIntensity),
            Iteration = Iteration
        };
        return gameDto;
    }

    /// <summary>
    /// Provede inicializaci hry.
    /// </summary>
    public async Task InitAsync()
    {
        await InitBombersAsync();
        
        var gameDto = GetDto();
        _iterationHistory.Add(gameDto);
    }

    /// <summary>
    /// Provede jednu iteraci hry.
    /// </summary>
    public async Task<bool> MoveAsync()
    {
        if (Completed) return false;

        Iteration++;

        await MoveBombersAsync();

        ProcessFires();
        CheckCollisions();
        DetonateBombs();
        GenerateMines();

        var gameDto = GetDto();
        _iterationHistory.Add(gameDto);

        var anyBomberAlive = Board.AliveBombers.Any();
        if (!anyBomberAlive)
        {
            Completed = true;
            CompletedText = "všichni hráči jsou mrtví";
        }
        else if (Iteration >= _settings.MaximumIterations)
        {
            Completed = true;
            CompletedText = "byl spotřebován vymezený počet iterací";
        }

        return anyBomberAlive;
    }

    private void ProcessFires()
    {
        foreach (var fire in Board.Fires.ToArray())
        {
            fire.Intensity--;
            if (fire.Intensity == -1)
            {
                Board.Fires.Remove(fire);
            }
        }
    }

    private async Task InitBombersAsync()
    {
        var tasks = new List<Task>();
        
        foreach (var bomber in Board.AliveBombers)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.MaximumTimeout ?? 10_000));
            var initTask = bomber.InitAsync(this, cts.Token);
            initTask = initTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    bomber.Kill($"Chyba: {t.Exception?.GetBaseException()?.Message}", Iteration);
                }
                else if (t.IsCanceled)
                {
                    bomber.Kill("Překročen maximální timeout při komunikaci s hráčem.", Iteration);
                }
            });
            tasks.Add(initTask);
        }

        await Task.WhenAll(tasks);
    }

    private async Task MoveBombersAsync()
    {
        var tasks = new List<Task>();
        
        var sw = Stopwatch.StartNew();

        foreach (var bomber in Board.AliveBombers)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.MaximumTimeout ?? 10_000));
            
            var task = bomber.MoveAsync(this, cts.Token);
            
            task = task.ContinueWith(t =>
            {
                bomber.AddResponseTime(sw.Elapsed);

                if (t.IsFaulted)
                {
                    bomber.Kill($"Chyba: {t.Exception?.GetBaseException()?.Message}", Iteration);
                }
                else if (t.IsCanceled)
                {
                    bomber.Kill("Překročen maximální timeout při komunikaci s hráčem.", Iteration);
                }
                else if(Iteration > 3 && bomber.Latency.TotalMilliseconds > _settings.AverageTimeout)
                {
                    bomber.Kill("Překročen průměrný timeout při komunikaci s hráčem.", Iteration);
                }

            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private void DetonateBombs()
    {
        // odpálíme bomby
        foreach (var bomb in Board.Bombs.ToArray())
        {
            if (bomb.Timer == 0)
            {
                bomb.Detonate(this);
            }
            else
            {
                bomb.Timer--;
            }
        }
    }

    private void CheckCollisions()
    {
        var aliveBombers = Board.AliveBombers.ToArray();

        // kolize hráčů s ohněm
        foreach (var bomber in aliveBombers)
        {
            if (!bomber.Alive) continue;

            if (Board.Fires.Any(f => f.Position == bomber.Position))
            {
                bomber.Kill("Kolize s ohněm.", Iteration);
            }

            if (Board.Mines.FirstOrDefault(f => f == bomber.Position) is { } mine)
            {
                mine.Detonate(this, bomber.Id);
            }
        }
    }

    private void GenerateMines()
    {
        while (Board.Mines.Count < _settings.MineCount)
        {
            if (!Board.TryAddMine()) return;
        }
    }

    /// <summary>
    /// Pro zadaný stav hry v dané iteraci odešle na všechny hráči požadavek na směr pohybu. Pohyb však ve hře nevykoná.
    /// </summary>
    /// <param name="iterationIndex">Index iterace hry.</param>
    public async Task SimulateRequestAsync(int iterationIndex)
    {
        if (iterationIndex < 0 || iterationIndex >= IterationHistory.Count) throw new ArgumentOutOfRangeException(nameof(iterationIndex));
        var gameDto = IterationHistory[iterationIndex];
        foreach (var bomber in Board.Bombers)
        {
            var gameForBomberDto = gameDto with
            {
                You = gameDto.Board.Bombers.Single(s => s.Id == bomber.Id)
            };
            await bomber.SimulateMoveAsync(gameForBomberDto, CancellationToken.None);
        }
    }

    internal bool TryAddFire(int x, int y, string bomberId)
    {
        var coord = new Coordinate(x, y);
        if (!IsOnBoard(coord)) return false;

        if (Board.Fires.FirstOrDefault(f => f.Position == coord) is { } fire)
        {
            if (fire.Intensity < _settings.FireIntensity)
            {
                Board.Fires.Remove(fire);
            }
            else
            {
                return true;
            }
        }
        fire = new Fire
               {
                   BomberId = bomberId,
                   Position = new Coordinate(x, y),
                   Intensity = _settings.FireIntensity
               };
        Board.Fires.Add(fire);

        if (Board.Mines.FirstOrDefault(f => f == coord) is { } mine)
        {
            mine.Detonate(this, bomberId);
        }

        if (Board.Bombs.FirstOrDefault(f => f.Position == coord) is { } bomb2)
        {
            bomb2.Detonate(this);
        }

        if (Board.Bombers.FirstOrDefault(f => f.Position == coord) is { } bomber2)
        {
            bomber2.Kill("Zabit explozí", Iteration);
        }

        return true;
    }

    internal bool IsOnBoard(Coordinate coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < Board.Width && coordinate.Y >= 0 && coordinate.Y < Board.Height;
    }
}