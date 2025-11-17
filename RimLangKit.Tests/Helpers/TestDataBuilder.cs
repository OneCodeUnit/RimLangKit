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
        string tagDef = "TestDef",
        string tagText = "Тестовый перевод",
        string tagType = "label",
        string tagComment = "")
    {
        if (string.IsNullOrEmpty(tagComment))
        {
            return new RimTag(tagDef, tagText, tagType);
        }
        return new RimTag(tagDef, tagText, tagComment, tagType);
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
                tagDef: $"TestDef{i}",
                tagText: $"Переведенный текст {i}",
                tagComment: $"Original Text {i}",
                tagType: i % 2 == 0 ? "description" : "label"
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
    /// Creates a valid XML string with comments for LanguageData
    /// </summary>
    public static string CreateValidXmlWithComments(params (string tag, string comment, string value)[] elements)
    {
        var content = string.Join("\n    ", elements.Select(e => $"<!-- EN: {e.comment} -->\n    <{e.tag}>{e.value}</{e.tag}>"));
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
    /// Creates a directory structure for RimWorld mod testing
    /// Example: ModName\Languages\Russian\DefInjected\ThingDef\
    /// </summary>
    public static string CreateModDirectoryStructure(string moduleName = "TestMod", string defType = "ThingDef")
    {
        var tempDir = CreateTempDirectory();
        var fullPath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", defType);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    /// <summary>
    /// Cleans up a temporary file
    /// </summary>
    public static void CleanupFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }

    /// <summary>
    /// Cleans up a temporary directory
    /// </summary>
    public static void CleanupDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            try
            {
                Directory.Delete(directoryPath, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
}
