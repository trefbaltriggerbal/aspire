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
