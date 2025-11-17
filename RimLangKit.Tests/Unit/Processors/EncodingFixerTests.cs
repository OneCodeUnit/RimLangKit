using FluentAssertions;
using RimLangKit.Processors;
using System.Text;

namespace RimLangKit.Tests.Unit.Processors;

public class EncodingFixerTests
{
    [Fact]
    public void EncodingFixerActivity_ShouldConvertTabsToSpaces()
    {
        // Arrange
        var content = "Line1\tLine2\t\tLine3";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();

            var fixedContent = File.ReadAllText(tempFile);
            fixedContent.Should().NotContain("\t");
            fixedContent.Should().Contain("  "); // Two spaces
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_ShouldConvertLineEndingsToCRLF()
    {
        // Arrange
        var content = "Line1\nLine2\nLine3";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var bytes = File.ReadAllBytes(tempFile);
            var fixedContent = Encoding.UTF8.GetString(bytes);

            // Should have CRLF line endings
            fixedContent.Should().Contain("\r\n");
            // Should not have standalone LF
            fixedContent.Replace("\r\n", "").Should().NotContain("\n");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_ShouldAddUTF8BOM()
    {
        // Arrange
        var content = "Test content without BOM";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content, new UTF8Encoding(false)); // Without BOM

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var bytes = File.ReadAllBytes(tempFile);
            var utf8Bom = Encoding.UTF8.GetPreamble();

            // Should start with UTF-8 BOM
            bytes.Take(utf8Bom.Length).Should().Equal(utf8Bom);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_CompleteXmlFile_ShouldFixAllIssues()
    {
        // Arrange
        var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<LanguageData>\n\t<Gun.label>винтовка</Gun.label>\n\t<Gun.description>Описание</Gun.description>\n</LanguageData>";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml, new UTF8Encoding(false));

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var bytes = File.ReadAllBytes(tempFile);
            var utf8Bom = Encoding.UTF8.GetPreamble();
            bytes.Take(utf8Bom.Length).Should().Equal(utf8Bom);

            var content = File.ReadAllText(tempFile);
            content.Should().NotContain("\t");
            content.Should().Contain("\r\n");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_EmptyFile_ShouldAddBOM()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, string.Empty);

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var bytes = File.ReadAllBytes(tempFile);
            var utf8Bom = Encoding.UTF8.GetPreamble();
            bytes.Take(utf8Bom.Length).Should().Equal(utf8Bom);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_FileAlreadyWithBOM_ShouldNotDuplicateBOM()
    {
        // Arrange
        var content = "Test content with BOM";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content, new UTF8Encoding(true)); // With BOM

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var bytes = File.ReadAllBytes(tempFile);
            var utf8Bom = Encoding.UTF8.GetPreamble();

            // Should start with UTF-8 BOM (not duplicated)
            bytes.Take(utf8Bom.Length).Should().Equal(utf8Bom);

            // Verify content is preserved
            var text = File.ReadAllText(tempFile);
            text.Should().Contain("Test content with BOM");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_MultipleTabsInRow_ShouldReplaceEachWithTwoSpaces()
    {
        // Arrange
        var content = "Text\t\t\tMore";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var fixedContent = File.ReadAllText(tempFile);
            fixedContent.Should().NotContain("\t");
            fixedContent.Should().Contain("Text  More"); // Each tab becomes 2 spaces
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void EncodingFixerActivity_RussianText_ShouldPreserveEncoding()
    {
        // Arrange
        var russianText = "Привет, мир! Это тест кириллицы.";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, russianText);

        try
        {
            // Act
            var result = EncodingFixer.EncodingFixerActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var fixedContent = File.ReadAllText(tempFile);
            fixedContent.Should().Contain("Привет, мир!");
            fixedContent.Should().Contain("кириллицы");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
