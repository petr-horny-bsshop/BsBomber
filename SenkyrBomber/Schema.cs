using System.Text.Json.Serialization;

namespace SenkyrBomber
{
    public class Game
    {
        [JsonPropertyName("board")]
        public required Board Board { get; init; }

        [JsonPropertyName("you")]
        public required Bomber You { get; init; }

        [JsonPropertyName("iteration")]
        public required int Iteration { get; init; }
    }

    public partial class Bomber
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("url")] 
        public string? Url { get; init; }

        [JsonPropertyName("position")]
        public Coordinate? Position { get; init; }

        [JsonPropertyName("score")]
        public int Score { get; init; }

        [JsonPropertyName("alive")]
        public bool Alive { get; init; }

        [JsonPropertyName("color")]
        public string? Color { get; init; }

        [JsonPropertyName("latency")]
        public int Latency { get; init; }

        [JsonPropertyName("deathCause")]
        public string? DeathCause { get; init; }

        [JsonPropertyName("deathIteration")]
        public int? DeathIteration { get; init; }
    }

    public class Bomb
    {
        [JsonPropertyName("position")]
        public required Coordinate Position { get; init; }

        [JsonPropertyName("timer")]
        public required int Timer { get; init; }

        [JsonPropertyName("bomberId")]
        public required string BomberId { get; init; }
    }

    public class Fire
    {
        [JsonPropertyName("position")]
        public required Coordinate Position { get; init; }

        [JsonPropertyName("bomberId")]
        public required string BomberId { get; init; }

        [JsonPropertyName("intensity")]
        public required int Intensity { get; init; }
    }

    public class Board
    {
        [JsonPropertyName("width")]
        public required int Width { get; init; }

        [JsonPropertyName("height")]
        public required int Height { get; init; }

        [JsonPropertyName("mines")]
        public required Coordinate[] Mines { get; init; }

        [JsonPropertyName("fire")]
        public required Fire[] Fires { get; init; }

        [JsonPropertyName("bombs")]
        public required Bomb[] Bombs { get; init; }

        [JsonPropertyName("bombers")]
        public required Bomber[] Bombers { get; init; }

        [JsonPropertyName("maximumFireIntensity")]
        public required int MaximumFireIntensity { get; init; }
    }

    public class Coordinate
    {
        [JsonPropertyName("x")]
        public required int X { get; init; }

        [JsonPropertyName("y")]
        public required int Y { get; init; }
    }
}
