using BsBomber.Contracts;

namespace BsBomber.Core.Model;

/// <summary>
/// Definuje souřadnici na hrací ploše.
/// </summary>
public class Coordinate
{
    /// <summary>
    /// Pozice X.
    /// </summary>
    public int X { get; }
    
    /// <summary>
    /// Pozice Y.
    /// </summary>
    public int Y { get; }

    /// <summary> ctor. </summary>
    /// <param name="x"><inheritdoc cref="X"/></param>
    /// <param name="y"><inheritdoc cref="Y"/></param>
    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Posune souřadnici o jeden bod v zadaném směru.
    /// </summary>
    public Coordinate Move(BomberAction bomberAction)
    {
        switch (bomberAction)
        {
            case BomberAction.None:
            case BomberAction.PutBomb:
                return new Coordinate(X, Y);
            case BomberAction.GoRight:
                return new Coordinate(X + 1, Y);
            case BomberAction.GoLeft:
                return new Coordinate(X - 1, Y);
            case BomberAction.GoUp:
                return new Coordinate(X, Y + 1);
            case BomberAction.GoDown:
                return new Coordinate(X, Y - 1);
            default: throw new ArgumentOutOfRangeException(nameof(bomberAction), bomberAction, null);
        }
    }

    /// <summary>
    /// Posune souřadnici o zadaný počet bodů.
    /// </summary>
    /// <param name="dx">Delta X.</param>
    /// <param name="dy">Delta Y.</param>
    public Coordinate Translate(int dx, int dy)
    {
        var result = new Coordinate(X + dx, Y + dy);
        return result;
    }

    /// <summary>
    /// Vrátí DTO objekt souřadnice.
    /// </summary>
    public CoordinateDto GetDto()
    {
        return new CoordinateDto(X, Y);
    }

    /// <summary>
    /// Operátor ekvivalence.
    /// </summary>
    public static bool operator == (Coordinate left, Coordinate right) => left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Operátor neekvivalence.
    /// </summary>
    public static bool operator !=(Coordinate left, Coordinate right) => !(left == right);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Coordinate coordinate && this == coordinate;

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}