using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Play-mode tests run inside Unity's runtime loop.
// Each test yields at least one frame to let the DebugGUI singleton initialise.
public class DebugGUITests
{
    // Keys used across this fixture — cleaned up in TearDown
    const string GraphKey     = "test_graph";
    const string LogKey       = "test_log";
    const string ExportKey    = "test_export";

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Trigger singleton creation and allow Awake/Init to run
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        DebugGUI.RemoveGraph(GraphKey);
        DebugGUI.RemoveGraph(ExportKey);
        DebugGUI.RemovePersistent(LogKey);
        yield return null;
    }

    // ── Graph API ─────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator Graph_PushValues_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 0.0f));
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 0.5f));
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 1.0f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Graph_NegativeValues_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, -1.0f));
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, -999.9f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetGraphProperties_ConfiguresGraph_DoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
            DebugGUI.SetGraphProperties(GraphKey, "Test", 0f, 100f, 0, Color.green, false));
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 50f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetGraphProperties_AutoScale_DoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
            DebugGUI.SetGraphProperties(GraphKey, "AutoScale", -1f, 1f, 0, Color.cyan, true));
        // Push a value outside the initial range — autoscale should absorb it
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 999f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RemoveGraph_ExistingKey_DoesNotThrow()
    {
        DebugGUI.Graph(GraphKey, 1f);
        yield return null;
        Assert.DoesNotThrow(() => DebugGUI.RemoveGraph(GraphKey));
    }

    [UnityTest]
    public IEnumerator RemoveGraph_NonExistentKey_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.RemoveGraph("__nonexistent_key__"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ClearGraph_ResetsData_DoesNotThrow()
    {
        DebugGUI.Graph(GraphKey, 1f);
        DebugGUI.Graph(GraphKey, 2f);
        Assert.DoesNotThrow(() => DebugGUI.ClearGraph(GraphKey));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ClearGraph_NonExistentKey_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.ClearGraph("__nonexistent_key__"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator MultipleGraphGroups_DoNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            DebugGUI.SetGraphProperties("grp0", "G0", 0f, 1f, 0, Color.red,   false);
            DebugGUI.SetGraphProperties("grp1", "G1", 0f, 1f, 1, Color.blue,  false);
            DebugGUI.SetGraphProperties("grp2", "G2", 0f, 1f, 2, Color.green, false);
            DebugGUI.Graph("grp0", 0.3f);
            DebugGUI.Graph("grp1", 0.6f);
            DebugGUI.Graph("grp2", 0.9f);
        });
        yield return null;
        DebugGUI.RemoveGraph("grp0");
        DebugGUI.RemoveGraph("grp1");
        DebugGUI.RemoveGraph("grp2");
    }

    // ── Log API ───────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator LogPersistent_AddsMessage_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.LogPersistent(LogKey, "Hello"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator LogPersistent_UpdatesExistingKey_DoesNotThrow()
    {
        DebugGUI.LogPersistent(LogKey, "First");
        Assert.DoesNotThrow(() => DebugGUI.LogPersistent(LogKey, "Second"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RemovePersistent_ExistingKey_DoesNotThrow()
    {
        DebugGUI.LogPersistent(LogKey, "To remove");
        Assert.DoesNotThrow(() => DebugGUI.RemovePersistent(LogKey));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RemovePersistent_NonExistentKey_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.RemovePersistent("__nonexistent_log__"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ClearPersistent_RemovesAll_DoesNotThrow()
    {
        DebugGUI.LogPersistent("k1", "a");
        DebugGUI.LogPersistent("k2", "b");
        Assert.DoesNotThrow(() => DebugGUI.ClearPersistent());
        yield return null;
    }

    [UnityTest]
    public IEnumerator Log_TransientMessage_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.Log("transient test message"));
        yield return null;
    }

    // ── Export ────────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator ExportGraphs_WithData_ReturnsNonNullPath()
    {
        DebugGUI.Graph(ExportKey, 1.0f);
        DebugGUI.Graph(ExportKey, 2.0f);
        yield return null;

        string path = DebugGUI.ExportGraphs();
        Assert.IsNotNull(path, "ExportGraphs() should return a file path, not null");

        Cleanup(path);
    }

    [UnityTest]
    public IEnumerator ExportGraphs_WithData_FileExistsOnDisk()
    {
        DebugGUI.Graph(ExportKey, 42f);
        yield return null;

        string path = DebugGUI.ExportGraphs();
        Assert.IsNotNull(path);
        Assert.IsTrue(File.Exists(path), $"Expected file at '{path}' to exist");

        Cleanup(path);
    }

    [UnityTest]
    public IEnumerator ExportGraphs_ProducesValidJson()
    {
        DebugGUI.Graph(ExportKey, 3.14f);
        DebugGUI.Graph(ExportKey, 2.72f);
        yield return null;

        string path = DebugGUI.ExportGraphs();
        Assert.IsNotNull(path);
        Assert.IsTrue(File.Exists(path));

        string json = File.ReadAllText(path);
        Assert.IsNotEmpty(json, "Exported JSON should not be empty");
        Assert.IsTrue(json.TrimStart().StartsWith("{"),
            "JSON should be an object (start with '{')");
        Assert.IsTrue(json.Contains("\"data\""),
            "JSON should contain a 'data' field");

        Cleanup(path);
    }

    [UnityTest]
    public IEnumerator ExportGraphs_FilenameMatchesExpectedPattern()
    {
        DebugGUI.Graph(ExportKey, 0f);
        yield return null;

        string path = DebugGUI.ExportGraphs();
        Assert.IsNotNull(path);

        string filename = Path.GetFileName(path);
        bool ok = Regex.IsMatch(filename,
            @"^debuggui_graph_export_\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}\.json$");
        Assert.IsTrue(ok,
            $"Filename '{filename}' does not match expected pattern " +
            "'debuggui_graph_export_yyyy-MM-ddTHH-mm-ss.json'");

        Cleanup(path);
    }

    // ── Enable / Disable ─────────────────────────────────────────────

    [UnityTest]
    public IEnumerator SetEnabled_False_HidesBothWindows()
    {
        DebugGUI.SetEnabled(false);
        yield return null;

        var graph = GameObject.Find("Graph");
        var log = GameObject.Find("Log");
        Assert.IsNotNull(graph, "Graph GameObject should still exist");
        Assert.IsNotNull(log, "Log GameObject should still exist");
        Assert.IsFalse(graph.activeSelf, "Graph should be inactive");
        Assert.IsFalse(log.activeSelf, "Log should be inactive");

        // Restore
        DebugGUI.SetEnabled(true);
    }

    [UnityTest]
    public IEnumerator SetEnabled_True_ShowsBothWindows()
    {
        DebugGUI.SetEnabled(false);
        yield return null;
        DebugGUI.SetEnabled(true);
        yield return null;

        var graph = GameObject.Find("Graph");
        var log = GameObject.Find("Log");
        Assert.IsTrue(graph.activeSelf, "Graph should be active");
        Assert.IsTrue(log.activeSelf, "Log should be active");
    }

    [UnityTest]
    public IEnumerator SetGraphsEnabled_TogglesIndependently()
    {
        DebugGUI.SetGraphsEnabled(false);
        yield return null;

        var graph = GameObject.Find("Graph");
        var log = GameObject.Find("Log");
        Assert.IsFalse(graph.activeSelf, "Graph should be inactive");
        Assert.IsTrue(log.activeSelf, "Log should remain active");

        DebugGUI.SetGraphsEnabled(true);
    }

    [UnityTest]
    public IEnumerator SetLogsEnabled_TogglesIndependently()
    {
        DebugGUI.SetLogsEnabled(false);
        yield return null;

        var graph = GameObject.Find("Graph");
        var log = GameObject.Find("Log");
        Assert.IsTrue(graph.activeSelf, "Graph should remain active");
        Assert.IsFalse(log.activeSelf, "Log should be inactive");

        DebugGUI.SetLogsEnabled(true);
    }

    [UnityTest]
    public IEnumerator Graph_WhileDisabled_DoesNotThrow()
    {
        DebugGUI.SetGraphsEnabled(false);
        yield return null;

        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 1f));
        Assert.DoesNotThrow(() => DebugGUI.SetGraphProperties(
            GraphKey, "Test", 0f, 1f, 0, Color.red, false));
        Assert.DoesNotThrow(() => DebugGUI.ClearGraph(GraphKey));
        Assert.DoesNotThrow(() => DebugGUI.RemoveGraph(GraphKey));

        DebugGUI.SetGraphsEnabled(true);
    }

    [UnityTest]
    public IEnumerator Log_WhileDisabled_DoesNotThrow()
    {
        DebugGUI.SetLogsEnabled(false);
        yield return null;

        Assert.DoesNotThrow(() => DebugGUI.Log("test"));
        Assert.DoesNotThrow(() => DebugGUI.LogPersistent(LogKey, "test"));
        Assert.DoesNotThrow(() => DebugGUI.RemovePersistent(LogKey));
        Assert.DoesNotThrow(() => DebugGUI.ClearPersistent());

        DebugGUI.SetLogsEnabled(true);
    }

    [UnityTest]
    public IEnumerator SetEnabled_RoundTrip_RestoresState()
    {
        // Disable, push data, re-enable — should not throw
        DebugGUI.SetEnabled(false);
        DebugGUI.Graph(GraphKey, 5f);
        DebugGUI.LogPersistent(LogKey, "hidden");
        yield return null;

        DebugGUI.SetEnabled(true);
        yield return null;

        // Verify windows are back
        Assert.IsTrue(GameObject.Find("Graph").activeSelf);
        Assert.IsTrue(GameObject.Find("Log").activeSelf);

        // Data pushed while disabled should still be queryable
        Assert.DoesNotThrow(() => DebugGUI.Graph(GraphKey, 10f));
    }

    // ── Misc ──────────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator ForceReinitializeAttributes_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => DebugGUI.ForceReinitializeAttributes());
        yield return null;
    }

    // ─────────────────────────────────────────────────────────────────

    static void Cleanup(string path)
    {
        if (path != null && File.Exists(path))
            File.Delete(path);
    }
}
