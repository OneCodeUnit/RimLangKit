using FluentAssertions;
using RimLangKit.Models;

namespace RimLangKit.Tests.Unit.Models;

public class RimTagTests
{
    [Fact]
    public void Constructor_WithoutComment_ShouldCreateTag()
    {
        // Act
        var tag = new RimTag("TestDef", "label", "Original", "Translation");

        // Assert
        tag.DefName.Should().Be("TestDef");
        tag.TagType.Should().Be("label");
        tag.Original.Should().Be("Original");
        tag.Translation.Should().Be("Translation");
        tag.Comment.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithComment_ShouldCreateTagWithComment()
    {
        // Act
        var tag = new RimTag("TestDef", "description", "Original", "Translation", "Test Comment");

        // Assert
        tag.DefName.Should().Be("TestDef");
        tag.TagType.Should().Be("description");
        tag.Original.Should().Be("Original");
        tag.Translation.Should().Be("Translation");
        tag.Comment.Should().Be("Test Comment");
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyTag()
    {
        // Act
        var tag = new RimTag();

        // Assert
        tag.DefName.Should().BeNull();
        tag.TagType.Should().BeNull();
        tag.Original.Should().BeNull();
        tag.Translation.Should().BeNull();
        tag.Comment.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var tag = new RimTag();

        // Act
        tag.DefName = "NewDef";
        tag.TagType = "label";
        tag.Original = "New Original";
        tag.Translation = "New Translation";
        tag.Comment = "New Comment";

        // Assert
        tag.DefName.Should().Be("NewDef");
        tag.TagType.Should().Be("label");
        tag.Original.Should().Be("New Original");
        tag.Translation.Should().Be("New Translation");
        tag.Comment.Should().Be("New Comment");
    }

    [Theory]
    [InlineData("", "", "", "")]
    [InlineData("Def", "label", "", "")]
    [InlineData("Def", "", "Original", "")]
    [InlineData("Def", "label", "Original", "")]
    public void Constructor_WithEmptyStrings_ShouldAcceptValues(string defName, string tagType, string original, string translation)
    {
        // Act
        var tag = new RimTag(defName, tagType, original, translation);

        // Assert
        tag.DefName.Should().Be(defName);
        tag.TagType.Should().Be(tagType);
        tag.Original.Should().Be(original);
        tag.Translation.Should().Be(translation);
    }
}
