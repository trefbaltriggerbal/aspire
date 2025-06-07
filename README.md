# Aspire Demo

This repository contains a minimal .NET 8 sample that demonstrates how to use the preview of **.NET Aspire** to orchestrate multiple services. The solution consists of:

- `Sender` – console app that sends an HTTP request to `Receiver`.
- `Receiver` – console app that hosts a small HTTP server and responds with `Pong`.
- `WebFrontend` – Blazor Web App that can trigger the `Sender` project.
- `AspireHost` – console project that will host the distributed application using the `Aspire.Hosting` package.
- `LCG API` – HTTP service exposing the linear congruential generator.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install the Aspire workload:

```bash
dotnet workload install aspire
```

## Building the solution

```bash
dotnet build AspireDemo.sln
```

## Running the apps individually

Start the receiver in one terminal:

```bash
dotnet run --project Receiver
```

Then in another terminal start the sender:

```bash
dotnet run --project Sender
```

The sender will call `http://receiver/ping` using service discovery to reach the receiver.

Both console projects run through the `Host` builder and use
`AddServiceDefaults` so their logs are forwarded to Aspire's monitoring
pipeline via OpenTelemetry. The built‑in JSON console formatter still outputs
structured logs locally, and each message includes its log level, timestamp and
structured data to make log analysis easier.

`AddServiceDefaults` also wires up OpenTelemetry HTTP instrumentation so the
`traceparent` header is added automatically to requests made by `HttpClient`.
The receiver extracts that header using the default propagator so spans from
both services share the same TraceId in the Aspire dashboard.

## Configuring log levels

Each project uses a root namespace starting with `Projects`, such as `Projects.Sender`. The logging configuration leverages this prefix so you can control the verbosity of your own code separately from framework components. An example `appsettings.json` section looks like:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Projects": "Debug"
  }
}
```

Because the `Projects` category matches each project's root namespace, adjusting this setting filters user-generated logs without affecting messages from ASP.NET Core or other libraries.


## Running with Aspire

The `AspireHost` project references the `Aspire.Hosting` package. When executed it will start the other projects and manage them as a distributed application. Run it with:

```bash
dotnet run --project AspireHost
```

This will launch the receiver and the web frontend as part of the distributed app. The sender project can be triggered from the web frontend or run separately.

## Next steps

This repository serves as a starting point for experimenting with .NET Aspire. Feel free to extend the projects, add more services or containers and explore the orchestration capabilities provided by Aspire.

For a step-by-step walkthrough see [the tutorial](TUTORIAL.md).

## Planned improvements

The following GitHub issues track upcoming features for logging and observability:
- [#9](https://github.com/trefbaltriggerbal/aspire/issues/9) Enable console logging
- [#10](https://github.com/trefbaltriggerbal/aspire/issues/10) Add structured logging
- [#11](https://github.com/trefbaltriggerbal/aspire/issues/11) Implement distributed tracing
- [#12](https://github.com/trefbaltriggerbal/aspire/issues/12) Add metrics
- [#16](https://github.com/trefbaltriggerbal/aspire/issues/16) Structured logs for console apps
