using System.Text.Encodings.Web;
using System.Text.Json;
using BsBomber.Contracts;
using BsBomber.Core.BomberEngines;
using BsBomber.Core.Model;

namespace BsBomber.Server;

internal class BomberServer
{
    private IBomberEngine _bomberEngine;

    private BomberServer(int id, WebApplication app)
    {
        app.MapGet($"/bomber/{id}", () =>
        {
            return Results.Ok("ok");
        }); 

        app.MapPost($"/bomber/{id}/init", async (GameDto game) =>
        {
            _bomberEngine = new SimpleBomberEngine();
            await _bomberEngine.InitAsync(game, CancellationToken.None);
            return Results.Ok();
        }); 

        app.MapPost($"/bomber/{id}/move", async (GameDto game) =>
        {
            var json = JsonSerializer.Serialize(game, new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            var result = await _bomberEngine.MoveAsync(game, CancellationToken.None);
            return Results.Ok(result);
        }); 
    }

    public static BomberServer Map(int id, WebApplication app)
    {
        var result = new BomberServer(id, app);
        return result;
    }
}