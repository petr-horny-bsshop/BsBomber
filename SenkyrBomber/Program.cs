using System.Text.Json;

namespace SenkyrBomber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Bomber bomber = new Bomber()
            {
                // jmeno bombice z konfiguracniho souboru appsettings.json
                Name = configuration.GetValue<string>("Bomber:Name")
            };

            WebApplication app = WebApplication.Create(args);

            app.Map($"/move",
                (Game game) => Results.Ok(bomber.Move(game)));
            app.Map($"/init",
                (Game game) => Results.Ok(bomber.Init(game)));
            app.MapGet($"/",
                () => bomber.Index());
            app.MapFallback(
                () => Results.NotFound("Not found"));

            app.Run();
        }
    }
}
