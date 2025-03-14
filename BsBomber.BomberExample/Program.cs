using BsBomber.Contracts;
using BsBomber.Core.BomberEngines;

namespace BsBomber.BomberExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            var bomberEngine = new SimpleBomberEngine();

            app.MapGet("/", () => Results.Ok("toto je ukázkový bomber")); 

            app.MapPost("/init", (GameDto _) => Results.Ok()); 

            app.MapPost("/move", async (GameDto game) =>
            {
                var response = await bomberEngine.MoveAsync(game, CancellationToken.None);

                return Results.Ok(response);
            }); 

            app.Run();
        }
    }
}