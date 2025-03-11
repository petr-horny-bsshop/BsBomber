namespace BsBomber.Core.Model;

/// <summary>
/// Mina.
/// </summary>
/// <inheritdoc />
public class Mine(int x, int y) : Coordinate(x, y)
{
    /// <summary>
    /// Odpálí bombu.
    /// </summary>
    public void Detonate(Game game, string bomberId)
    {
        game.TryAddFire(X - 1, Y - 1, bomberId);
        game.TryAddFire(X, Y - 1, bomberId);
        game.TryAddFire(X + 1, Y - 1, bomberId);
        game.TryAddFire(X - 1, Y, bomberId);
        game.TryAddFire(X - 1, Y + 1, bomberId);
        game.TryAddFire(X, Y + 1, bomberId);
        game.TryAddFire(X + 1, Y + 1, bomberId);
        game.TryAddFire(X + 1, Y, bomberId);
        game.Board.Mines.Remove(this);

        var bomber = game.Board.Bombers.First(b => b.Id == bomberId);
        bomber.AddScore();
    }
}
