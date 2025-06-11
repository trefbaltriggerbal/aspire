using Microsoft.Playwright;
using System.Runtime.InteropServices;
using System.Text.Json;
using static AssertUrlIsCheck;

namespace UiFlowRecorder;

/// <summary>
/// Leest een JSON-bestand en bouwt Playwright-flows, mét ondersteuning voor asserts.
/// </summary>
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
                GetAction(s),
                GetChecks(s) // 👈 check-lijst toevoegen
            )).ToArray()
        )).ToList();
    }

    private static List<ICheck> GetChecks(JsonFlowStep step)
    {
        var list = new List<ICheck>();

        // checks worden apart gedefinieerd met Type zoals "AssertUrlIs", enz.
        // (voeg eventueel méér types toe hier)
        if (step.Type.StartsWith("Assert", StringComparison.OrdinalIgnoreCase))
        {
            switch (step.Type)
            {
                case "AssertUrlIs":
                    list.Add(new AssertUrlIsCheck(step.Name, step.Expected));
                    break;

                case "AssertTextNotEquals":
                    list.Add(new AssertTextNotEqualsCheck(step.Name, step.Selector, step.Expected));
                    break;

                    // voeg eventueel andere checks toe…
            }
        }

        return list;
    }


    private static Func<IPage, Task> GetAction(JsonFlowStep step) => step.Type switch
    {
        // ---------- gewone UI-acties ----------
        "GotoAsync" => async page =>
        {
            var url = step.Data?.Trim().Equals("Config.BaseUrl", StringComparison.OrdinalIgnoreCase) == true
                        ? Config.BaseUrl.ToString()
                        : step.Data ?? string.Empty;

            await page.GotoAsync(url);
        }
        ,
        "ClickAsync" => async page => await page.ClickAsync(step.Data!),
        "WaitForSelectorAsync" => async page => await page.WaitForSelectorAsync(step.Data!),
        "WaitTimeout" => async page => await page.WaitForTimeoutAsync(int.Parse(step.Data!)),
        "FillAsync" => async page =>
        {
            var selector = step.Selector ?? throw new("Selector ontbreekt");
            var value = step.Data ?? string.Empty;

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isAffectedControlName = selector == "input[name='Input.Email']" || selector == "input[name='Input.UserName']";

            if (isWindows && isAffectedControlName && page.Url.Contains("Register")) {
                value += Guid.NewGuid().ToString(); //
            }

            await page.FillAsync(selector, value);
        },

        // ---------- ASSERTS / CHECKS ----------
        // ⬇️  Faalt de check → Exception → step wordt ❌ in het verslag
        "AssertTextNotEquals" => async page =>
        {
            var selector = step.Selector ?? throw new("Selector ontbreekt");
            var forbidden = step.Expected ?? throw new("Expected ontbreekt");

            try
            {
                await page.WaitForSelectorAsync(selector);

                var text = (await page.InnerTextAsync(selector)).Trim();

                if (text == forbidden)
                    throw new($"AssertTextNotEquals faalde: tekst is '{forbidden}'.");
            }
            catch (Exception e)
            {
                /*Timeout 30000ms exceeded.
Call log:
  - waiting for Locator("p#counter")*/
                throw;
            }
        }
        ,
        "AssertTextEquals" => async page =>
        {
            var selector = step.Selector ?? throw new("Selector ontbreekt");
            var expected = step.Expected ?? throw new("Expected ontbreekt");
            var text = (await page.InnerTextAsync(selector)).Trim();

            if (text != expected)
                throw new($"AssertTextEquals faalde: kreeg '{text}', verwacht '{expected}'.");
        }
        ,
        "AssertUrlIs" => async page =>
        {
            var expected = step.Expected ?? throw new InvalidOperationException("❌ 'Expected' ontbreekt.");

            // Lokale URL-delen weghalen voor een duidelijker vergelijk
            string CleanUrl(string url) =>
                url.Replace("https://localhost:7020", "").Replace("http://localhost:5228", "");

            var actualClean = CleanUrl(page.Url);
            var expectedClean = CleanUrl(expected);

            if (!string.Equals(actualClean, expectedClean, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"❌ AssertUrlIs faalde:\n" +
                    $"   Verwacht: '{expectedClean}'\n" +
                    $"   Gevonden: '{actualClean}'"
                );
            }
        }

        ,
        "AssertUrlContains" => async page =>
        {
            var substr = step.Expected ?? throw new("Expected ontbreekt");
            if (!page.Url.Contains(substr, StringComparison.OrdinalIgnoreCase))
                throw new($"AssertUrlContains faalde: '{page.Url}' bevat '{substr}' niet.");
        }
        ,

        // ---------- onbekend ----------
        _ => throw new InvalidOperationException($"Onbekend step type: {step.Type}")
    };
}

public class JsonFlow
{
    public string Name { get; set; } = string.Empty;
    public List<JsonFlowStep> Steps { get; set; } = new();
}

/// <summary>
/// Eén stap uit de JSON.  Voor asserts zijn 'Selector' en/of 'Expected' optioneel nodig.
/// </summary>
public class JsonFlowStep
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // bv. ClickAsync, AssertUrlIs …
    public string? Data { get; set; }                   // extra string (bv. CSS-selector)
    public string? Selector { get; set; }                   // voor asserts op tekst
    public string? Expected { get; set; }                   // verwachte/verboden waarde
}

/// <summary> Eén stap met UI-actie + zero-of-meer checks. </summary>
public sealed record FlowStep(
    string Name,
    Func<IPage, Task> Action,
    List<ICheck> Checks);

/// <summary> Een volledige flow. </summary>
public sealed record FlowDefinition(
    string Name,
    FlowStep[] Steps);