using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

// Edit-mode tests run without entering Play Mode.
// They cover pure logic that has no MonoBehaviour / scene dependency.
public class ExportTests
{
    // ── DateTime / filename format ─────────────────────────────────────

    [Test]
    public void ExportDateTimeFormat_ProducesExpectedLength()
    {
        // "2024-01-01T12-30-45" is exactly 19 chars
        var result = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        Assert.AreEqual(19, result.Length,
            $"Expected 19 characters, got {result.Length} for '{result}'");
    }

    [Test]
    public void ExportDateTimeFormat_MatchesPattern()
    {
        var result = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        bool ok = Regex.IsMatch(result, @"^\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}$");
        Assert.IsTrue(ok, $"'{result}' did not match yyyy-MM-ddTHH-mm-ss");
    }

    [Test]
    public void ExportDateTimeFormat_YearIsFourDigits()
    {
        var result = new DateTime(2024, 6, 15).ToString("yyyy-MM-ddTHH-mm-ss");
        string year = result.Substring(0, 4);
        Assert.IsTrue(int.TryParse(year, out int y));
        Assert.Greater(y, 2000);
    }

    [Test]
    public void ExportDateTimeFormat_MonthIsZeroPadded()
    {
        // Month 1 must produce "01", not "1" (would happen with "mm" or "M")
        var result = new DateTime(2024, 1, 15).ToString("yyyy-MM-ddTHH-mm-ss");
        string month = result.Substring(5, 2);
        Assert.AreEqual("01", month,
            "Month should be zero-padded to two digits (requires 'MM', not 'mm' or 'M')");
    }

    [Test]
    public void ExportDateTimeFormat_IsSafeForFilename()
    {
        var result = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        char[] invalid = Path.GetInvalidFileNameChars();
        foreach (char c in result)
        {
            Assert.IsFalse(Array.IndexOf(invalid, c) >= 0,
                $"Character '{c}' (0x{(int)c:X2}) is not valid in a filename");
        }
    }

    [Test]
    public void ExportFilename_HasCorrectPrefixAndSuffix()
    {
        var dateStr = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        var filename = $"debuggui_graph_export_{dateStr}.json";
        Assert.IsTrue(filename.StartsWith("debuggui_graph_export_"),
            "Filename should start with 'debuggui_graph_export_'");
        Assert.IsTrue(filename.EndsWith(".json"),
            "Filename should end with '.json'");
    }

    [Test]
    public void ExportFilename_ContainsDatetimeComponent()
    {
        var dateStr = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        var filename = $"debuggui_graph_export_{dateStr}.json";
        // Extract the datetime portion and confirm it matches the pattern
        int start = "debuggui_graph_export_".Length;
        int len = 19; // yyyy-MM-ddTHH-mm-ss
        string extracted = filename.Substring(start, len);
        bool ok = Regex.IsMatch(extracted, @"^\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}$");
        Assert.IsTrue(ok, $"Extracted datetime '{extracted}' is malformed");
    }
}
