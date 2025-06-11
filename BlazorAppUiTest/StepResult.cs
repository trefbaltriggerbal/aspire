public sealed record StepResult(
    string FlowName,
    string Label,
    bool Success,
    string Md5,
    List<string> FailedChecks
);
