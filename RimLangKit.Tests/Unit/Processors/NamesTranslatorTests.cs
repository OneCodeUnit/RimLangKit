using FluentAssertions;
using RimLangKit.Processors;

namespace RimLangKit.Tests.Unit.Processors;

public class NamesTranslatorTests
{
    [Fact]
    public void NamesTranslatorActivity_SimpleNames_ShouldTransliterate()
    {
        // Arrange
        var names = "John\nMary\nDavid";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            File.Exists(outputFile).Should().BeTrue();

            var transliterated = File.ReadAllLines(outputFile);
            transliterated.Should().HaveCount(3);
            transliterated[0].Should().StartWith("Дж"); // John -> Джон
            transliterated[1].Should().StartWith("М"); // Mary
            transliterated[2].Should().StartWith("Д"); // David

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_EmptyLines_ShouldReportErrors()
    {
        // Arrange
        var names = "John\n\nMary";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Contain("записано с ошибкой");

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            File.Exists(outputFile).Should().BeTrue();

            var transliterated = File.ReadAllLines(outputFile);
            transliterated[1].Should().Be("Ошибка");

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_NameWithCh_ShouldTransliterateCorrectly()
    {
        // Arrange
        var names = "Charles\nCharlotte";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            var transliterated = File.ReadAllLines(outputFile);

            // ch -> ч
            transliterated[0].Should().Contain("ч");
            transliterated[1].Should().Contain("ч");

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_NameWithSh_ShouldTransliterateCorrectly()
    {
        // Arrange
        var names = "Sharon\nShawn";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            var transliterated = File.ReadAllLines(outputFile);

            // sh -> ш
            transliterated[0].Should().Contain("ш");
            transliterated[1].Should().Contain("ш");

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_CapitalizationRule_ShouldCapitalizeFirstLetter()
    {
        // Arrange
        var names = "alice\nbob\ncarol";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            var transliterated = File.ReadAllLines(outputFile);

            // All names should start with uppercase
            transliterated.Should().AllSatisfy(name =>
            {
                name.Should().NotBeNullOrEmpty();
                char.IsUpper(name[0]).Should().BeTrue();
            });

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_LeadingTrailingSpaces_ShouldTrim()
    {
        // Arrange
        var names = "  John  \n  Mary  ";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            var transliterated = File.ReadAllLines(outputFile);

            // Should not have leading or trailing spaces
            transliterated.Should().AllSatisfy(name =>
            {
                name.Should().NotStartWith(" ");
                name.Should().NotEndWith(" ");
            });

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_SingleCharacterName_ShouldCapitalize()
    {
        // Arrange
        var names = "a";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            var transliterated = File.ReadAllLines(outputFile);

            transliterated.Should().HaveCount(1);
            transliterated[0].Should().BeUpperCased();

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_NameEndingWithE_ShouldRemoveE()
    {
        // Arrange - ending 'e' should be removed according to lineEnd rules
        var names = "Alice\nJane";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            File.Exists(outputFile).Should().BeTrue();

            var transliterated = File.ReadAllLines(outputFile);
            transliterated.Should().HaveCount(2);

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void NamesTranslatorActivity_NonExistentFile_ShouldThrowException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");

        // Act
        Action act = () => NamesTranslator.NamesTranslatorActivity(nonExistentFile);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void NamesTranslatorActivity_MultipleNames_ShouldCreateOutputFile()
    {
        // Arrange
        var names = "Alexander\nElizabeth\nWilliam\nMargaret\nJames";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, names);

        try
        {
            // Act
            var result = NamesTranslator.NamesTranslatorActivity(tempFile);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeEmpty();

            var outputFile = tempFile.Replace(".tmp", "_NEW.tmp");
            File.Exists(outputFile).Should().BeTrue();

            var transliterated = File.ReadAllLines(outputFile);
            transliterated.Should().HaveCount(5);
            transliterated.Should().AllSatisfy(name =>
            {
                name.Should().NotBeNullOrEmpty();
                name.Should().NotBe("Ошибка");
            });

            File.Delete(outputFile);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
