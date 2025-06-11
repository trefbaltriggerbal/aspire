using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UiFlowRecorder.Extra;

public static class PersistLayer
{
    public static Dictionary<string, string> LoadMapping()
    {
        if (!File.Exists(Config.PersistFile)) return new();
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(Config.PersistFile)) ?? new();
        }
        catch
        {
            Console.WriteLine("⚠️  Kon mapping niet lezen – start schoon.");
            return new();
        }
    }

    public static void SaveMapping(Dictionary<string, string> map)
    {
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(tmp, Config.PersistFile, overwrite: true);
        Console.WriteLine($"💾 Persisted URL map → {Config.PersistFile}");
    }

    public static void PrintOverview(IEnumerable<ScreenshotInfo> shots, IReadOnlyDictionary<string, string> urlMap)
    {
        Console.WriteLine("\n=== Stap → Screenshot → MD5 → Link ===");
        foreach (var s in shots)
        {
            var url = urlMap.TryGetValue(s.Md5, out var u) ? u : "<not-uploaded>";
            Console.WriteLine($"{s.Label}.png → {s.Md5} → {url}");
        }
        Console.WriteLine("======================================\n");
    }
}
