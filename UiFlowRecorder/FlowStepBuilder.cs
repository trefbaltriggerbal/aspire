
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace UiFlowRecorder;

public static class FlowStepBuilder
{
    public static List<FlowDefinition> FromJsonFile(string path)
    {
        var json = File.ReadAllText(path);
        var flows = JsonSerializer.Deserialize<List<JsonFlow>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        return flows.Select(f => new FlowDefinition(
            f.Name,
            f.Steps.Select(s => new FlowStep(
                s.Name,
                async page => await page.GotoAsync(s.Data), // Simplified for demo
                new List<ICheck>()
            )).ToArray()
        )).ToList();
    }
}

public class JsonFlow
{
    public string Name { get; set; } = string.Empty;
    public List<JsonFlowStep> Steps { get; set; } = new();
}

public class JsonFlowStep
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Data { get; set; }
}

public record FlowStep(string Name, Func<Microsoft.Playwright.IPage, Task> Action, List<ICheck> Checks);
public record FlowDefinition(string Name, FlowStep[] Steps);

public interface ICheck
{
    string Name { get; }
    Task ExecuteAsync(Microsoft.Playwright.IPage page);
}
