using RimLangKit.Models;

namespace RimLangKit.Tests.Helpers;

/// <summary>
/// Builder class for creating test data
/// </summary>
public static class TestDataBuilder
{
    /// <summary>
    /// Creates a sample RimTag with default values
    /// </summary>
    public static RimTag CreateSampleTag(
        string defName = "TestDef",
        string tagType = "label",
        string original = "Test Original",
        string translation = "Тестовый перевод",
        string comment = "")
    {
        return new RimTag(defName, tagType, original, translation, comment);
    }

    /// <summary>
    /// Creates a list of sample tags for testing
    /// </summary>
    public static List<RimTag> CreateSampleTags(int count = 5)
    {
        var tags = new List<RimTag>();
        for (int i = 1; i <= count; i++)
        {
            tags.Add(new RimTag(
                defName: $"TestDef{i}",
                tagType: i % 2 == 0 ? "description" : "label",
                original: $"Original Text {i}",
                translation: $"Переведенный текст {i}"
            ));
        }
        return tags;
    }

    /// <summary>
    /// Creates a valid XML string for LanguageData
    /// </summary>
    public static string CreateValidXml(params (string tag, string value)[] elements)
    {
        var content = string.Join("\n    ", elements.Select(e => $"<{e.tag}>{e.value}</{e.tag}>"));
        return $"""
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                {content}
            </LanguageData>
            """;
    }

    /// <summary>
    /// Creates an XML file in a temporary location
    /// </summary>
    public static string CreateTempXmlFile(string content)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);
        return tempFile;
    }

    /// <summary>
    /// Creates a temporary directory for testing
    /// </summary>
    public static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    /// <summary>
    /// Cleans up a temporary file
    /// </summary>
    public static void CleanupFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Cleans up a temporary directory
    /// </summary>
    public static void CleanupDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, recursive: true);
        }
    }
}
