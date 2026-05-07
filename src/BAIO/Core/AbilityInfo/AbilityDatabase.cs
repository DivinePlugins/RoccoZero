namespace BAIO.Core.AbilityInfo;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class AbilityDatabase
{
    private static Dictionary<string, AbilityInfo> abilityinfoDictionary = new();

    private static List<AbilityInfo> spells;

    static AbilityDatabase()
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BAIO.Core.Resources.AbilityDatabase.json");
        var reader = new StreamReader(stream);

        var json = JsonNode.Parse(reader.ReadToEnd(), null, new() { CommentHandling = JsonCommentHandling.Skip });
        spells = JsonSerializer.Deserialize<AbilityInfo[]>(json["Abilities"]).ToList();
    }

    public static AbilityInfo Find(string abilityName)
    {
        AbilityInfo info;
        if (abilityinfoDictionary.TryGetValue(abilityName, out info))
        {
            return info;
        }

        info = spells.FirstOrDefault(data => data.AbilityName == abilityName);
        abilityinfoDictionary.TryAdd(abilityName, info);

        return info;
    }
}