using Microsoft.Playwright;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UiFlow.Tests.TestInfra;

public class ScreenshotService
{
    private readonly IPage _page;

    public ScreenshotService(IPage page)
    {
        _page = page;
    }

    public async Task<string> CaptureAsync(string label)
    {
        var path = $"{label}.png";
        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
        var md5 = Convert.ToHexString(MD5.HashData(File.ReadAllBytes(path)));
        return md5;
    }

    public string Capture(string label)
    {
        return CaptureAsync(label).GetAwaiter().GetResult();
    }
}
