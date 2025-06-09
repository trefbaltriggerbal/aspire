# .NET Aspire Sample

This repository contains a minimal sample that shows how to use **.NET Aspire** to host multiple services with shared defaults and observability.

The solution includes:

- **BlazorApp** – a simple web interface that uses ASP.NET Core Identity.
- **AspireDemo.ServiceDefaults** – a project that configures service discovery, OpenTelemetry and health checks.
- **AspireDemo.AppHost** – a console app that starts the Blazor project as part of a distributed application.
- **BlazorAppUiTest** – Playwright-based UI tests for the Blazor app.

See [TUTORIAL.md](TUTORIAL.md) for a step-by-step walkthrough.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- The Aspire workload:

```bash
dotnet workload install aspire
```

## Building the solution

```bash
dotnet build AspireDemo.sln
```

## Running the distributed app

```bash
dotnet run --project AspireDemo.AppHost
```

The app host launches the Blazor web app and exposes the Aspire dashboard on port **18888**.

## Running tests

```bash
dotnet test AspireDemo.sln
```

UI tests are executed headless by default. See the Playwright section in `AGENTS.md` for more details on configuring the test environment.

