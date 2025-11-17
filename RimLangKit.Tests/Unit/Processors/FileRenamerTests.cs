using FluentAssertions;
using RimLangKit.Processors;

namespace RimLangKit.Tests.Unit.Processors;

public class FileRenamerTests
{
    [Fact]
    public void FileRenamerActivity_ValidPath_ShouldRenameFileWithModuleName()
    {
        // Arrange - Create structure: ModuleName\Languages\Russian\DefInjected\ThingDef\file.xml
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var moduleName = "TestModule";
        var filePath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", "Weapons.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><LanguageData></LanguageData>");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();

            // Original file should not exist
            File.Exists(filePath).Should().BeFalse();

            // New file should exist with module name appended
            var expectedNewPath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", $"Weapons_{moduleName}.xml");
            File.Exists(expectedNewPath).Should().BeTrue();
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
    public void FileRenamerActivity_NonXmlFile_ShouldReturnError()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var moduleName = "TestModule";
        var filePath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", "file.txt");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, "test");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().NotBeEmpty();
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
    public void FileRenamerActivity_PathTooShort_ShouldReturnError()
    {
        // Arrange - Path with less than required depth
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "file.xml");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "test");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Contain("Папка модуля не найдена");
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
    public void FileRenamerActivity_DifferentModuleNames_ShouldUseCorrectName()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var moduleName = "CoreGame";
        var filePath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", "Items.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><LanguageData></LanguageData>");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeTrue();

            var expectedNewPath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", $"Items_{moduleName}.xml");
            File.Exists(expectedNewPath).Should().BeTrue();
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
    public void FileRenamerActivity_FileWithUppercaseExtension_ShouldWork()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var moduleName = "TestMod";
        var filePath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", "Data.XML");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><LanguageData></LanguageData>");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeTrue();

            var expectedNewPath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", $"Data_{moduleName}.XML");
            File.Exists(expectedNewPath).Should().BeTrue();
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
    public void FileRenamerActivity_FileNameWithSpaces_ShouldRenameCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var moduleName = "My Module";
        var filePath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", "Some File.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><LanguageData></LanguageData>");

        try
        {
            // Act
            var result = FileRenamer.FileRenamerActivity(filePath);

            // Assert
            result.Item1.Should().BeTrue();

            var expectedNewPath = Path.Combine(tempDir, moduleName, "Languages", "Russian", "DefInjected", "ThingDef", $"Some File_{moduleName}.xml");
            File.Exists(expectedNewPath).Should().BeTrue();
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
    public void FileRenamerActivity_NonExistentFile_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = @"C:\NonExistent\Module\Languages\Russian\DefInjected\ThingDef\file.xml";

        // Act
        Action act = () => FileRenamer.FileRenamerActivity(nonExistentPath);

        // Assert
        act.Should().Throw<Exception>();
    }
}
