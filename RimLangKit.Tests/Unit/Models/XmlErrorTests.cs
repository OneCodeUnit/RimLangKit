using FluentAssertions;
using RimLangKit.Checks;

namespace RimLangKit.Tests.Unit.Models;

public class XmlErrorTests
{
    [Fact]
    public void Constructor_WithSuccessAndMessage_ShouldSetProperties()
    {
        // Act
        var error = new XmlError(true, "Success message");

        // Assert
        error.IsValid.Should().BeTrue();
        error.Message.Should().Be("Success message");
    }

    [Fact]
    public void Constructor_WithErrorAndMessage_ShouldSetProperties()
    {
        // Act
        var error = new XmlError(false, "Error occurred");

        // Assert
        error.IsValid.Should().BeFalse();
        error.Message.Should().Be("Error occurred");
    }

    [Fact]
    public void Constructor_WithEmptyMessage_ShouldAcceptEmptyString()
    {
        // Act
        var error = new XmlError(true, string.Empty);

        // Assert
        error.IsValid.Should().BeTrue();
        error.Message.Should().BeEmpty();
    }

    [Theory]
    [InlineData(true, "Test message 1")]
    [InlineData(false, "Test message 2")]
    [InlineData(true, "")]
    [InlineData(false, "Ошибка с кириллицей")]
    public void Constructor_WithVariousInputs_ShouldSetPropertiesCorrectly(bool isValid, string message)
    {
        // Act
        var error = new XmlError(isValid, message);

        // Assert
        error.IsValid.Should().Be(isValid);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var error = new XmlError(true, "Initial");

        // Act
        error.IsValid = false;
        error.Message = "Updated";

        // Assert
        error.IsValid.Should().BeFalse();
        error.Message.Should().Be("Updated");
    }
}
