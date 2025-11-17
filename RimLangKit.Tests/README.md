# RimLangKit.Tests

Проект автоматических тестов для RimLangKit.

## Структура проекта

```
RimLangKit.Tests/
├── Unit/                          # Модульные тесты
│   ├── Checks/                    # Тесты для валидации XML
│   │   └── XmlErrorCheckerTests.cs
│   ├── Models/                    # Тесты для моделей данных
│   │   └── RimTagTests.cs
│   ├── Parsers/                   # Тесты для парсеров
│   ├── Processors/                # Тесты для процессоров
│   ├── Repositories/              # Тесты для репозиториев
│   │   └── TranslationRepositoryTests.cs
│   └── Services/                  # Тесты для сервисов
├── Integration/                   # Интеграционные тесты
├── TestData/                      # Тестовые данные
│   ├── ValidXml/                  # Валидные XML файлы
│   ├── InvalidXml/                # Невалидные XML файлы
│   └── SampleTranslations/        # Примеры переводов
└── Helpers/                       # Вспомогательные классы
    └── TestDataBuilder.cs         # Построитель тестовых данных
```

## Используемые технологии

- **xUnit** - фреймворк для тестирования
- **FluentAssertions** - читаемые утверждения в тестах
- **Moq** - библиотека для создания моков
- **LiteDB** - для тестирования работы с БД
- **coverlet.collector** - сбор покрытия кода

## Запуск тестов

### Через командную строку

```bash
# Запустить все тесты
dotnet test

# Запустить тесты с детальным выводом
dotnet test --logger "console;verbosity=detailed"

# Запустить тесты с измерением покрытия
dotnet test /p:CollectCoverage=true
```

### Через Visual Studio

1. Откройте `RimLangKit.sln`
2. Откройте Test Explorer (Test > Test Explorer)
3. Нажмите "Run All Tests"

### Через VS Code

1. Установите расширение "C# Dev Kit"
2. Откройте панель Testing
3. Нажмите кнопку запуска тестов

## Написание новых тестов

### Пример модульного теста

```csharp
using FluentAssertions;
using RimLangKit.YourNamespace;

namespace RimLangKit.Tests.Unit.YourNamespace;

public class YourClassTests
{
    [Fact]
    public void MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        var input = "test";

        // Act
        var result = YourClass.Method(input);

        // Assert
        result.Should().Be("expected");
    }

    [Theory]
    [InlineData("input1", "output1")]
    [InlineData("input2", "output2")]
    public void MethodName_WithDifferentInputs_ReturnsExpectedOutputs(
        string input,
        string expected)
    {
        // Act
        var result = YourClass.Method(input);

        // Assert
        result.Should().Be(expected);
    }
}
```

### Использование TestDataBuilder

```csharp
using RimLangKit.Tests.Helpers;

// Создать тег с дефолтными значениями
var tag = TestDataBuilder.CreateSampleTag();

// Создать тег с кастомными значениями
var customTag = TestDataBuilder.CreateSampleTag(
    defName: "CustomDef",
    tagType: "label",
    translation: "Кастомный перевод"
);

// Создать список тегов
var tags = TestDataBuilder.CreateSampleTags(count: 10);

// Создать временный XML файл
var xml = TestDataBuilder.CreateValidXml(
    ("Gun.label", "винтовка"),
    ("Gun.description", "Описание")
);
var tempFile = TestDataBuilder.CreateTempXmlFile(xml);
```

## Рекомендации по тестированию

### Приоритеты

1. **Высокий приоритет** - критичные компоненты:
   - XmlErrorChecker
   - TranslationParser
   - TranslationRepository

2. **Средний приоритет** - обработка данных:
   - Processors (CommentInserter, EncodingFixer, FileRenamer и т.д.)
   - Services (MorpherService, GitHubService)

3. **Низкий приоритет**:
   - GUI компоненты (лучше интеграционные/ручные тесты)
   - Утилиты

### Покрытие кода

Целевое покрытие: **70-80%** для бизнес-логики.

### Что тестировать

- ✅ Обычные сценарии использования
- ✅ Граничные случаи
- ✅ Обработка ошибок
- ✅ Валидация входных данных
- ✅ Работа с файлами и БД

### Что НЕ тестировать на уровне Unit-тестов

- ❌ GUI (Windows Forms)
- ❌ Реальные HTTP запросы (использовать моки)
- ❌ Реальные файлы (использовать временные)

## Примеры существующих тестов

### XmlErrorCheckerTests

- ✅ Проверка валидного XML
- ✅ Проверка невалидного XML
- ✅ Проверка отсутствия корневого элемента LanguageData
- ✅ Проверка пустого файла
- ✅ Проверка несуществующего файла

### TranslationRepositoryTests

- ✅ Сохранение нового тега
- ✅ Сохранение тега с комментарием
- ✅ Обновление существующего тега (с флагом overwrite)
- ✅ НЕ обновление существующего тега (без флага overwrite)
- ✅ Получение существующего тега
- ✅ Получение несуществующего тега
- ✅ Очистка коллекции
- ✅ Сохранение тегов с разными типами

### RimTagTests

- ✅ Создание тега без комментария
- ✅ Создание тега с комментарием
- ✅ Дефолтный конструктор
- ✅ Установка свойств
- ✅ Работа с пустыми строками

## Дальнейшие шаги

Следующие компоненты требуют покрытия тестами:

1. **TranslationParser** - парсинг XML и работа с БД
2. **CommentInserter** - добавление комментариев в XML
3. **EncodingFixer** - исправление кодировки файлов
4. **FileRenamer** - переименование файлов
5. **MorpherService** - работа с API морфологии (с моками)
6. **GitHubService** - работа с GitHub API (с моками)

## Помощь

Если у вас есть вопросы по написанию тестов, обратитесь к:
- [Документация xUnit](https://xunit.net/)
- [Документация FluentAssertions](https://fluentassertions.com/)
- [Документация Moq](https://github.com/moq/moq4)
