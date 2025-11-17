using FluentAssertions;
using RimLangKit.Models;

namespace RimLangKit.Tests.Unit.Models;

public class RimTagTests
{
    [Fact]
    public void Constructor_WithoutComment_ShouldCreateTag()
    {
        // Act
        var tag = new RimTag("TestDef", "Translation", "label");

        // Assert
        tag.TagDef.Should().Be("TestDef");
        tag.TagText.Should().Be("Translation");
        tag.TagType.Should().Be("label");
        tag.TagComment.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithComment_ShouldCreateTagWithComment()
    {
        // Act
        var tag = new RimTag("TestDef", "Translation", "Test Comment", "description");

        // Assert
        tag.TagDef.Should().Be("TestDef");
        tag.TagText.Should().Be("Translation");
        tag.TagComment.Should().Be("Test Comment");
        tag.TagType.Should().Be("description");
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyTag()
    {
        // Act
        var tag = new RimTag();

        // Assert
        tag.TagDef.Should().BeEmpty();
        tag.TagText.Should().BeEmpty();
        tag.TagComment.Should().BeEmpty();
        tag.TagType.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var tag = new RimTag();

        // Act
        tag.TagDef = "NewDef";
        tag.TagText = "New Translation";
        tag.TagComment = "New Comment";
        tag.TagType = "label";

        // Assert
        tag.TagDef.Should().Be("NewDef");
        tag.TagText.Should().Be("New Translation");
        tag.TagComment.Should().Be("New Comment");
        tag.TagType.Should().Be("label");
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("Def", "Translation", "")]
    [InlineData("Def", "", "label")]
    [InlineData("Def", "Translation", "label")]
    public void Constructor_WithEmptyStrings_ShouldAcceptValues(string tagDef, string tagText, string tagType)
    {
        // Act
        var tag = new RimTag(tagDef, tagText, tagType);

        // Assert
        tag.TagDef.Should().Be(tagDef);
        tag.TagText.Should().Be(tagText);
        tag.TagType.Should().Be(tagType);
    }
}
