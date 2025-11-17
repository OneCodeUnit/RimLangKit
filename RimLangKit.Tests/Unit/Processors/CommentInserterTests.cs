using FluentAssertions;
using RimLangKit.Processors;
using System.Xml.Linq;

namespace RimLangKit.Tests.Unit.Processors;

public class CommentInserterTests
{
    [Fact]
    public void InsertComments_ValidXml_ShouldAddCommentsBeforeEachTag()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun_AssaultRifle.label>винтовка</Gun_AssaultRifle.label>
                <Gun_AssaultRifle.description>Военная штурмовая винтовка</Gun_AssaultRifle.description>
            </LanguageData>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = CommentInserter.InsertComments(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();

            var content = File.ReadAllText(tempFile);
            content.Should().Contain("<!-- EN: винтовка -->");
            content.Should().Contain("<!-- EN: Военная штурмовая винтовка -->");

            // Verify XML structure
            var doc = XDocument.Load(tempFile);
            var nodes = doc.Root.Nodes().ToList();

            // Should have comments before each element
            nodes.OfType<XComment>().Should().HaveCountGreaterOrEqualTo(2);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void InsertComments_InvalidXml_ShouldReturnError()
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
            var result = CommentInserter.InsertComments(tempFile);

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
    public void InsertComments_MissingLanguageDataRoot_ShouldReturnError()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <SomeOtherRoot>
                <Gun.label>винтовка</Gun.label>
            </SomeOtherRoot>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = CommentInserter.InsertComments(tempFile);

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
    public void InsertComments_EmptyElements_ShouldAddEmptyComments()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label></Gun.label>
                <Gun.description>Описание</Gun.description>
            </LanguageData>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = CommentInserter.InsertComments(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var content = File.ReadAllText(tempFile);
            content.Should().Contain("<!-- EN:  -->");
            content.Should().Contain("<!-- EN: Описание -->");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void InsertComments_NonExistentFile_ShouldReturnError()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");

        // Act
        var result = CommentInserter.InsertComments(nonExistentFile);

        // Assert
        result.Item1.Should().BeFalse();
        result.Item2.Should().NotBeEmpty();
    }

    [Fact]
    public void InsertComments_XmlWithSpecialCharacters_ShouldPreserveCharacters()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Test.label>Текст с &amp; специальными &lt;символами&gt;</Test.label>
            </LanguageData>
            """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, xml);

        try
        {
            // Act
            var result = CommentInserter.InsertComments(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var doc = XDocument.Load(tempFile);
            var element = doc.Root.Element(XName.Get("Test.label"));
            element.Should().NotBeNull();
            element.Value.Should().Contain("&");
            element.Value.Should().Contain("<");
            element.Value.Should().Contain(">");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
