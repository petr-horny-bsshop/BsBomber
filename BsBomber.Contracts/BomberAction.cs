using System.Text.Json.Serialization;

namespace BsBomber.Contracts;

/// <summary>
/// Akce hráče.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BomberAction
{
    /// <summary>
    /// Nic nedělej.
    /// </summary>
    None,

    /// <summary>
    /// Jdi vpravo.
    /// </summary>
    GoRight,
    
    /// <summary>
    /// Jdi vlevo.
    /// </summary>
    GoLeft,
    
    /// <summary>
    /// Jdi nahoru.
    /// </summary>
    GoUp,
    
    /// <summary>
    /// Jdi dolu.
    /// </summary>
    GoDown,

    /// <summary>
    /// Umísti bombu vpravo.
    /// </summary>
    PutBombRight,

    /// <summary>
    /// Umísti bombu vlevo.
    /// </summary>
    PutBombLeft,

    /// <summary>
    /// Umísti bombu nahoru.
    /// </summary>
    PutBombUp,

    /// <summary>
    /// Umísti bombu dolu.
    /// </summary>
    PutBombDown
}