using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

class Program
{
    static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5228");
        await page.ClickAsync("a[href='counter']");
        await page.WaitForSelectorAsync("text=Current count");
        var before = await page.InnerTextAsync("p:has-text('Current count')");
        await page.ClickAsync("text=Click me");
        await page.WaitForFunctionAsync("document.querySelector('p').innerText !== '" + before + "'");
        var after = await page.InnerTextAsync("p:has-text('Current count')");
        Console.WriteLine($"Before: {before} | After: {after}");
        Console.WriteLine(before != after ? "Counter incremented" : "No change");
    }
}
