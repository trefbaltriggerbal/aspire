using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UiFlowRecorder.Extra;
using Xunit;    // ← PersistLayer

namespace UiFlow.Tests.TestInfra;

[CollectionDefinition("Global")]
public class GlobalTestCollection : ICollectionFixture<ReportWriter> { }

public sealed class ReportWriter : IDisposable
{
    public void Dispose()
    {
        var map = PersistLayer.LoadMapping();
        var md = BuildMarkdown(UiRunState.Root, map);
        File.WriteAllText("UiReport.md", md, Encoding.UTF8);
        Console.WriteLine("📝 UiReport.md geschreven.");
    }

    /* ---------- Markdown ---------- */

    private static string BuildMarkdown(ResultNode root,
                                        IReadOnlyDictionary<string, string> urlMap)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🧪 UI Flow Verslag\n");
        sb.AppendLine(root.Success ? "**✅ Alles geslaagd**\n"
                                   : "**❌ Er waren fouten**\n");

        Recurse(root, 2, sb, urlMap);
        return sb.ToString();
    }

    private static void Recurse(ResultNode n, int depth,
                                StringBuilder sb,
                                IReadOnlyDictionary<string, string> urlMap)
    {
        string header = new string('#', depth);
        string icon = n.Success ? "✅" : "❌";
        sb.AppendLine($"{header} {icon} {n.Name}\n");

        if (n is StepNode s)
        {
            if (!string.IsNullOrWhiteSpace(s.Md5Before) &&
                urlMap.TryGetValue(s.Md5Before, out var beforeUrl))
                sb.AppendLine($"**Voor:**\n![before]({beforeUrl})\n");

            if (urlMap.TryGetValue(s.Md5After, out var afterUrl))
                sb.AppendLine($"**Na:**\n![after]({afterUrl})\n");

            foreach (var ck in s.Checks.Where(c => !c.Success))
                sb.AppendLine($"* ❌ **{ck.Name}** – {ck.Message}");

            sb.AppendLine();
        }

        foreach (var child in n.Children)
            Recurse(child, depth + 1, sb, urlMap);
    }
}
