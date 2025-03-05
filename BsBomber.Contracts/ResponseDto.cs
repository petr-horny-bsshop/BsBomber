namespace BsBomber.Contracts
{
    /// <summary>
    /// Odpověď hráče v aktuální iteraci.
    /// </summary>
    public record ResponseDto
    {
        /// <summary>
        /// Akce, kterou chce hráč provést.
        /// </summary>
        public BomberAction BomberAction { get; set; }
    }
}
