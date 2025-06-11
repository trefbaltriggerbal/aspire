using System.Net;
using System.Net.Http.Headers;

namespace UiFlowRecorder;

public sealed class Uploader
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
