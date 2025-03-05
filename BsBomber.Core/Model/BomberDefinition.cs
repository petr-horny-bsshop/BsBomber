using System.Text.Json.Serialization;

namespace BsBomber.Core.Model
{
    /// <summary>
    /// Definice hráče.
    /// </summary>
    public record BomberDefinition
    {
        /// <summary>
        /// Název hráče.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// Url adresa hráče.
        /// </summary>
        [JsonPropertyName("url")]
        public required string Url { get; init; }
    }
}
