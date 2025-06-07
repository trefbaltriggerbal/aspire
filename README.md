# Aspire Demo

This repository contains a minimal .NET 8 sample that demonstrates how to use the preview of **.NET Aspire** to orchestrate multiple services. The solution consists of:

- `Sender` – console app that sends an HTTP request to `Receiver`.
- `Receiver` – console app that hosts a small HTTP server and responds with `Pong`.
- `WebFrontend` – Blazor Web App that can trigger the `Sender` project.
- `AspireHost` – console project that will host the distributed application using the `Aspire.Hosting` package.

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

The sender will call `http://localhost:5000/ping` hosted by the receiver and print the response.

## Running with Aspire

The `AspireHost` project references the `Aspire.Hosting` package. When executed it will start the other projects and manage them as a distributed application. Run it with:

```bash
dotnet run --project AspireHost
```

This will launch the receiver and the web frontend as part of the distributed app. The sender project can be triggered from the web frontend or run separately.

## Next steps

This repository serves as a starting point for experimenting with .NET Aspire. Feel free to extend the projects, add more services or containers and explore the orchestration capabilities provided by Aspire.

## Planned improvements

The following GitHub issues track upcoming features for logging and observability:
- [#9](https://github.com/trefbaltriggerbal/aspire/issues/9) Enable console logging
- [#10](https://github.com/trefbaltriggerbal/aspire/issues/10) Add structured logging
- [#11](https://github.com/trefbaltriggerbal/aspire/issues/11) Implement distributed tracing
- [#12](https://github.com/trefbaltriggerbal/aspire/issues/12) Add metrics
