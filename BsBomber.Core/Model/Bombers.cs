using System.Text.Json;

namespace BsBomber.Core.Model;

/// <summary>
/// Helper třída pro načítání a ukládání definic hráčů.
/// </summary>
public static class Bombers
{
    /// <summary>
    /// Načte definice hráčů ze souboru bombers.json.
    /// </summary>
    public static IReadOnlyList<BomberDefinition> LoadBomberDefinitions()
    {
        if (!File.Exists("bombers.json")) return Array.Empty<BomberDefinition>();
        var json = File.ReadAllText("bombers.json");
        var result = JsonSerializer.Deserialize<BomberDefinition[]>(json) ?? Array.Empty<BomberDefinition>();
        return result;
    }

    /// <summary>
    /// Uloží definice hráčů do souboru bombers.json.
    /// </summary>
    public static void SaveBomberDefinitions(IList<BomberDefinition> bomberDefinitions)
    {
        var json = JsonSerializer.Serialize(bomberDefinitions);
        File.WriteAllText("bombers.json", json);
    }
}