# Agent Instructions

- Run `dotnet build AspireDemo.sln` after modifying code or docs. If `dotnet` is
  not available, note the failure in the Testing section.
- Install the .NET 8 SDK with `apt-get update` and `apt-get install -y dotnet-sdk-8.0` if `dotnet` is missing.
- Use concise commit messages and describe your changes in the PR summary.
- Always include a Testing section in PRs summarizing any commands run.
- Ensure the solution builds successfully before creating a PR.
- If test projects are present, run `dotnet test` and confirm all tests pass.
- Each PR must provide evidence of running `dotnet build AspireDemo.sln`.
- Only open a PR after all tests pass when test projects are present.
- Keep this repository focused on a minimal sample demonstrating .NET Aspire.
- Ensure it contains:
  - A Blazor project for hands-on testing.
  - Two console projects that communicate with each other in any manner you choose.
  - A tutorial explaining how Aspire works.
- Create GitHub issues to track work on:
  - Console logs
  - Structured logs
  - Traces
  - Metrics
  so that these resources can be viewed when running the console projects.


## UI-tests met Playwright (BlazorAppUiTest)

### Eenmalige setup (Lokaal of CI-agent)

```bash
# .NET 8 SDK moet al aanwezig zijn (zie eerdere sectie)
dotnet tool install --global Microsoft.Playwright.CLI   # voegt 'playwright' CLI toe
playwright install --with-deps                          # download Chromium/FF/WebKit
````

### Test-project aanmaken

```bash
cd aspire                               # repo-root
dotnet new console -n BlazorAppUiTest
cd BlazorAppUiTest
dotnet add package Microsoft.Playwright
cd ..                                   # terug naar root
dotnet sln AspireDemo.sln add BlazorAppUiTest/BlazorAppUiTest.csproj
```

> **Tip voor CI/build-agents**
> Voeg in **BlazorAppUiTest.csproj** dit MSBuild-target toe zodat browsers
> automatisch worden gedownload wanneer `dotnet build` draait:
>
> ```xml
> <Target Name="PlaywrightInstall" AfterTargets="Build">
>   <Exec Command="playwright install --with-deps" />
> </Target>
> ```

### Lokaal ontwikkelen / debuggen

```bash
# Blazor-app starten (aparte shell)
dotnet run --project BlazorApp > /tmp/blazor.log 2>&1 &

# UI-test draaien (headless)
dotnet run --project BlazorAppUiTest

# Visueel meekijken?
#  – pas in Program.cs aan →  Headless = false
#  – voer de test opnieuw uit, Chromium-venster opent
```

### Verwachte console-output

```
Before: Current count: 0 | After: Current count: 1
Counter incremented
```

🔧 Met deze stappen kan elke agent (of CI-runner) de Playwright-tests herhalen en garanderen dat de headless UI-controle van **BlazorApp** altijd werkt.
