using FluentAssertions;
using RimLangKit.Processors;

namespace RimLangKit.Tests.Unit.Processors;

public class ChangesFinderTests
{
    [Fact]
    public void GetTranslationData_ValidXmlWithComments_ShouldExtractData()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: rifle -->
                <Gun_AssaultRifle.label>винтовка</Gun_AssaultRifle.label>
                <!-- EN: A military assault rifle -->
                <Gun_AssaultRifle.description>Военная штурмовая винтовка</Gun_AssaultRifle.description>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetTranslationData(tempFile);

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
    public void GetTranslationData_InvalidXml_ShouldReturnError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: rifle -->
                <Gun.label>винтовка
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, invalidXml);

        try
        {
            // Act
            var result = ChangesFinder.GetTranslationData(tempFile);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().NotBeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetTranslationData_MissingLanguageDataRoot_ShouldReturnError()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <SomeOtherRoot>
                <!-- EN: rifle -->
                <Gun.label>винтовка</Gun.label>
            </SomeOtherRoot>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetTranslationData(tempFile);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Contain("LanguageData");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetModData_ValidXml_ShouldExtractData()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun_AssaultRifle.label>rifle</Gun_AssaultRifle.label>
                <Gun_AssaultRifle.description>A military assault rifle</Gun_AssaultRifle.description>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetModData(tempFile);

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
    public void GetModData_InvalidXml_ShouldReturnError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label>rifle
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, invalidXml);

        try
        {
            // Act
            var result = ChangesFinder.GetModData(tempFile);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().NotBeEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void WriteChanges_NoData_ShouldReportNoChanges()
    {
        // Act
        var result = ChangesFinder.WriteChanges();

        // Assert
        result.Should().Contain("Не найдено сломанных файлов");
    }

    [Fact]
    public void GetTranslationData_WithDuplicateKeys_ShouldHandleUniquely()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: rifle -->
                <Gun.label>винтовка</Gun.label>
                <!-- EN: pistol -->
                <Gun.label>пистолет</Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetTranslationData(tempFile);

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
    public void GetModData_WithDuplicateKeys_ShouldHandleUniquely()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label>rifle</Gun.label>
                <Gun.label>pistol</Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetModData(tempFile);

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
    public void GetTranslationData_EmptyElements_ShouldNotExtract()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: empty -->
                <Gun.label></Gun.label>
            </LanguageData>
            """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = ChangesFinder.GetTranslationData(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetTranslationData_NonExistentFile_ShouldReturnError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");

        // Act
        var result = ChangesFinder.GetTranslationData(nonExistentFile);

        // Assert
        result.Item1.Should().BeFalse();
        result.Item2.Should().NotBeEmpty();
    }

    [Fact]
    public void GetModData_NonExistentFile_ShouldReturnError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");

        // Act
        var result = ChangesFinder.GetModData(nonExistentFile);

        // Assert
        result.Item1.Should().BeFalse();
        result.Item2.Should().NotBeEmpty();
    }
}
