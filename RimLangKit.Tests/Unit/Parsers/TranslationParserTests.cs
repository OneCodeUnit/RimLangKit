using FluentAssertions;
using LiteDB;
using RimLangKit.Models;
using RimLangKit.Parsers;
using RimLangKit.Repositories;

namespace RimLangKit.Tests.Unit.Parsers;

public class TranslationParserTests : IDisposable
{
    private readonly TranslationRepository _repository;
    private readonly string _tempDbPath;

    public TranslationParserTests()
    {
        _tempDbPath = Path.GetTempFileName();
        _repository = new TranslationRepository(_tempDbPath);
    }

    public void Dispose()
    {
        if (File.Exists(_tempDbPath))
        {
            File.Delete(_tempDbPath);
        }
    }

    [Fact]
    public void ParseAndSaveToDatabase_ValidXmlWithComments_ShouldSaveTags()
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

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var tagType = "Weapons";
        var filePath = Path.Combine(tempDir, tagType, "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndSaveToDatabase(filePath, _repository, "TestCollection", rewrite: false);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Message.Should().Contain("2 тегов сохранено");

            // Verify tags were saved
            using (var db = new LiteDatabase(_tempDbPath))
            {
                var collection = db.GetCollection<RimTag>("TestCollection");
                var tags = collection.FindAll().ToList();

                tags.Should().HaveCount(2);

                var rifleTag = tags.FirstOrDefault(t => t.TagDef == "Gun_AssaultRifle.label");
                rifleTag.Should().NotBeNull();
                rifleTag.TagText.Should().Be("винтовка");
                rifleTag.TagComment.Should().Be("EN: rifle");
                rifleTag.TagType.Should().Be(tagType);

                var descTag = tags.FirstOrDefault(t => t.TagDef == "Gun_AssaultRifle.description");
                descTag.Should().NotBeNull();
                descTag.TagText.Should().Be("Военная штурмовая винтовка");
                descTag.TagComment.Should().Be("EN: A military assault rifle");
            }
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
    public void ParseAndSaveToDatabase_XmlWithoutComments_ShouldNotSaveTags()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun_AssaultRifle.label>винтовка</Gun_AssaultRifle.label>
                <Gun_AssaultRifle.description>Военная штурмовая винтовка</Gun_AssaultRifle.description>
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndSaveToDatabase(filePath, _repository, "TestCollection", rewrite: false);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("не найдено тегов");
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
    public void ParseAndSaveToDatabase_InvalidXml_ShouldReturnError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: rifle -->
                <Gun.label>винтовка
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, invalidXml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndSaveToDatabase(filePath, _repository, "TestCollection", rewrite: false);

            // Assert
            result.IsValid.Should().BeFalse();
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
    public void ParseAndSaveToDatabase_MissingTagType_ShouldUseUnknown()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: test -->
                <Test.label>тест</Test.label>
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "test.xml"); // No parent directory for tag type

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndSaveToDatabase(filePath, _repository, "TestCollection", rewrite: false);

            // Assert
            result.IsValid.Should().BeTrue();

            using (var db = new LiteDatabase(_tempDbPath))
            {
                var collection = db.GetCollection<RimTag>("TestCollection");
                var tag = collection.FindOne(x => x.TagDef == "Test.label");

                tag.Should().NotBeNull();
                tag.TagType.Should().Be("Unknown");
            }
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
    public void ParseAndTranslateTags_WithExistingTranslations_ShouldUpdateFile()
    {
        // Arrange - First save translations to database
        var translationTags = new List<RimTag>
        {
            new RimTag("Gun_AssaultRifle.label", "винтовка", "rifle", "Weapons"),
            new RimTag("Gun_AssaultRifle.description", "Военная штурмовая винтовка", "A military assault rifle", "Weapons")
        };
        _repository.SaveTags(translationTags, "TestCollection", rewrite: false);

        // Create file to translate with English text
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun_AssaultRifle.label>rifle</Gun_AssaultRifle.label>
                <Gun_AssaultRifle.description>A military assault rifle</Gun_AssaultRifle.description>
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndTranslateTags(filePath, _repository, "TestCollection");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Message.Should().Contain("Автоматически переведено 2 тегов");

            // Verify file was updated
            var updatedContent = File.ReadAllText(filePath);
            updatedContent.Should().Contain("<Gun_AssaultRifle.label>винтовка</Gun_AssaultRifle.label>");
            updatedContent.Should().Contain("<Gun_AssaultRifle.description>Военная штурмовая винтовка</Gun_AssaultRifle.description>");
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
    public void ParseAndTranslateTags_WithNoMatchingTranslations_ShouldNotChangeFile()
    {
        // Arrange
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun_AssaultRifle.label>rifle</Gun_AssaultRifle.label>
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndTranslateTags(filePath, _repository, "TestCollection");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Message.Should().Contain("Автоматически переведено 0 тегов");

            var updatedContent = File.ReadAllText(filePath);
            updatedContent.Should().Contain("<Gun_AssaultRifle.label>rifle</Gun_AssaultRifle.label>");
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
    public void ParseAndTranslateTags_InvalidXml_ShouldReturnError()
    {
        // Arrange
        var invalidXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <Gun.label>rifle
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, invalidXml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndTranslateTags(filePath, _repository, "TestCollection");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().NotBeEmpty();
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
    public void ParseAndSaveToDatabase_ShortComment_ShouldNotSaveTag()
    {
        // Arrange - Comment too short (less than 13 characters: "<!-- EN: X -->")
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <LanguageData>
                <!-- EN: -->
                <Gun.label>винтовка</Gun.label>
            </LanguageData>
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(tempDir, "Weapons", "test.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, xml);

        try
        {
            // Act
            var result = TranslationParser.ParseAndSaveToDatabase(filePath, _repository, "TestCollection", rewrite: false);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("не найдено тегов");
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
