using Microsoft.Playwright;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace UiFlowRecorder;

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
