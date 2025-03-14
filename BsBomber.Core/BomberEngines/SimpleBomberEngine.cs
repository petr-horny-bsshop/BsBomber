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
        switch (status.Iteration % 3)
        {
            case 1:
                return Task.FromResult(new ResponseDto { BomberAction = BomberAction.GoDown });
            case 2:
                return Task.FromResult(new ResponseDto { BomberAction = BomberAction.GoRight });
            case 0:
                
            //return Task.FromResult(new ResponseDto { BomberAction = BomberAction.None});
            return Task.FromResult(new ResponseDto { BomberAction = BomberAction.PutBomb, Argument = 3 });
        }

        
        throw new NotImplementedException();
    }

    

    
}