using FluentAssertions;
using LiteDB;
using RimLangKit.Models;
using RimLangKit.Repositories;

namespace RimLangKit.Tests.Unit.Repositories;

public class TranslationRepositoryTests : IDisposable
{
    private readonly TranslationRepository _repository;
    private readonly string _tempDbPath;

    public TranslationRepositoryTests()
    {
        // Create a temporary database for each test
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
    public void SaveTags_NewTags_ShouldSaveSuccessfully()
    {
        // Arrange
        var tags = new List<RimTag>
        {
            new RimTag("TestDef1", "Перевод 1", "label"),
            new RimTag("TestDef2", "Перевод 2", "description")
        };

        // Act
        _repository.SaveTags(tags, "TestCollection", rewrite: false);

        // Assert
        using (var db = new LiteDatabase(_tempDbPath))
        {
            var collection = db.GetCollection<RimTag>("TestCollection");
            collection.Count().Should().Be(2);

            var savedTag1 = collection.FindOne(x => x.TagDef == "TestDef1");
            savedTag1.Should().NotBeNull();
            savedTag1.TagText.Should().Be("Перевод 1");
            savedTag1.TagType.Should().Be("label");
        }
    }

    [Fact]
    public void SaveTags_WithRewrite_ShouldUpdateExisting()
    {
        // Arrange
        var originalTags = new List<RimTag>
        {
            new RimTag("TestDef", "Старый перевод", "label")
        };
        var updatedTags = new List<RimTag>
        {
            new RimTag("TestDef", "Новый перевод", "label")
        };

        // Act
        _repository.SaveTags(originalTags, "TestCollection", rewrite: false);
        _repository.SaveTags(updatedTags, "TestCollection", rewrite: true);

        // Assert
        using (var db = new LiteDatabase(_tempDbPath))
        {
            var collection = db.GetCollection<RimTag>("TestCollection");
            var tags = collection.Find(x => x.TagDef == "TestDef").ToList();
            tags.Should().HaveCount(1);
            tags[0].TagText.Should().Be("Новый перевод");
        }
    }

    [Fact]
    public void SaveTags_WithoutRewrite_ShouldNotUpdateExisting()
    {
        // Arrange
        var originalTags = new List<RimTag>
        {
            new RimTag("TestDef", "Старый перевод", "label")
        };
        var updatedTags = new List<RimTag>
        {
            new RimTag("TestDef", "Новый перевод", "label")
        };

        // Act
        _repository.SaveTags(originalTags, "TestCollection", rewrite: false);
        _repository.SaveTags(updatedTags, "TestCollection", rewrite: false);

        // Assert
        using (var db = new LiteDatabase(_tempDbPath))
        {
            var collection = db.GetCollection<RimTag>("TestCollection");
            var tags = collection.Find(x => x.TagDef == "TestDef").ToList();
            tags.Should().HaveCount(1);
            tags[0].TagText.Should().Be("Старый перевод");
        }
    }

    [Fact]
    public void GetTag_ExistingTag_ShouldReturnTranslation()
    {
        // Arrange
        var tags = new List<RimTag>
        {
            new RimTag("TestDef", "Перевод", "English", "label")
        };
        _repository.SaveTags(tags, "TestCollection", rewrite: false);

        var searchTag = new RimTag("TestDef", "English", "label");

        // Act
        var result = _repository.GetTag(searchTag, "TestCollection");

        // Assert
        result.Should().Be("Перевод");
    }

    [Fact]
    public void GetTag_NonExistentTag_ShouldReturnEmpty()
    {
        // Arrange
        var searchTag = new RimTag("NonExistent", "Some text", "label");

        // Act
        var result = _repository.GetTag(searchTag, "TestCollection");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Analyze_WithData_ShouldReturnCount()
    {
        // Arrange
        var tags = new List<RimTag>
        {
            new RimTag("Def1", "Перевод 1", "label"),
            new RimTag("Def2", "Перевод 2", "label"),
            new RimTag("Def3", "Перевод 3", "description")
        };
        _repository.SaveTags(tags, "TestCollection", rewrite: false);

        // Act
        var result = _repository.Analyze("TestCollection");

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Contain("3 тегов");
    }

    [Fact]
    public void Analyze_WithoutData_ShouldReturnError()
    {
        // Act
        var result = _repository.Analyze("EmptyCollection");

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("ничего нет");
    }

    [Fact]
    public void Clear_ShouldRemoveAllTags()
    {
        // Arrange
        var tags = new List<RimTag>
        {
            new RimTag("Def1", "Перевод 1", "label"),
            new RimTag("Def2", "Перевод 2", "description"),
            new RimTag("Def3", "Перевод 3", "label")
        };
        _repository.SaveTags(tags, "TestCollection", rewrite: false);

        // Act
        var result = _repository.Clear("TestCollection");

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Contain("Удалено 3 записей");

        using (var db = new LiteDatabase(_tempDbPath))
        {
            var collection = db.GetCollection<RimTag>("TestCollection");
            collection.Count().Should().Be(0);
        }
    }

    [Fact]
    public void SaveTags_WithDifferentTypes_ShouldSaveAll()
    {
        // Arrange
        var tags = new List<RimTag>
        {
            new RimTag("Def1", "Метка", "label"),
            new RimTag("Def1", "Описание", "description"),
            new RimTag("Def2", "Метка 2", "label")
        };

        // Act
        _repository.SaveTags(tags, "TestCollection", rewrite: false);

        // Assert
        using (var db = new LiteDatabase(_tempDbPath))
        {
            var collection = db.GetCollection<RimTag>("TestCollection");
            collection.Count().Should().Be(3);
        }
    }
}
