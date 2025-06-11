using System.Collections.Generic;
using System.Threading;
using UiFlowRecorder.Extra;

namespace UiFlow.Tests.TestInfra;

public static class UiRunState
{
    public static RootNode Root { get; } = new();
    public static ScreenshotService Shots = default!;

    public static readonly AsyncLocal<JobNode?> CurrentJob = new();
    public static readonly AsyncLocal<string?> CurrentStep = new();

    public static readonly AsyncLocal<string?> CurrentStepBefore = new();
    public static readonly AsyncLocal<List<CheckResult>> CurrentStepChecks = new();

    // helper
    public static string Url(string md5) => $"{md5}.png";
}
