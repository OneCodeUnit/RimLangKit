using FluentAssertions;
using RimLangKit.Checks;

namespace RimLangKit.Tests.Unit.Checks;

public class XmlErrorCheckerTests
{
    [Fact]
    public void CheckXml_WithValidXml_ShouldReturnSuccess()
    {
        // Arrange
        var validXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <TestTag>Test Value</TestTag>
            </LanguageData>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, validXml);

        try
        {
            // Act
            var result = XmlErrorChecker.CheckXml(tempFile);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.ErrorMessage.Should().BeNullOrEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CheckXml_WithInvalidXml_ShouldReturnError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <TestTag>Test Value
            </LanguageData>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, invalidXml);

        try
        {
            // Act
            var result = XmlErrorChecker.CheckXml(tempFile);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CheckXml_WithMissingLanguageDataRoot_ShouldReturnError()
    {
        // Arrange
        var xmlWithoutRoot = """
            <?xml version="1.0" encoding="utf-8"?>
            <SomeOtherRoot>
                <TestTag>Test Value</TestTag>
            </SomeOtherRoot>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xmlWithoutRoot);

        try
        {
            // Act
            var result = XmlErrorChecker.CheckXml(tempFile);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.ErrorMessage.Should().Contain("LanguageData");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CheckXml_WithEmptyFile_ShouldReturnError()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, string.Empty);

        try
        {
            // Act
            var result = XmlErrorChecker.CheckXml(tempFile);

            // Assert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CheckXml_WithNonExistentFile_ShouldReturnError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");

        // Act
        var result = XmlErrorChecker.CheckXml(nonExistentFile);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }
}
