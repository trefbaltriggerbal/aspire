using Projects.Codex.CLI;

var runArgs = args.Length == 0 ? new[] { "--count", "1" } : args;
CliApp.Run(runArgs);
