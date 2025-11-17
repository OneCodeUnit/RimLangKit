using FluentAssertions;
using LiteDB;
using RimLangKit.Models;
using RimLangKit.Repositories;

namespace RimLangKit.Tests.Unit.Repositories;

public class TranslationRepositoryTests : IDisposable
{
    private readonly LiteDatabase _database;
    private readonly string _tempDbPath;

    public TranslationRepositoryTests()
    {
        // Create a temporary database for each test
        _tempDbPath = Path.GetTempFileName();
        _database = new LiteDatabase(_tempDbPath);
    }

    public void Dispose()
    {
        _database?.Dispose();
        if (File.Exists(_tempDbPath))
        {
            File.Delete(_tempDbPath);
        }
    }

    [Fact]
    public void SaveTag_NewTag_ShouldSaveSuccessfully()
    {
        // Arrange
        var tag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "Original Text",
            Translation = "Переведенный текст"
        };

        // Act
        TranslationRepository.SaveTag(_database, tag);

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        var savedTag = collection.FindOne(x => x.DefName == "TestDef" && x.TagType == "label");

        savedTag.Should().NotBeNull();
        savedTag.Original.Should().Be("Original Text");
        savedTag.Translation.Should().Be("Переведенный текст");
    }

    [Fact]
    public void SaveTag_WithComment_ShouldSaveComment()
    {
        // Arrange
        var tag = new RimTag("TestDef", "description", "English", "Русский", "Test Comment");

        // Act
        TranslationRepository.SaveTag(_database, tag);

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        var savedTag = collection.FindOne(x => x.DefName == "TestDef");

        savedTag.Should().NotBeNull();
        savedTag.Comment.Should().Be("Test Comment");
    }

    [Fact]
    public void SaveTag_WithOverwrite_ShouldUpdateExisting()
    {
        // Arrange
        var originalTag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "Old Text",
            Translation = "Старый текст"
        };

        var updatedTag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "New Text",
            Translation = "Новый текст"
        };

        // Act
        TranslationRepository.SaveTag(_database, originalTag, overwrite: false);
        TranslationRepository.SaveTag(_database, updatedTag, overwrite: true);

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        var tags = collection.Find(x => x.DefName == "TestDef").ToList();

        tags.Should().HaveCount(1);
        tags[0].Original.Should().Be("New Text");
        tags[0].Translation.Should().Be("Новый текст");
    }

    [Fact]
    public void SaveTag_WithoutOverwrite_ShouldNotUpdateExisting()
    {
        // Arrange
        var originalTag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "Old Text",
            Translation = "Старый текст"
        };

        var updatedTag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "New Text",
            Translation = "Новый текст"
        };

        // Act
        TranslationRepository.SaveTag(_database, originalTag, overwrite: false);
        TranslationRepository.SaveTag(_database, updatedTag, overwrite: false);

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        var tags = collection.Find(x => x.DefName == "TestDef").ToList();

        tags.Should().HaveCount(1);
        tags[0].Original.Should().Be("Old Text");
        tags[0].Translation.Should().Be("Старый текст");
    }

    [Fact]
    public void GetTag_ExistingTag_ShouldReturnTag()
    {
        // Arrange
        var tag = new RimTag
        {
            DefName = "TestDef",
            TagType = "label",
            Original = "Test",
            Translation = "Тест"
        };
        TranslationRepository.SaveTag(_database, tag);

        // Act
        var result = TranslationRepository.GetTag(_database, "TestDef", "label");

        // Assert
        result.Should().NotBeNull();
        result.DefName.Should().Be("TestDef");
        result.TagType.Should().Be("label");
    }

    [Fact]
    public void GetTag_NonExistentTag_ShouldReturnNull()
    {
        // Act
        var result = TranslationRepository.GetTag(_database, "NonExistent", "label");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ClearCollection_ShouldRemoveAllTags()
    {
        // Arrange
        TranslationRepository.SaveTag(_database, new RimTag { DefName = "Tag1", TagType = "label" });
        TranslationRepository.SaveTag(_database, new RimTag { DefName = "Tag2", TagType = "description" });
        TranslationRepository.SaveTag(_database, new RimTag { DefName = "Tag3", TagType = "label" });

        // Act
        TranslationRepository.ClearCollection(_database);

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        collection.Count().Should().Be(0);
    }

    [Fact]
    public void SaveMultipleTags_WithDifferentTypes_ShouldSaveAll()
    {
        // Arrange
        var tags = new[]
        {
            new RimTag { DefName = "Def1", TagType = "label", Translation = "Метка" },
            new RimTag { DefName = "Def1", TagType = "description", Translation = "Описание" },
            new RimTag { DefName = "Def2", TagType = "label", Translation = "Метка 2" }
        };

        // Act
        foreach (var tag in tags)
        {
            TranslationRepository.SaveTag(_database, tag);
        }

        // Assert
        var collection = _database.GetCollection<RimTag>("RimTags");
        collection.Count().Should().Be(3);
    }
}
