Программа содержит в себе ряд инструментов для работы с локализацией игры RimWorld.

## Системные требования
Для запуска требуется [.Net](https://dotnet.microsoft.com/en-us/download).
Он существует в трёх вариантах:
+ SDK - запуск и разработка приложений.
+ Runtime - только запуск приложений.
+ Desktop Runtime - только запуск настольных приложений для Windows.

Для Windows 7 и 8.1 подойдёт только .Net 6, а для Windows 10 (версия 1607+) и 11 (версия 22000+) можно установить .Net 7.

## Работа программы
Программа позволяет выбрать папку и применить ко всем файлам в ней одно из действий:
+ [Обновить локализацию](https://github.com/OneCodeUnit/RimLangKit#%D0%BE%D0%B1%D0%BD%D0%BE%D0%B2%D0%B8%D1%82%D1%8C-%D0%BB%D0%BE%D0%BA%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8E)
+ [Добавить комментарии](https://github.com/OneCodeUnit/RimLangKit#%D0%B4%D0%BE%D0%B1%D0%B0%D0%B2%D0%B8%D1%82%D1%8C-%D0%BA%D0%BE%D0%BC%D0%BC%D0%B5%D0%BD%D1%82%D0%B0%D1%80%D0%B8%D0%B8)
+ [Избавиться от одинаковых имён файлов](https://github.com/OneCodeUnit/RimLangKit#%D0%B8%D0%B7%D0%B1%D0%B0%D0%B2%D0%B8%D1%82%D1%8C%D1%81%D1%8F-%D0%BE%D1%82-%D0%BE%D0%B4%D0%B8%D0%BD%D0%B0%D0%BA%D0%BE%D0%B2%D1%8B%D1%85-%D0%B8%D0%BC%D1%91%D0%BD-%D1%84%D0%B0%D0%B9%D0%BB%D0%BE%D0%B2)
+ [Транскрипция имён](https://github.com/OneCodeUnit/RimLangKit#%D1%82%D1%80%D0%B0%D0%BD%D1%81%D0%BA%D1%80%D0%B8%D0%BF%D1%86%D0%B8%D1%8F-%D0%B8%D0%BC%D1%91%D0%BD)
+ [Создать Case, Plural и Gender файлы](https://github.com/OneCodeUnit/RimLangKit#%D0%BE%D0%B1%D0%BD%D0%BE%D0%B2%D0%B8%D1%82%D1%8C-%D0%BB%D0%BE%D0%BA%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8E)

## Обновить локализацию
Из-за того, что игру переводит сообщество, работа над переводом начинается только после выхода обновления и может продолжаться значительное время. Более того, в саму игру он попадает с большой задержкой. Данная программа позволяет скачать самую последнюю версию перевода и сразу добавить его в игру.
Для работы необходимо указать путь до корневой папки с игрой (та, где есть папка "Data" и файл "RimWorldWin64.exe"), а потом выбрать в игре язык "Russian (GitHub)" вместо "Russian (Русский)".
Преимущества по сравнению с [консольным скриптом](https://github.com/asidsx/RimWorldRuslangAutoUpdater/blob/main/auto.bat) заключаются в том, что:
+ Работает со всеми версиями Windows, начиная с Windows 7.
+ Ничего не скачивает, если нет новой версии.
+ Не записывает данные лишний раз.

## Добавить комментарии
Программа добавит во все .xml файлы в выбранной папке и во всех подпапках комментарии с исходным текстом. Ожидается, что файлы уже обработаны RimTrans или его аналогом.
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

## Избавиться от одинаковых имён файлов
Программа добавит всем .xml файлам приписку к имени, соответствующей имени папки самого верхнего уровня до («Languages»).
### Пример
Исходный файл
> Vanilla Furniture Expanded Russian Language pack\Architect\Languages\Russian\DefInjected\TerrainDef\Terrain_Floors_Arch.xml

Обработанный файл:
> Vanilla Furniture Expanded Russian Language pack\Architect\Languages\Russian\DefInjected\TerrainDef\Terrain_Floors_Arch_Architect.xml

## Транскрипция имён
Программа прочитает все .txt файлы и создаст дубли с припиской "_NEW", текст в которых будет транслитерирован согласно правилам из словаря.
Для работы требуется файл dictionary.txt, который можно скачать из данного репозитория или создать самостоятельно.
Он состоит из нескольких строк:
1. Четырёхбуквенные части
2. Трёхбуквенные части
3. Двухбуквенные части
4. Однобуквенные части
5. Не имеющие транскрипции части
6. Правила для начала слова
7. Правила для конца слова

Порядок их проверки: 6 - 7 - 5 - 1 - 2 - 3 - 4

### Синтаксис словаря:
Правила словаря представляют из себя последовательность пар, разделённых запятой (в конце и начале строки запятая не ставится)
Каждая пара разделена двоеточием (:). Слева английский текст, справа русский.
Пробелы в правилах не используются. Для обозначения пустоты используется символов равенства (=).

## Создать Case, Plural и Gender файлы
Для коррентного склонения названий объектов игре требуются дополнительные файлы:
+ Case.txt - склонение слова по падежам
+ Plural.txt - склонение слова по числам
+ Female.txt, Male.txt, Neuter.txt и Plural.txt в папке "Gender" распределение слов по родам.
Программа проанализует все названия объектов мода и попытается сделать всё за вас. Сделает она это далеко не идеально, поэтому текст необходимо дорабатывать вручную.
Однако объём работы всё равно будет сильно сокращён.
Всё, что программа совсем не смогла опознать, будет помещено в файл "Undefined.txt".
Обратите внимаение, что программа добавит немного больше объектов, чем требуется. Не стесняйтесь удалять лишнее.

## Обратите внимание
+ Не стоит проверять обновления локализации слишком часто (речь о десятках раз в час), так как это может привести к временному бану по IP.

## Заходите на сервер [Vanilla Russian Expanded](https://discord.gg/GB2e2VhgVE), посвященный RimWorld, модам и переводам.
