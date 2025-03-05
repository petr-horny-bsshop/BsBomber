using BsBomber.Contracts;

namespace BsBomber.Core.Model;

internal static class CoordinateDtoExtensions
{
    public static bool IsLeftTo(this CoordinateDto my, CoordinateDto other) => other.Y == my.Y && my.X +1 == other.X;
    public static bool IsRightTo(this CoordinateDto my, CoordinateDto other) => other.Y == my.Y && my.X -1 == other.X;
    public static bool IsAboveTo(this CoordinateDto my, CoordinateDto other) => other.X == my.X && my.Y -1 == other.Y;
    public static bool IsBellowTo(this CoordinateDto my, CoordinateDto other) => other.X == my.X && my.Y +1 == other.Y;
    public static int DistanceTo(this CoordinateDto my, CoordinateDto other) => Math.Abs(other.X - my.X) + Math.Abs(my.Y - other.Y);
    public static CoordinateDto[] GetDto(this IEnumerable<Coordinate> coordinates) => coordinates.Select(c => c.GetDto()).ToArray();

    public static CoordinateDto Move(this CoordinateDto coordinate, BomberAction bomberAction)
    {
        switch (bomberAction)
        {
            case BomberAction.None:
                return new CoordinateDto(coordinate.X, coordinate.Y);
            case BomberAction.GoRight:
                return new CoordinateDto(coordinate.X + 1, coordinate.Y);
            case BomberAction.GoLeft:
                return new CoordinateDto(coordinate.X - 1, coordinate.Y);
            case BomberAction.GoUp:
                return new CoordinateDto(coordinate.X, coordinate.Y + 1);
            case BomberAction.GoDown:
                return new CoordinateDto(coordinate.X, coordinate.Y - 1);
            default:
                throw new ArgumentOutOfRangeException(nameof(bomberAction), bomberAction, null);
        }
    }

    public static BomberAction DirectionTo(this CoordinateDto coordinate, CoordinateDto coordinate2)
    {
        if (coordinate.X < coordinate2.X) return BomberAction.GoRight;
        if (coordinate.X > coordinate2.X) return BomberAction.GoLeft;
        if (coordinate.Y < coordinate2.Y) return BomberAction.GoUp;
        if (coordinate.Y > coordinate2.Y) return BomberAction.GoDown;
        return BomberAction.None;
    }
}