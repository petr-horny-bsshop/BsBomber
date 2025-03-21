using System.Diagnostics.CodeAnalysis;
using BsBomber.Contracts;

namespace BsBomber.Core.Model;

/// <summary>
/// Definuje hrací plochu.
/// Souřadnice [0,0] odpovídá levému dolnímu rohu.
/// </summary>
public class Board
{
    private readonly List<Bomber> _bombers = new List<Bomber>();

    /// <summary>
    /// Šířka hrací plochy.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Výška hrací plochy.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Seznam pozic s minou na hrací ploše.
    /// </summary>
    public IList<Mine> Mines { get; } = new List<Mine>();

    /// <summary>
    /// Seznam pozic s ohněm (po detonaci bomby) na hrací ploše.
    /// </summary>
    public IList<Fire> Fires { get; } = new List<Fire>();

    /// <summary>
    /// Seznam všech bomb na hrací ploše.
    /// </summary>
    public LinkedList<Bomb> Bombs { get; } = new();

    /// <summary>
    /// Seznam všech hráčů na hrací ploše.
    /// </summary>
    public IReadOnlyCollection<Bomber> Bombers => _bombers;
    
    /// <summary>
    /// Seznam živých hráčů na hrací ploše.
    /// </summary>
    public IEnumerable<Bomber> AliveBombers => _bombers.Where(s => s.Alive);

    /// <summary> ctor. </summary>
    /// <param name="width"><inheritdoc cref="Width"/></param>
    /// <param name="height"><inheritdoc cref="Height"/></param>
    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Přidá hráče na hrací plochu.
    /// </summary>
    /// <param name="bomberEngine">Engine hráče.</param>
    /// <param name="name">Název hráče.</param>
    /// <param name="color">Barva  hráče.</param>
    public Bomber AddBomber(IBomberEngine bomberEngine, string name, string color)
    {
        var bomber = new Bomber(GetFreeCell(), bomberEngine, name, color);
        _bombers.Add(bomber);
        return bomber;
    }

    /// <summary>
    /// Vrátí DTO s informacemi o hrací ploše.
    /// </summary>
    public BoardDto GetDto(int maximumFireIntensity)
    {
        var dto = new BoardDto
        {
            Mines = Mines.GetDto(),
            Bombs = Bombs.Select(b => b.GetDto()).ToArray(),
            Fires = Fires.Select(f =>f .GetDto()).ToArray(),
            Height = Height,
            Width = Width,
            Bombers = Bombers.Select(s => s.GetDto()).ToArray(),
            MaximumFireIntensity = maximumFireIntensity
        };
        return dto;
    }

    /// <summary>
    /// Přidá minu na náhodně vybrané volné pole.
    /// </summary>
    public bool TryAddMine()
    {
        if (TryGetFreeCell(out var coordinate))
        {
            Mines.Add(new Mine(coordinate.X, coordinate.Y));
            return true;
        }

        return false;
    }

    private Coordinate GetFreeCell()
    {
        if (TryGetFreeCell(out var coordinate)) return coordinate;
        throw new Exception("Nepovedlo se najít volnou buňku.");
    }

    private bool TryGetFreeCell([NotNullWhen(true)]out Coordinate? coordinate, int maxAttempts=100)
    {
        for(var attempt=0; attempt<maxAttempts; attempt++)
        {
            var x = Random.Shared.Next(0, Width);
            var y = Random.Shared.Next(0, Height);
            coordinate = new Coordinate(x, y);
            if (!IsCellFree(coordinate)) continue;
            if (!IsCellFree(coordinate.Translate(-1,-1))) continue;
            if (!IsCellFree(coordinate.Translate(-1,0))) continue;
            if (!IsCellFree(coordinate.Translate(-1,1))) continue;
            if (!IsCellFree(coordinate.Translate(0,-1))) continue;
            if (!IsCellFree(coordinate.Translate(0,0))) continue;
            if (!IsCellFree(coordinate.Translate(0,1))) continue;
            if (!IsCellFree(coordinate.Translate(1,-1))) continue;
            if (!IsCellFree(coordinate.Translate(1,0))) continue;
            if (!IsCellFree(coordinate.Translate(1,1))) continue;
            
            return true;
        }

        coordinate = default;
        return false;
    }

    private bool IsCellFree(Coordinate coordinate)
    {
        if (Mines.Any(f => f == coordinate)) return false;
        if (AliveBombers.Any(f => f.Position == coordinate)) return false;
        if (Bombs.Any(f => f.Position == coordinate)) return false;
        if (Fires.Any(f => f.Position == coordinate)) return false;
        return true;
    }

}