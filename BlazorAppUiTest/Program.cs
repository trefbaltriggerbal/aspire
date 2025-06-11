using Microsoft.Playwright;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace UiFlowRecorder;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;

        // Pad naar de JSON-flowdefinitie (of “flows.json” als standaard)
        var jsonPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "flows.json";
        if (!File.Exists(jsonPath))
        {
            var alt = Path.Combine("BlazorAppUiTest", "flows.json");
            if (File.Exists(alt)) jsonPath = alt;
        }

        var flows = FlowStepBuilder.FromJsonFile(jsonPath);
        if (flows.Count == 0)
        {
            Console.WriteLine($"⚠️  Geen flows gevonden in {jsonPath} – stoppen.");
            Environment.Exit(1);
        }

        var playwright = await Playwright.CreateAsync();
        var anyFailure = false;

        try
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = !isWindows 
            });
            var page = await browser.NewPageAsync();
            await page.SetViewportSizeAsync(1280, 720);

            var screenshotSvc = new ScreenshotService(page);
            var results = new List<StepResult>();

            foreach (var flow in flows)
            {
                Console.WriteLine($"🔁 Flow: {flow.Name}");
                foreach (var step in flow.Steps)
                {
                    Console.WriteLine($"   ➡️  Step: {step.Name}");
                    var ok = true;
                    var failedChecks = new List<string>();

                    /* ────── UI-actie ────── */
                    try { await step.Action(page); }
                    catch (Exception ex)
                    {
                        ok = false;
                        anyFailure = true;
                        Console.WriteLine($"      ❌  Step failed: {ex.Message}");
                    }

                    /* ────── Checks / asserts ────── */
                    foreach (var check in step.Checks)
                    {
                        try { await check.ExecuteAsync(page); }
                        catch (Exception ex)
                        {
                            failedChecks.Add($"❌ {check.Name}: {ex.Message}");
                            anyFailure = true;
                        }
                    }

                    /* ────── Screenshot + resultaat ────── */
                    var md5 = await screenshotSvc.CaptureAsync($"{flow.Name}_{step.Name}");
                    results.Add(new(
                        FlowName: flow.Name,
                        Label: step.Name,
                        Success: ok && failedChecks.Count == 0,
                        Md5: md5,
                        FailedChecks: failedChecks));
                }
            }

            /* ────── Uploaden & mapping ────── */
            var uploader = new Uploader();
            await uploader.UploadAsync(screenshotSvc.Screenshots);

            PersistLayer.SaveMapping(uploader.HashToUrl);
            PersistLayer.PrintOverview(screenshotSvc.Screenshots, uploader.HashToUrl);

            /* ────── Markdown-rapport ────── */
            var md = BuildMarkdown(results, uploader.HashToUrl);
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "FileToGiveBackToUser.md");
            File.WriteAllText(outputPath, md, Encoding.UTF8);
            Console.WriteLine($"📝 Verslag opgeslagen als {outputPath}");
        }
        finally
        {
            if (playwright is IAsyncDisposable ad) await ad.DisposeAsync();
        }

        /* ────── Exit-code voor CI ────── */
        if (anyFailure)
        {
            Console.WriteLine("❌ Eén of meer checks zijn gefaald – job eindigt met code 1.");
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine("✅ Alle checks/steps geslaagd.");
        }
    }

    /* ───────────────────────────────────────── Markdown ───────────────────────────────────────── */
    private static string BuildMarkdown(IEnumerable<StepResult> steps, IReadOnlyDictionary<string, string> urlMap)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🧪 UI Flow Verslag\n");

        var hasFails = steps.Any(s => !s.Success);
        sb.AppendLine(hasFails
            ? "**❌ Eén of meer checks/steps gefaald**\n"
            : "**✅ Alle checks/steps geslaagd**\n");

        foreach (var group in steps.GroupBy(s => s.FlowName))
        {
            sb.AppendLine($"## 🔁 Flow: {group.Key}\n");
            int i = 1;
            foreach (var s in group)
            {
                var icon = s.Success ? "✅" : "❌";
                var url = urlMap.TryGetValue(s.Md5, out var link) ? link : "<not-uploaded>";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{s.FlowName}_{s.Label}.png");
                var sizeMiB = File.Exists(filePath) ? new FileInfo(filePath).Length / 1024.0 / 1024.0 : 0;
                var expiration = CalculateExpirationDate(sizeMiB);

                sb.AppendLine($"### {icon} {i++}. {s.Label}\n");
                sb.AppendLine($"![{s.Label}]({url})\n");
                sb.AppendLine($"<sub>Bestandsgrootte: {(sizeMiB * 1024):F3} kB – Vervaldatum: {expiration}</sub>\n");

                if (s.FailedChecks.Count > 0)
                {
                    sb.AppendLine("#### ❌ Mislukte checks\n");
                    foreach (var fail in s.FailedChecks)
                        sb.AppendLine($"1. {fail}\n");
                }
            }
        }
        return sb.ToString();
    }

    private static string CalculateExpirationDate(double fileSizeMib)
    {
        const double maxSize = 512.0;
        const int minAge = 30;
        const int maxAge = 365;

        var term = Math.Pow((fileSizeMib / maxSize - 1), 3);
        var retentionDays = minAge + (minAge - maxAge) * term;
        var expiration = DateTime.UtcNow.AddDays(retentionDays);

        return expiration.ToString("dddd dd MMMM yyyy 'om' HH:mm:ss (UTC)");
    }
}

/* ───────────────────────────────────────── Models ───────────────────────────────────────── */
public sealed record StepResult(
    string FlowName,
    string Label,
    bool Success,
    string Md5,
    List<string> FailedChecks
    
    );
