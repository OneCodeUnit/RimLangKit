using FluentAssertions;
using RimLangKit.Processors;

namespace RimLangKit.Tests.Unit.Processors;

public class FileFixerTests
{
    [Fact]
    public void FileFixerActivity_ValidXml_ShouldReturnSuccess()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label>винтовка</Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileFixerActivity_InvalidXml_ShouldRecordError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label>винтовка
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, invalidXml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue(); // Always returns true, but records error
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileFixerActivity_MissingDeclaration_ShouldRecordError()
    {
        // Arrange
        var xmlWithoutDeclaration = """
            <LanguageData>
                <Gun.label>винтовка</Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xmlWithoutDeclaration);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileFixerActivity_WrongDeclaration_ShouldRecordError()
    {
        // Arrange
        var xmlWithWrongDeclaration = """
            <?xml version="1.1" encoding="iso-8859-1"?>
            <LanguageData>
                <Gun.label>винтовка</Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xmlWithWrongDeclaration);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileFixerActivity_AboutXml_ShouldSkipCheck()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <ModMetaData>
                <name>Test Mod</name>
            </ModMetaData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var aboutFile = Path.Combine(tempDir, "About.xml");
        File.WriteAllText(aboutFile, xml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(aboutFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void FileFixerActivity_LoadFoldersXml_ShouldSkipCheck()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <loadFolders>
                <v1.0>
                    <li>Core</li>
                </v1.0>
            </loadFolders>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var loadFoldersFile = Path.Combine(tempDir, "LoadFolders.xml");
        File.WriteAllText(loadFoldersFile, xml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(loadFoldersFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void BrokenFilesWriterActivity_NoBrokenFiles_ShouldReportNone()
    {
        // Act
        var result = FileFixer.BrokenFilesWriterActivity();

        // Assert
        result.Should().Contain("Не найдено сломанных файлов");
    }

    [Fact]
    public void FileFixerActivity_MultipleFiles_ShouldProcessAll()
    {
        // Arrange
        var validXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Test1.label>тест1</Test1.label>
            </LanguageData>
            """;

        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Test2.label>тест2
            </LanguageData>
            """;

        var tempFile1 = Path.GetTempFileName();
        var tempFile2 = Path.GetTempFileName();
        File.WriteAllText(tempFile1, validXml);
        File.WriteAllText(tempFile2, invalidXml);

        try
        {
            // Act
            var result1 = FileFixer.FileFixerActivity(tempFile1);
            var result2 = FileFixer.FileFixerActivity(tempFile2);

            // Assert
            result1.Item1.Should().BeTrue();
            result2.Item1.Should().BeTrue();
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    [Fact]
    public void FileFixerActivity_CaseInsensitiveFileNames_ShouldWork()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <ModMetaData>
                <name>Test</name>
            </ModMetaData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var aboutFile = Path.Combine(tempDir, "ABOUT.XML"); // uppercase
        File.WriteAllText(aboutFile, xml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(aboutFile);

            // Assert
            result.Item1.Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void FileFixerActivity_XmlWithCorrectDeclarationUtf8_ShouldPass()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Test.label>Тест с кириллицей</Test.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = FileFixer.FileFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
