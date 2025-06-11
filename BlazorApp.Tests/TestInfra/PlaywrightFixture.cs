using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace UiFlow.Tests.TestInfra;

public sealed class PlaywrightFixture : IAsyncLifetime
{
    public IBrowser Browser { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var pw = await Playwright.CreateAsync();
        Browser = await pw.Chromium.LaunchAsync(new() { Headless = true });
    }

    public async Task DisposeAsync() => await Browser.DisposeAsync();
}
