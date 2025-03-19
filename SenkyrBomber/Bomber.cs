using System.Text.Json.Serialization;

namespace SenkyrBomber
{
    public partial class Bomber
    {
        // povolene akce bombice
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BomberAction
        {
            None,
            GoRight,
            GoLeft,
            GoUp,
            GoDown,
            PutBomb,
        }

        // format odpovedi
        public class Response
        {
            [JsonPropertyName("bomberAction")]
            public BomberAction BomberAction { get; set; }

            [JsonPropertyName("argument")]
            public int? Argument { get; set; }
        }

        // nedostane data hry, vraci jmeno bombice
        public string? Index()
        {
            return Name;
        }

        // dostane data hry, vraci prazdnou odpoved
        public string Init(Game game)
        {
            return string.Empty;
        }

        // dostane data hry, vraci akci bombice
        public Response Move(Game game)
        {
            /*
                SEM UMISTUJTE SVUJ KOD
            */

            // pro ukazku se vraci nahodny pohyb (bez pokladani bomby)
            BomberAction[] actions = (BomberAction[])Enum.GetValues(typeof(BomberAction));
            BomberAction randomAction = actions[new Random().Next(actions.Length - 1)];

            Response response = new Response() { BomberAction = randomAction };
            
            return response;
        }
    }
}
