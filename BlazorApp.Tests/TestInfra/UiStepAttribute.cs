using System;
using System.Collections.Generic;
using System.Linq; // voor OfType<>
using System.Reflection;
using UiFlow.Tests.TestInfra;
using Xunit.Sdk;


public sealed class UiStepAttribute : BeforeAfterTestAttribute
{
    private readonly string _group;
    private readonly string _job;
    private readonly string _step;

    public UiStepAttribute(string group, string job, string step)
        => (_group, _job, _step) = (group, job, step);

    public override void Before(MethodInfo methodUnderTest)
    {
        var g = UiRunState.Root.Children
                 .OfType<GroupNode>()
                 .FirstOrDefault(x => x.Name == _group)
             ?? new GroupNode(_group) { Parent = UiRunState.Root };

        if (!UiRunState.Root.Children.Contains(g))
            UiRunState.Root.Add(g);

        var j = g.Children
                 .OfType<JobNode>()
                 .FirstOrDefault(x => x.Name == _job)
             ?? new JobNode(_job) { Parent = g };

        if (!g.Children.Contains(j))
            g.Add(j);

        UiRunState.CurrentJob.Value = j;
        UiRunState.CurrentStep.Value = _step;

        try
        {
            var md5Before = UiRunState.Shots.Capture($"{_job}_{_step}_before");
            UiRunState.CurrentStepBefore.Value = md5Before;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Screenshot vóór stap mislukt: {ex.Message}");
        }
    }

    public override void After(MethodInfo _)
    {
        try
        {
            // 1. Screenshot ná de UI-actie
            var md5After = UiRunState.Shots.Capture($"{_job}_{_step}_after");

            /* 2. Verzamel alle checks die tijdens de stap zijn toegevoegd
                   (UiCheckAttribute of eigen code kan aan CurrentStepChecks toevoegen) */
            var checks = UiRunState.CurrentStepChecks.Value ?? new List<CheckResult>();

            /* 3. Maak de Step-node en koppel hem aan de huidige Job */
            var node = new StepNode(
                name: _step,
                md5After: md5After,
                md5Before: UiRunState.CurrentStepBefore.Value // kan null/"" zijn
            );

            node.Checks.AddRange(checks);
            UiRunState.CurrentJob.Value!.Add(node);

            // 4. Reset voor de volgende stap
            checks.Clear();
            UiRunState.CurrentStepBefore.Value = null;
        }
        catch
        {
            // Fout bubbelt door naar Job & hoger
            UiRunState.CurrentJob.Value?.Fail();
            throw;
        }
    }

}
