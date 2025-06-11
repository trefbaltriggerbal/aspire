using Microsoft.Playwright;
using System.Threading.Tasks;
using UiFlow.Tests.TestInfra;
using UiFlowRecorder.Extra;
using Xunit;

namespace UiFlow.Tests.Flows;

[Collection("Playwright")]
public class LoginFlow : IClassFixture<PlaywrightFixture>
{
    private readonly IPage _page;

    public LoginFlow(PlaywrightFixture fx)
    {
        _page = fx.Browser.NewPageAsync().Result;
        UiRunState.Shots = new ScreenshotService(_page);
    }

    [Fact, UiStep("Login", "Login", "Open login")]
    public async Task Step1() =>
        await _page.GotoAsync("http://localhost:5228/Account/Login");

    [Fact, UiStep("Login", "Login", "Assert page change")]
    public async Task Step2() =>
        await _page.WaitForSelectorAsync("text=Login");
}
