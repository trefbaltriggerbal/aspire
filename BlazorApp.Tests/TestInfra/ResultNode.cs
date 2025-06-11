using System.Collections.Generic;

namespace UiFlow.Tests.TestInfra;

public abstract class ResultNode
{
    public string Name { get; }
    public bool Success { get; private set; } = true;
    public ResultNode? Parent { get; set; }
    public List<ResultNode> Children { get; } = new();

    protected ResultNode(string name) => Name = name;
    public void Add(ResultNode n) { n.Parent = this; Children.Add(n); }
    public void Fail() { Success = false; Parent?.Fail(); }
}

public sealed class RootNode : ResultNode { public RootNode() : base("Root") { } }
public sealed class GroupNode : ResultNode { public GroupNode(string n) : base(n) { } }
public sealed class JobNode : ResultNode { public JobNode(string n) : base(n) { } }

public sealed class StepNode : ResultNode
{
    public string Md5Before { get; }
    public string Md5After { get; }
    public List<CheckResult> Checks { get; } = new();

    public StepNode(string name, string md5After, string? md5Before = null)
        : base(name) { Md5Before = md5Before ?? ""; Md5After = md5After; }
}

public record CheckResult(string Name, bool Success, string? Message);
