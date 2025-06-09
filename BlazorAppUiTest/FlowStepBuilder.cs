using Microsoft.Playwright;
using System.Text.Json;

namespace UiFlowRecorder;

/// <summary>Leest een JSON-bestand en bouwt testflows.</summary>
internal static class FlowStepBuilder
{
    public static List<FlowDefinition> FromJsonFile(string path)
    {
        var json = File.ReadAllText(path);
        var flows = JsonSerializer.Deserialize<List<JsonFlow>>(json,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        return flows.Select(f => new FlowDefinition(
            f.Name,
            f.Steps.Select(s => new FlowStep(s.Name, GetAction(s))).ToArray()
        )).ToList();
    }

    private static Func<IPage, Task> GetAction(JsonFlowStep step) => step.Type switch
    {
        "GotoAsync" => async page =>
        {
            var url = step.Data == "Config.BaseUrl" ? Config.BaseUrl.ToString() : step.Data;
            await page.GotoAsync(url);
        }
        ,
        "ClickAsync" => async page => await page.ClickAsync(step.Data),
        "WaitForSelectorAsync" => async page => await page.WaitForSelectorAsync(step.Data),
        "WaitTimeout" => async page => await page.WaitForTimeoutAsync(int.Parse(step.Data)),
        _ => throw new InvalidOperationException($"Onbekend step type: {step.Type}")
    };
}

internal class JsonFlow
{
    public string Name { get; set; } = string.Empty;
    public List<JsonFlowStep> Steps { get; set; } = new();
}

internal class JsonFlowStep
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

internal sealed record FlowStep(string Name, Func<IPage, Task> Action);
internal sealed record FlowDefinition(string Name, FlowStep[] Steps);
