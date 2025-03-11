using BsBomber.Contracts;
using BsBomber.Core.Model;

namespace BsBomber.Core.BomberEngines;

/// <summary>
/// Jednoduchý engine, který ovládá hráče.
/// </summary>
public class SimpleBomberEngine : IBomberEngine
{
    /// <inheritdoc />
    public Task InitAsync(GameDto gameDto, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<ResponseDto> MoveAsync(GameDto status, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    

    
}