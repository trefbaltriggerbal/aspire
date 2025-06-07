# .NET Aspire Tutorial

This short guide shows how the sample projects work together and how to view observability data using the Aspire dashboard.

## Projects

- **AspireApp.AppHost** – launches the Sender, Receiver and WebFrontend projects as a distributed application.
- **Sender** – console app that sends a `GET /ping` request to Receiver.
- **Receiver** – lightweight HTTP listener that responds with `Pong`.
- **WebFrontend** – Blazor web app that can trigger Sender.

## Running the distributed app

1. Ensure the Aspire workload is installed:

   ```bash
   dotnet workload install aspire
   ```
2. Build the solution:

   ```bash
   dotnet build AspireDemo.sln
   ```
3. Launch the distributed application:

   ```bash
   dotnet run --project AspireApp/AspireApp.AppHost
   ```

This starts all services and shows the Aspire dashboard on [http://localhost:18888](http://localhost:18888).

## Observing logs and traces

Open the dashboard and select a trace to see the spans produced by Sender and Receiver. The `AddServiceDefaults` extension configures OpenTelemetry so that each log written within an `Activity` automatically carries the current trace context and is grouped with its span.

Because Sender uses `HttpClient` with OpenTelemetry instrumentation, each request includes the `traceparent` header. The Receiver extracts this header and starts its span with the same TraceId, letting you follow a request across both services.

Use the Blazor WebFrontend to trigger the Sender, or run the console apps individually with `dotnet run --project Sender` and `dotnet run --project Receiver`.

The tutorial demonstrates how Aspire combines service orchestration with OpenTelemetry to collect logs and distributed traces out of the box.
