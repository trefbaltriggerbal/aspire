using Microsoft.Playwright;

internal interface ICheck
{
    string Name { get; }
    Task ExecuteAsync(IPage page);
}
internal sealed class AssertUrlIsCheck : ICheck
{
    public string Name { get; }
    private readonly string _expected;
    public AssertUrlIsCheck(string name, string? expected)
    {
        Name = name;
        _expected = expected ?? throw new ArgumentNullException(nameof(expected));
    }

    public Task ExecuteAsync(IPage page)
    {

        // Lokale URL-delen weghalen voor een duidelijker vergelijk
        string CleanUrl(string url) =>
            url.Replace("https://localhost:7020", "").Replace("http://localhost:5228", "");
        var actualClean = CleanUrl(page.Url);
        var expectedClean = CleanUrl(_expected);

        if (!string.Equals(actualClean, expectedClean, StringComparison.OrdinalIgnoreCase))
        {
            throw new($"URL '{page.Url}' ≠ '{_expected}'");
        }

        return Task.CompletedTask;
    }


    internal sealed class AssertTextNotEqualsCheck : ICheck
    {
        public string Name { get; }
        private readonly string _selector;
        private readonly string _forbidden;

        public AssertTextNotEqualsCheck(string name, string? selector, string? forbidden)
        {
            Name = name;
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _forbidden = forbidden ?? throw new ArgumentNullException(nameof(forbidden));
        }

        public async Task ExecuteAsync(IPage page)
        {
            await page.WaitForSelectorAsync(_selector);
            var text = (await page.InnerTextAsync(_selector)).Trim();
            if (text == _forbidden)
                throw new($"Tekst is '{_forbidden}', maar mocht dat niet zijn.");
        }
    }
}