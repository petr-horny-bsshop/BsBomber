using BsBomber.Contracts;

namespace BsBomber.Core.Model;

/// <summary>
/// Engine, který ovládá pohyb hráče.
/// </summary>
public interface IBomberEngine
{
    /// <summary>
    /// Řekne hráči, aby se připravil na novou hru.
    /// </summary>
    Task InitAsync(GameDto gameDto, CancellationToken cancellationToken);

    /// <summary>
    /// Zeptá se hráče, kterým směrem má pohnout.
    /// </summary>
    Task<ResponseDto> MoveAsync(GameDto game, CancellationToken cancellationToken);
}