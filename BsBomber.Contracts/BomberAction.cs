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
    /// Umísti bombu pod sebe.
    /// </summary>
    PutBomb
}