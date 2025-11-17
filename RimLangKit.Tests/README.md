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

## Статистика тестов

**Всего тестов: 85**

### Разбивка по компонентам:

#### Checks (6 тестов)
- **XmlErrorCheckerTests** - валидация XML файлов
  - Проверка валидного/невалидного XML
  - Проверка корневого элемента LanguageData
  - Обработка пустых файлов и несуществующих файлов

#### Models (10 тестов)
- **RimTagTests** (5 тестов) - модель данных для тегов
  - Конструкторы с/без комментария
  - Установка свойств
- **XmlErrorTests** (5 тестов) - модель результата операций
  - Валидация успешных/неуспешных операций
  - Работа с сообщениями

#### Parsers (8 тестов)
- **TranslationParserTests** - парсинг XML и работа с БД
  - Парсинг XML с комментариями формата `<!-- EN: ... -->`
  - Сохранение тегов в LiteDB
  - Автоматический перевод из базы данных
  - Обработка некорректных данных

#### Processors (52 теста)
- **CommentInserterTests** (6 тестов) - добавление комментариев
- **EncodingFixerTests** (8 тестов) - исправление кодировки
- **FileRenamerTests** (7 тестов) - переименование файлов
- **ChangesFinderTests** (11 тестов) - поиск изменений
- **NamesTranslatorTests** (10 тестов) - транскрипция имен
- **FileFixerTests** (10 тестов) - проверка и исправление XML

#### Repositories (9 тестов)
- **TranslationRepositoryTests** - работа с LiteDB
  - Сохранение с/без перезаписи
  - Поиск переводов
  - Анализ коллекций
  - Очистка базы данных

## Дальнейшие шаги

Компоненты, которые могут быть покрыты тестами:

1. **MorpherService** - работа с API морфологии (требуются моки HttpClient)
2. **GitHubService** - работа с GitHub API (требуются моки HttpClient)
3. **CaseCreator** - создание файлов склонений (интеграция с MorpherService)
4. **TagCollector** - сбор статистики по тегам
5. **PreTranslator** - повторное использование переводов
6. **AutoTranslator** - полный workflow автоперевода
7. **Интеграционные тесты** - end-to-end сценарии использования

## Помощь

Если у вас есть вопросы по написанию тестов, обратитесь к:
- [Документация xUnit](https://xunit.net/)
- [Документация FluentAssertions](https://fluentassertions.com/)
- [Документация Moq](https://github.com/moq/moq4)
