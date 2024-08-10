﻿Набор инструментов для работы с локализацией игры RimWorld от создателя сервера [Vanilla Russian Expanded](https://discord.gg/GB2e2VhgVE) и проекта переводов модов [Vanilla Russian Expanded](https://github.com/OneCodeUnit/VanillaRussianExpanded).

+ [Для пользователей](https://github.com/OneCodeUnit/RimLangKit#для-пользователей)
+ [Для переводчиков](https://github.com/OneCodeUnit/RimLangKit#для-переводчиков)

## Системные требования
| Версия  | Windows (минимальная версия) | .NET |
| ------------- | ------------- | ------------- |
| RimLangKit.exe | Windows 10 x64 1607 | [.NET 8](https://dotnet.microsoft.com/en-us/download) (любой вариант) |
| RimLangKit.7z | Windows 10 x64 1607 | - |

Фактически программе ничто не мешает работать на x86 системах и (с минимальными правками кода) на более старых версиях Windows. 
Я не публикую для них сборки из-за их непопулярности, но вы можете собрать себе желаемую версию из исходников.
И если не доверяете собранным мной .exe (уважаемо), собрать проект из исходников можно самостоятельно запустив !publish.bat

Для создания Case, Plural и Gender файлов потребуется до 800 Мбайт ОЗУ, в остальных сценариях - до 50.

### [Скачать программу](https://github.com/OneCodeUnit/RimLangKit/releases/latest)

### Включенные решения
+ Для темной темы Windows Forms используется проект от [BlueMystical](https://github.com/BlueMystical/Dark-Mode-Forms)
+ За склонение слов, определение рода и создание множественного числа отвечает библиотека [Cyriller](https://github.com/miyconst/Cyriller)
+ Дополнительно для склонения слов используется API [веб-сервиса «Морфер» 3.0](https://morpher.ru/ws3/)
+ Для проверки версии программы, проверки версии локализации и её загрузки используется GitHub API

### Установка и удаление
Программу не нужно устанавливать - она запускается из папки, где ее разместили.
Для полного удаления можно удалить сохраненные настройки. Они расположены по стандартному пути для .Net-приложений  "C:\Users\\%ваш__пользователь%\AppData\Local\OliveWizard"

# Для пользователей

## Работа программы
Из-за того, что игру переводит сообщество, работа над переводом начинается только после выхода обновления и может продолжаться значительное время. Более того, в саму игру он попадает с большой задержкой. Данная программа позволяет скачать самую последнюю версию перевода и сразу добавить его в игру.
Для работы необходимо указать путь до корневой папки с игрой (та, где есть папка "Data" и файл "RimWorldWin64.exe"), а потом выбрать в игре язык "Russian (GitHub)" вместо "Russian (Русский)".

Преимущества по сравнению с [консольным скриптом](https://github.com/asidsx/RimWorldRuslangAutoUpdater/blob/main/auto.bat) заключаются в том, что:
+ Ничего не скачивает, если нет новой версии
+ Не записывает данные лишний раз
+ Может использоваться для обновления не только русской локализации игры

Дополнительные возможности:
+ Поле с "Ludeon/RimWorld-ru". Позволяет выбрать другой репозиторий для загрузки языка (это часть адреса. Например, https://github.com/Ludeon/RimWorld-ru). Полезно для других языков и собственных форков.
+ Поле с "Russian (GitHub)". Позволяет задать название языка в игре. Часть "Russian" название языка. Если играете с русским, менять не следует. Если нет, то именно ее и нужно изменить. Часть "(GitHub)" может быть произвольной и нужна для идентификации перевода.
+ Кнопка "Удалить перевод". Сбрасывает номер текущей версии перевода и удаляет папки с выбранным языком.
+ Кнопка "Сброс настроек". Возвращает все настройки к исходным (как при первом запуске).

### Обратите внимание

+ Не стоит проверять обновления локализации слишком часто (речь о десятках раз в час), так как это может привести к временной блокировке по IP.


# Для переводчиков

## Работа программы
Программа позволяет выбрать папку и применить ко всем файлам в ней одно из действий:
+ [Добавить комментарии](https://github.com/OneCodeUnit/RimLangKit#добавить-комментарии)
+ [Избавиться от одинаковых имен файлов](https://github.com/OneCodeUnit/RimLangKit#избавиться-от-одинаковых-имен-файлов)
+ [Транскрипция имен](https://github.com/OneCodeUnit/RimLangKit#транскрипция-имен)
+ [Создать Case, Plural и Gender файлы](https://github.com/OneCodeUnit/RimLangKit#создать-case-plural-и-gender-файлы)
+ [Исправление кодировки](https://github.com/OneCodeUnit/RimLangKit#исправление-кодировки)
+ [Анализ тегов](https://github.com/OneCodeUnit/RimLangKit#анализ-тегов)
+ [Поиск сломанных файлов](https://github.com/OneCodeUnit/RimLangKit#поиск-сломанных-файлов)
+ [Поиск изменений в тексте](https://github.com/OneCodeUnit/RimLangKit#поиск-изменений-в-тексте)

## Добавить комментарии
Программа добавит во все .xml файлы в выбранной папке и во всех подпапках комментарии с исходным текстом. Полезно для того, чтобы не забыть что ты переводишь.
**Важно! Это действие подразумевает, что файлы уже обработаны RimTrans или его аналогом.**
### Пример
Исходный текст:
> \<VBE_StartInsertion>Bring Syrup</VBE_StartInsertion>\
> \<VBE_StartInsertionDesc>Bring a syrup to the soda fountain to start processing</VBE_StartInsertionDesc>\
> \<VBE_CancelBringingSyrup>Cancel Bringing Syrup</VBE_CancelBringingSyrup>\
> \<VBE_CancelBringingSyrupDesc>Cancel bringing syrup to the soda fountain.</VBE_CancelBringingSyrupDesc>

Обработанный текст:
> \<!-- EN: Bring Syrup -->\
> \<VBE_StartInsertion>Bring Syrup</VBE_StartInsertion>\
> \<!-- EN: Bring a syrup to the soda fountain to start processing -->\
> \<VBE_StartInsertionDesc>Bring a syrup to the soda fountain to start processing</VBE_StartInsertionDesc>\
> \<!-- EN: Cancel Bringing Syrup -->\
> \<VBE_CancelBringingSyrup>Cancel Bringing Syrup</VBE_CancelBringingSyrup>\
> \<!-- EN: Cancel bringing syrup to the soda fountain. -->\
> \<VBE_CancelBringingSyrupDesc>Cancel bringing syrup to the soda fountain.</VBE_CancelBringingSyrupDesc>

## Избавиться от одинаковых имен файлов
Программа добавит всем .xml файлам приписку к имени, соответствующей имени папки перед «Languages».
### Пример
Исходный файл
> Vanilla Furniture Expanded Russian Language pack\Architect\Languages\Russian\DefInjected\TerrainDef\Terrain_Floors_Arch.xml

Обработанный файл:
> Vanilla Furniture Expanded Russian Language pack\Architect\Languages\Russian\DefInjected\TerrainDef\Terrain_Floors_Arch_Architect.xml

## Транскрипция имен
Программа прочитает все .txt файлы и создаст дубли с припиской "_NEW", текст в которых будет транслитерирован согласно правилам из словаря. Полезно для перевода списка имен.
Он состоит из нескольких строк:
1. Четырехбуквенные части
2. Трехбуквенные части
3. Двухбуквенные части
4. Однобуквенные части
5. Не имеющие транскрипции части
6. Правила для начала слова
7. Правила для конца слова

Порядок их проверки: 6 - 7 - 5 - 1 - 2 - 3 - 4

## Создать Case, Plural и Gender файлы
Для корректного склонения названий объектов игре требуются дополнительные файлы:
+ Case.txt - склонение слова по падежам
+ Plural.txt - склонение слова по числам
+ Female.txt, Male.txt, Neuter.txt и Plural.txt в папке "Gender" - распределение слов по родам.

Программа проанализирует все названия объектов мода и попытается сделать все за вас. Сделает она это далеко не идеально, поэтому текст необходимо дорабатывать вручную.
Однако объем работы все равно будет сильно сокращен. За склонение отвечает библиотека [Cyriller](https://github.com/miyconst/Cyriller).

Все, что программа совсем не смогла опознать, будет помещено в файл "Undefined.txt". Обратите внимание, что программа добавит немного больше объектов, чем требуется. Не стесняйтесь удалять лишнее.

Программе нужно указать корневую папку перевода.
**Важно! Это действие подразумевает, что файлы уже обработаны RimTrans или его аналогом.**

## Исправление кодировки
При выгрузке из внешних сервисов, например, Crowdin, файлы скачиваются в неправильной кодировке. 
Программа пересохраняет каждый файл в кодировке UTF-8 BOM, с табуляцией в два пробела и окончанием строк CRLF. Это позволит переводу соотвествовать стандарту русской локализации.

## Анализ тегов
Программа собирает статистику использования тегов и defs в переводе и сохраняет ее в текстовые файлы в папке исполняемого файла. Может быть полезно?
**Важно! Это действие подразумевает, что файлы уже обработаны RimTrans или его аналогом.**

## Поиск сломанных файлов
Позволяет найти в переводе сломанные по той или иной причине файлы, которые не будут считываться игрой или просто не соответствуют стандартам xml.

## Поиск изменений в тексте
Позволяет сравнить исходный текст перевода и текущий текст мода. Результат выводится в текстовые файлы в папке исполняемого файла. Результат представляет собой связку из defName и нового текста.
+ defName с совпадающим текстом в моде и переводе не записываются
+ defName различающимся текстом записываются в файл ChangedData.txt. На них стоит обратить внимание. Обновления мода изменили текст.
+ В TranslationData.txt записываются defName *из перевода* у которых не нашлось совпадений в *файлах мода**. Эти строки необязательно ошибка, так как ваша программа для перевода могла вытянуть не всё.
+ В ModData.txt записываются defName *из мода* у которых не нашлось совпадений в *файлах перевода**. Эти строки необязательно ошибка, так как ваша программа для перевода могла вытянуть лишнее.


defName иногда бывают неуникальны и в таком случае к ним при записи добавляются 4 уникальные цифры. Из-за этого такие строки проверить не получится.
**Важно! Это действие подразумевает, что файлы уже обработаны RimTrans или его аналогом.**
