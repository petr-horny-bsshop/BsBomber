namespace BsBomber.Contracts;

/// <summary>
/// Extension metody pro <see cref="BomberAction"/>.
/// </summary>
public static class BomberActionExtensions
{
    /// <summary>
    /// Zda se jedná o pohyb.
    /// </summary>
    public static bool IsMovement(this BomberAction action) => action switch
    {
        BomberAction.GoRight => true,
        BomberAction.GoLeft => true,
        BomberAction.GoUp => true,
        BomberAction.GoDown => true,
        _ => false
    };

    /// <summary>
    /// Zda se jedná o umístění bomby.
    /// </summary>
    public static bool IsBombPlacing(this BomberAction action) => action switch
    {
        BomberAction.PutBombRight => true,
        BomberAction.PutBombLeft => true,
        BomberAction.PutBombUp => true,
        BomberAction.PutBombDown => true,
        _ => false
    };
}