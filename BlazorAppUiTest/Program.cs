using Microsoft.Playwright;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace UiFlowRecorder;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;

        // 1ste niet-optionele arg = JSON-pad, anders “flows.json”
        var jsonPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "flows.json";
        var headed = args.Any(a => a is "--headed" or "-H");

        // 🔄  Flows inladen
        var flows = FlowStepBuilder.FromJsonFile(jsonPath);
        if (flows.Count == 0)
        {
            Console.WriteLine($"⚠️  Geen flows gevonden in {jsonPath} – stoppen.");
            return;
        }

        var playwright = await Playwright.CreateAsync();
        try
        {
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
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
                    try { await step.Action(page); }
                    catch (Exception ex)
                    {
                        ok = false;
                        Console.WriteLine($"      ❌  Step failed: {ex.Message}");
                    }

                    var md5 = await screenshotSvc.CaptureAsync($"{flow.Name}_{step.Name}");
                    results.Add(new(flow.Name, step.Name, ok, md5));
                }
            }

            var uploader = new Uploader();
            await uploader.UploadAsync(screenshotSvc.Screenshots);

            PersistLayer.SaveMapping(uploader.HashToUrl);
            PersistLayer.PrintOverview(screenshotSvc.Screenshots, uploader.HashToUrl);

            var md = BuildMarkdown(results, uploader.HashToUrl);
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "FileToGiveBackToUser.md");
            File.WriteAllText(outputPath, md, Encoding.UTF8);
            Console.WriteLine($"📝 Verslag opgeslagen als {outputPath}");
        }
        finally
        {
            if (playwright is IAsyncDisposable ad)
                await ad.DisposeAsync();
            else
                (playwright as IDisposable)?.Dispose();
        }

        Console.WriteLine("Druk op Enter om af te sluiten…");
    }

    private static string BuildMarkdown(IEnumerable<StepResult> steps, IReadOnlyDictionary<string, string> urlMap)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🧪 UI Flow Verslag\n");

        // Groepeer per flow
        foreach (var group in steps.GroupBy(s => s.FlowName))
        {
            sb.AppendLine($"## 🔁 Flow: {group.Key}\n");
            int i = 1;
            foreach (var s in group)
            {
                var icon = s.Success ? "✅" : "❌";
                var url = urlMap.TryGetValue(s.Md5, out var link) ? link : "<not-uploaded>";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{s.FlowName}_{s.Label}.png");
                double sizeMiB = File.Exists(filePath) ? new FileInfo(filePath).Length / 1024.0 / 1024.0 : 0;
                var expiration = CalculateExpirationDate(sizeMiB);

                sb.AppendLine($"### {icon} {i++}. {s.Label}\n");
                sb.AppendLine($"![{s.Label}]({url})\n");
                sb.AppendLine($"<sub>Bestandsgrootte: {(sizeMiB * 1024):F0} kB – Vervaldatum: {expiration}</sub>\n");
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
        return expiration.ToString("dddd dd MMMM yyyy 'om' HH:mm:ss (UTC)", new CultureInfo("nl-BE"));
    }
}

internal sealed record StepResult(string FlowName, string Label, bool Success, string Md5);

// Re-added support classes

internal static class Config
{
    public static readonly Uri BaseUrl = new("http://localhost:5228");
    public const string UploadEndpoint = "https://0x0.st";
    public const string PersistFile = "UploadMap2.json";
    public const string UserAgent = "curl/8.5.0";
    public static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }
}

internal sealed class ScreenshotService
{
    private readonly IPage _page;
    private readonly ConcurrentBag<ScreenshotInfo> _screens = new();

    public IReadOnlyCollection<ScreenshotInfo> Screenshots => _screens;

    public ScreenshotService(IPage page) => _page = page;

    public async Task<string> CaptureAsync(string label)
    {
        var file = $"{label}.png";
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = true });
        var md5 = CalcMd5(file);
        _screens.Add(new(label, file, md5));
        Console.WriteLine($"   📸 {file} (MD5 {md5})");
        return md5;
    }

    private static string CalcMd5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var bytes = md5.ComputeHash(stream);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

internal sealed record ScreenshotInfo(string Label, string FilePath, string Md5);

internal sealed class Uploader
{
    public Dictionary<string, string> HashToUrl { get; } = PersistLayer.LoadMapping();

    public async Task UploadAsync(IEnumerable<ScreenshotInfo> shots, CancellationToken ct = default)
    {
        var unique = new Dictionary<string, ScreenshotInfo>();
        foreach (var s in shots) unique.TryAdd(s.Md5, s);

        using var semaphore = new SemaphoreSlim(4);
        var tasks = new List<Task>();

        foreach (var (hash, info) in unique)
        {
            if (HashToUrl.ContainsKey(hash)) continue;

            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync(ct);
                try
                {
                    var url = await TryUploadAsync(info.FilePath, ct) ??
                              await TryUploadAsync(info.FilePath, ct, asPng: false);

                    HashToUrl[hash] = url ?? "<not-uploaded>";
                    Console.WriteLine(url is null
                        ? $"      ⚠️  Upload FAILED voor {info.FilePath}"
                        : $"      ✅ Done → {url}");
                }
                finally { semaphore.Release(); }
            }, ct));
        }

        await Task.WhenAll(tasks);
    }

    private static async Task<string?> TryUploadAsync(string path, CancellationToken ct, bool asPng = true)
    {
        try
        {
            await using var stream = File.OpenRead(path);
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(asPng ? "image/png" : "application/octet-stream");

            using var form = new MultipartFormDataContent { { content, "file", Path.GetFileName(path) } };
            using var resp = await Config.Http.PostAsync(Config.UploadEndpoint, form, ct);

            if (resp.StatusCode == HttpStatusCode.Forbidden) return null;
            resp.EnsureSuccessStatusCode();
            var link = (await resp.Content.ReadAsStringAsync(ct)).Trim();
            return link.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? link : null;
        }
        catch { return null; }
    }
}

internal static class PersistLayer
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
