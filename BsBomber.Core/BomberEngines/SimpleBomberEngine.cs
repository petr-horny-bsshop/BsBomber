using BsBomber.Contracts;
using BsBomber.Core.Model;

namespace BsBomber.Core.BomberEngines;

/// <summary>
/// Jednoduchý engine, který ovládá hráče.
/// </summary>
public class SimpleBomberEngine : IBomberEngine
{
    /// <inheritdoc />
    public Task<ResponseDto> MoveAsync(GameDto status, CancellationToken cancellationToken)
    {
        var allowedActions = new HashSet<BomberAction>
        {
            BomberAction.None,
            BomberAction.GoRight,
            BomberAction.GoUp,
            BomberAction.GoDown,
            BomberAction.GoLeft
        };

        // kolize s okraji hrací plochy
        if (status.You.Position.X >= status.Board.Width - 1) allowedActions.Remove(BomberAction.GoRight);
        if (status.You.Position.X <= 0) allowedActions.Remove(BomberAction.GoLeft);
        if (status.You.Position.Y <= 0) allowedActions.Remove(BomberAction.GoDown);
        if (status.You.Position.Y >= status.Board.Height - 1) allowedActions.Remove(BomberAction.GoUp);

        // kolize s ostatními hráči (včetně sebe)
        foreach (var bomber in status.Board.Bombers.Where(s => s.Alive))
        {
            if (status.You.Position.IsRightTo(bomber.Position)) allowedActions.Remove(BomberAction.GoLeft);
            if (status.You.Position.IsLeftTo(bomber.Position)) allowedActions.Remove(BomberAction.GoRight);
            if (status.You.Position.IsAboveTo(bomber.Position)) allowedActions.Remove(BomberAction.GoDown);
            if (status.You.Position.IsBellowTo(bomber.Position)) allowedActions.Remove(BomberAction.GoUp);
        }

        // kolize s překážkami
        foreach (var coordinate in status.Board.Obstacles)
        {
            if (status.You.Position.IsRightTo(coordinate)) allowedActions.Remove(BomberAction.GoLeft);
            if (status.You.Position.IsLeftTo(coordinate)) allowedActions.Remove(BomberAction.GoRight);
            if (status.You.Position.IsAboveTo(coordinate)) allowedActions.Remove(BomberAction.GoDown);
            if (status.You.Position.IsBellowTo(coordinate)) allowedActions.Remove(BomberAction.GoUp);
        }

        var preferredDirection = DecideAction(status);
        if (allowedActions.Contains(preferredDirection)) return Task.FromResult(new ResponseDto {BomberAction = preferredDirection});

        var result = allowedActions.FirstOrDefault();
        return Task.FromResult(new ResponseDto { BomberAction = result });
    }

    /// <inheritdoc />
    public Task InitAsync(GameDto gameDto, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private BomberAction DecideAction(GameDto status)
    {
        var distanceToFood = status.Board.Food.OrderBy(f => status.You.Position.DistanceTo(f));

        foreach (var food in distanceToFood)
        {
            var direction = status.You.Position.DirectionTo(food);
        }

        return default;
    }

       
}