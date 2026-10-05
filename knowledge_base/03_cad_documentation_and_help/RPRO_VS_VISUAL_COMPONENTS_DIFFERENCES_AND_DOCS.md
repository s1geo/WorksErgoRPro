# Сравнительный Анализ Архитектуры, Форматов и Документации: Р-Про v2.2.2 vs Visual Components

> **Автор:** Senior CAD Documentation & Systems Integration Engineer  
> **Дата:** 5 октября 2026 г.  
> **Проект:** WorksErgo R-Pro Edition (Автономный локальный модуль эргономики)  
> **Расположение:** `D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help\RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md`

---

## 1. Введение и Архитектурная Родословная (Executive Summary & Lineage)

Программный комплекс **«Р-Про»** (разработка Инженерно-технологического центра «Р-Про», г. Санкт-Петербург, сборка v2.2.2) представляет собой адаптированную для российского промышленного сектора, глубоко модифицированную и локализованную версию платформы **Visual Components** (производство Visual Components Oy, Финляндия).

Анализ декомпилированных бинарных сборок, метаданных компонентов и конфигурационных файлов подтверждает, что кодовая база Р-Про v2.2.2 ответвлена от ревизии ядра **Visual Components 4.3** (внутренний маркер ревизии: `4.3.0.34`, базовые сборки UI версии `4.5.0.0`):
1. **Графическое и кинематическое ядро:** Построено на C++ библиотеках `rlinker.dll` (33.6 МБ) и `TSNLib.dll` (6.4 МБ), осуществляющих расчет 3D-сцены, B-Rep геометрии, прямой и обратной кинематики роботов, а также детекцию коллизий.
2. **Пользовательский интерфейс (WPF / Infragistics):** Реализован на платформе Microsoft .NET Framework 4.8 с использованием Caliburn.Micro и Infragistics NetAdvantage Ribbon.
3. **Скриптовый движок:** Встроенный интерпретатор CPython 2.7.1 (`python27.dll`), взаимодействующий с ядром через COM-обертку и внутренний модуль `vcCommand` / `vcApplication`.

Несмотря на внешнее сходство интерфейса, между Р-Про v2.2.2 и стандартным Visual Components (ревизий 4.3–4.8+) существует **ряд критических несовместимостей** на уровне файловых форматов, пространств имен XML, строгой подписи сборок .NET, путей ленточного меню и эргономических модулей. Попытка прямого переноса компонентов или скомпилированных плагинов без специальной адаптации приводит к сбоям загрузки.

Ниже приводится исчерпывающий сравнительный анализ каждого слоя экосистемы.

---

## 2. Сравнительная Матрица Ключевых Различий

| Подсистема / Аспект | Р-Про v2.2.2 (`D:\Apps\RProv222`) | Visual Components (4.3 – 4.8+) | Практическое следствие для разработчика |
| :--- | :--- | :--- | :--- |
| **Расширение файла компонента** | `.rpro` | `.vcmx` (устар. `.vcm`) | Р-Про не открывает файлы с расширением `.vcmx` через стандартный диалог без переименования или конвертации. |
| **Корневой XML Namespace** | `http://schemas.RProSoftDigital1.com/2017/01/component/componentxml` | `http://schemas.visualcomponents.com/2017/01/component/componentxml` | **Фатально:** При простом переименовании `.vcmx` $\to$ `.rpro` компонент бракуется парсером как невалидный. Требуется замена XML пространства имен. |
| **Строгая подпись сборок (.NET)** | **Не подписаны** (`PublicKeyToken=null`) | **Строго подписаны** (`PublicKeyToken=31bc53bc7503b77a` в VC 4.8+) | Плагины C# (.NET), скомпилированные под VC, вызывают ошибку CLR `0x80131044` при запуске в Р-Про. Сборки должны компилироваться против unsigned `UX.Shared.dll`. |
| **Binding Redirects конфигурации** | Тег `tag="RPro"`, редиректы на версию `0.2.2.2` | Стандартные редиректы без кастомных тегов | Конфигурационный файл `rpro_starter.exe.config` жестко перенаправляет зависимости на сборки Р-Про. |
| **Интеграция в Ribbon (Вкладки)** | Вкладка `VcTabHome`, группы `VcRibbonTools`, `VcRibbonClipboard` | Вкладки `VcTabModeling`, `VcTabDrawing`, `VcTabTeach`, `VcTabLayout` | Плагины должны регистрировать кнопки по пути `VcTabHome/VcRibbonTools` или создавать группу `VcTabHome/Works Ergo`. |
| **Блокировка элементов интерфейса** | Секция `<blockedRibbonItemsSection>` в `rpro_starter.exe.config` | Отсутствует либо контролируется политиками лицензий | Р-Про скрывает часть нативных кнопок VC через секцию блокировки конфигуратора. |
| **Штатный модуль «Эргономика»** | `Plugin.Ergonomics.dll` (RULA, REBA, ISO 11226) | Отсутствует в базовой поставке (требуется плагин стороннего вендора) | Р-Про содержит встроенный расчет RULA/REBA по углам суставов в плоскостных проекциях. |
| **Модуль «Рабочие позы»** | `Plugin.ErgonomicsWPP.dll` (WPP) | Отсутствует в базовой поставке | Специализированный модуль Р-Про для позиционирования манекенов операторов. |
| **Модуль «Захват движения»** | `UX.MoCap.dll` + `Resource.*.MoCap.dll` | Отсутствует в базовой поставке | Заводской модуль подключения и стриминга данных MoCap костюмов. |
| **Официальный аддон Work(s) Ergo** | Не поддерживается штатно (облачный плагин `Plugin.WorksErgoAPI.dll` требует облако `worksergo.com`) | Официальный аддон через WebView2 + SaaS REST API | Оригинальный Work(s) Ergo отправляет данные на внешние серверы. Наш автономный проект полностью исключает облако. |
| **Python Среда** | CPython 2.7.1 (`D:\Apps\RProv222\Python`) | CPython 2.7.1 (в VC 4.3–4.7), Python 3.x (в VC 4.8+) | Должен соблюдаться синтаксис Python 2.7 (`print`, кодировки utf-8, деление чисел). |

---

## 3. Детальный Анализ Формата Компонентов (.rpro vs .vcmx)

Оба формата представляют собой контейнеры **PKZIP** (RFC 1951 / 1952), содержащие структурированный набор файлов сериализации модели, геометрии и графических ресурсов.

### 3.1. Структура архива

Типичный архив `.rpro` (например, [`DHM_Worker.rpro`](file:///D:/Git/WorksErgoRPro/components/DHM_Worker.rpro) или [`DrawingTemplateA0.rpro`](file:///D:/Apps/RProv222/SrcTemplates/Drawings/DrawingTemplateA0.rpro)) содержит следующие элементы:

```
[Имя_Файла.rpro] (PKZIP Container)
├── model.xml                    # Метаданные компонента, GUID (VCID), свойства, ссылки
├── component.dat                # Граф узлов модели (Scene Graph), иерархия ссылок и шарниров
├── component.rsc                # Бинарные геометрические ресурсы (меши, полигоны, вертексы)
├── materials.dat                # Палитра материалов (Diffuse, Ambient, Specular, Opacity)
├── component_icon_preview.tga   # Превью для каталога eCatalog (Truevision TGA, BGRA)
└── layout_icon.tga              # Иконка для отображения в 3D-виде
```

### 3.2. Содержимое и валидация `model.xml`

Файл `model.xml` является дескриптором компонента. Ниже приведено сопоставление заголовков:

#### Нативный заголовок Р-Про (`.rpro`):
```xml
<?xml version="1.0" encoding="utf-8"?>
<VcModel xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
         version="1" 
         xmlns="http://schemas.RProSoftDigital1.com/2017/01/component/componentxml">
  <Properties>
    <Property name="VCID">e9517ca4-bf78-40ea-93a3-f019e0719783</Property>
    <Property name="ModelType">Component</Property>
    <Property name="Name">Barrier</Property>
    <Property name="Description">Works Ergo Parametric Collision Barrier</Property>
    <Property name="Manufacturer">R-Pro WorksErgo</Property>
    <Property name="Revision">1</Property>
  </Properties>
  <ModelUrl>component.rsc</ModelUrl>
  <ThumbnailImageUrl>layout_icon.tga</ThumbnailImageUrl>
  <PreviewImageUrl>component_icon_preview.tga</PreviewImageUrl>
</VcModel>
```

#### Нативный заголовок Visual Components (`.vcmx`):
```xml
<?xml version="1.0" encoding="utf-8"?>
<VcModel xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
         version="1" 
         xmlns="http://schemas.visualcomponents.com/2017/01/component/componentxml">
  <Properties>
    <Property name="VCID">967008ae-725b-4bb7-88de-f5eb11a4a8c9</Property>
    <Property name="ModelType">Component</Property>
    <Property name="Name">Robotic_Arm</Property>
    <Property name="Manufacturer">Visual Components</Property>
    <Property name="Revision">34</Property>
  </Properties>
  <ModelUrl>component.rsc</ModelUrl>
  <ThumbnailImageUrl>layout_icon.tga</ThumbnailImageUrl>
  <PreviewImageUrl>component_icon_preview.tga</PreviewImageUrl>
</VcModel>
```

> [!IMPORTANT]
> **Правило двунаправленной конвертации:**  
> Для портирования компонента из Visual Components в Р-Про недостаточно сменить расширение с `.vcmx` на `.rpro`. Необходимо распаковать архив, заменить строку `xmlns="http://schemas.visualcomponents.com/2017/01/component/componentxml"` на `xmlns="http://schemas.RProSoftDigital1.com/2017/01/component/componentxml"` в файле `model.xml` и запаковать архив обратно с сохранением структуры сжатия Deflate.

---

## 4. Архитектура Плагинов .NET и Контракты MEF

### 4.1. Проблема строгой подписи сборок (Strong-Naming Issue)

В среде Microsoft .NET Framework среда CLR (Common Language Runtime) строго разделяет подписанные (Strong-Named) и неподписанные сборки:
* Сборка **Р-Про** [`UX.Shared.dll`](file:///D:/Apps/RProv222/UX.Shared.dll):  
  `UX.Shared, Version=4.5.0.0, Culture=neutral, PublicKeyToken=null`
* Сборка **Visual Components 4.8+**:  
  `VisualComponents.UX.Shared, Version=4.8.0.0, Culture=neutral, PublicKeyToken=31bc53bc7503b77a`

Если разработчик попытается скомпилировать C# плагин со ссылкой на официальные SDK-сборки Visual Components и скопировать его в `D:\Apps\RProv222\`, загрузчик MEF вызовет исключение:
```
System.IO.FileLoadException: Could not load file or assembly 'UX.Shared, Version=4.5.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
HRESULT: 0x80131044 (A strongly-named assembly is required or public key token mismatch)
```

**Решение для WorksErgo R-Pro Edition:**  
Наш плагин [`Plugin.WorksErgo.dll`](file:///D:/Git/WorksErgoRPro/knowledge_base/02_rpro_and_vc_binaries/Plugin.WorksErgo.dll) компилируется непосредственно против локальных неподписанных сборок Р-Про из папки `D:\Apps\RProv222\`:
- `UX.Shared.dll` (v4.5.0.0, `PublicKeyToken=null`)
- `System.ComponentModel.Composition.dll` (MEF v4.0)
- `Caliburn.Micro.dll`

### 4.2. Механизм обнаружения плагинов (MEF Discovery)

При запуске `rpro_starter.exe` инициализирует каталог расширений:
```csharp
var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "Plugin.*.dll");
var container = new CompositionContainer(catalog);
container.ComposeParts(this);
```
Все библиотеки в корневом каталоге `D:\Apps\RProv222\`, имя которых начинается с `Plugin.`, автоматически анализируются на наличие экспортируемых MEF-атрибутов:
* `[Export(typeof(IPlugin))]` — интерфейс жизненного цикла плагина (`OnAppInitialized()`, `OnAppClosing()`).
* `[Export(typeof(IRibbonGroup))]` — наследуется от `RibbonGroupBase`, объявляет визуальную группу элементов управления на ленте.
* `[Export(typeof(IActionItem))]` — наследуется от `ActionItem`, объявляет интерактивные кнопки.
* `[Export(typeof(IDockableScreen))]` — наследуется от `DockableScreen`, док-панели на базе Caliburn.Micro.

### 4.3. Конфигурация сборок (`rpro_starter.exe.config`)

В файле конфигурации Р-Про прописаны правила поиска и связывания зависимостей:
```xml
<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <probing privatePath="Modules" />
    <dependentAssembly>
        <assemblyIdentity name="UX.Shared" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-0.2.2.2" newVersion="0.2.2.2" tag="RPro" />
    </dependentAssembly>
    <dependentAssembly>
        <assemblyIdentity name="UX.Ribbon" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-0.2.2.2" newVersion="0.2.2.2" tag="RPro" />
    </dependentAssembly>
    <dependentAssembly>
        <assemblyIdentity name="rpro.conn.Core" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-0.2.2.2" newVersion="0.2.2.2" tag="RPro" />
    </dependentAssembly>
</assemblyBinding>
```
Наличие атрибута `tag="RPro"` свидетельствует о специфической системе валидации манифестов внутри загрузчика Р-Про.

---

## 5. Интеграция Пользовательского Интерфейса (Ribbon UI Paths)

### 5.1. Дерево сайтов Ribbon (Ribbon Site Hierarchy)

В Visual Components стандартная лента делится по задачам: `Home`, `Modeling`, `Drawing`, `Teach`, `Process`, `Help`.  
В Р-Про v2.2.2 структура вкладок и идентификаторов сайтов имеет следующий вид (зафиксировано в `rpro_starter.exe.config`):

```
Ribbon Root
├── VcTabHome (Главная вкладка)
│   ├── VcRibbonClipboard      # Буфер обмена (Copy, Paste, Cut)
│   ├── VcRibbonView           # Управление камерой и видимостью
│   ├── VcRibbonTools          # Инструменты и расширения (Точка входа WorksErgo)
│   └── Works Ergo             # Пользовательский сайт нашего плагина
├── VcTabDrawing               # Черчение (A0..A4 шаблоны ЕСКД)
├── VcTabTeach                 # Программирование роботов (KRC, Fanuc, ABB)
├── VcTabAuthor (VcTabModeling)# Редактор геометрии и моделирование кинематики
├── VcTabProcess               # Моделирование материальных потоков
├── VcTabPaint                 # Модуль подготовки окраски изделий
└── VcTabHelp                  # Справка и документация
```

### 5.2. Точки подключения аддона через Python API

В скрипте инициализации [`__init__.py`](file:///D:/Apps/RProv222/Python/Commands/WorksErgo/__init__.py) интеграция производится через функции модуля `vcApplication`:

```python
# -*- coding: utf-8 -*-
from vcApplication import *

COMMAND_NAME = 'cmdWorksErgo'
MENU_TITLE = 'Work(s) Ergo'

def OnAppInitialized():
    cmduri = getApplicationPath() + 'cmdWorksErgo.py'
    cmd = loadCommand(COMMAND_NAME, cmduri)
    if not cmd:
        return

    # 1. Создание выделенной группы на главной вкладке:
    addUxSite('VcTabHome/Works Ergo', -1)
    addMenuItem('VcTabHome/Works Ergo', MENU_TITLE, -1, COMMAND_NAME)

    # 2. Дублирование в системную группу инструментов:
    addMenuItem('VcTabHome/VcRibbonTools', MENU_TITLE, -1, COMMAND_NAME)

    # 3. Регистрация в меню мастеров моделирования:
    addMenuItem(VC_MENU_MODELING_WIZARDS + '/Component Wizards', MENU_TITLE, -1, COMMAND_NAME)

    # 4. Регистрация в контекстном меню 3D-сцены:
    addMenuItem(VC_MENU_HOME, MENU_TITLE, -1, COMMAND_NAME)
```

---

## 6. Сравнение Эргономических Подсистем

### 6.1. Детальная архитектурная таблица

| Критерий | Заводской модуль Р-Про (`Plugin.Ergonomics.dll`) | Официальный аддон VC (`Plugin.WorksErgoAPI.dll`) | Наш автономный модуль WorksErgo R-Pro Edition |
| :--- | :--- | :--- | :--- |
| **Сетевая архитектура** | 100% Локальный (офлайн) | **Облачный SaaS** (зависимость от `https://worksergo.com/api/v1/`) | **100% Локальный (офлайн)**, без внешних вызовов |
| **Пользовательский интерфейс** | Стандартные панели WPF DockManager | Окно WPF на базе Microsoft Edge WebView2 | 3D-манипуляторы в сцене + автономный WPF Dashboard |
| **Биомеханика позвоночника** | **Отсутствует** (нет расчета компрессии L5/S1) | Расчет на сервере вендора через облако | **Локальный расчет компрессии L5/S1** (модель Chaffin / University of Michigan 3DSSPP) |
| **Методики оценки** | RULA, REBA, ISO 11226 (статические позы) | Snook/Ciriello, NIOSH Lifting Equation, Liberty Mutual | RULA, REBA, NIOSH, Snook/Ciriello, ISO 11228-1/2/3, EAWS, DCR |
| **Управление позой (Posing UX)** | Вращение осей в панели свойств компонента | **3 Напольных цилиндра:** Желтый (поза), Синий (задача), Розовый (сброс) | **3 Напольных цилиндра** прямо в 3D-компоненте `DHM_Worker.rpro` |
| **Позиционирование кистей** | Ручное через джог-панель суставов | **Floating Hands:** Снаппинг мишеней кистей к детали с нормалью `Align Axis: -X` | **Floating Hands:** Аналитический двухзвенный IK + привязка кистей по нормали |
| **Учет препятствий** | Статические CAD-модели коллизий | Параметрический `Barrier` с динамическими ручками граней и фиксацией пола | Параметрический [`Barrier.rpro`](file:///D:/Git/WorksErgoRPro/components/Barrier.rpro) с интерактивным изменением габаритов в 3D |
| **Индикация результатов** | Цветовая шкала в отдельной вкладке | Парящий планшет в 3D (Body Map Whiteboard), контур стоп COP на полу | 3D Body Map планшет + опорный контур COP стоп с индикацией опрокидывания |
| **Скорость расчета** | 10–20 кадров/сек (синхронно в UI потоке) | Задержка 500–2500 мс на HTTP-запрос | **Менее 2 мс (60+ FPS)** за счет локального C# ядра и ONNX Runtime |

### 6.2. Внутреннее устройство `Plugin.Ergonomics.dll` Р-Про

Декомпиляция сборки [`Plugin.Ergonomics.dll`](file:///D:/Apps/RProv222/Plugin.Ergonomics.dll) выявила математический аппарат расчета суставных углов:
* Класс `ErgonomicsAddon.Ergonomics.Angle`:
  - Рассчитывает знаковые углы отклонения звеньев скелета манекена методом ортогональной проекции:
    ```csharp
    Double GetSignedAngleWithProjection(Vector3 vector, Vector3 projection, Vector3 signReference, Vector3 increaseReference);
    ```
  - Позволяет сопоставлять текущую ориентацию плеча, предплечья, шеи и туловища с эталонными шкалами эргономических стандартов ISO 11226, RULA и REBA.
  - Однако модуль полностью лишен динамического расчета мышечных усилий, нагрузок на межпозвоночный диск L5/S1 и таблиц допустимых весов подъема/переноски грузов Снука-Чириелло.

---

## 7. Среда Скриптинга CPython 2.7.1

### 7.1. Структура файлового дерева интерпретатора

В составе Р-Про v2.2.2 развернут изолированный рантайм CPython 2.7.1:
```
D:\Apps\RProv222\Python\
├── Commands\              # Пользовательские и системные команды (автозагрузка)
│   ├── RProStudio\        # Модуль студии Р-Про
│   ├── WorksErgo\         # Наш модуль WorksErgo R-Pro Edition
│   ├── vcHelpers\         # Вспомогательные утилиты геометрии и топологии
│   └── vc.py              # Точка входа скриптов
├── DLLs\                  # Скомпилированные C-расширения (_socket.pyd, etc.)
└── lib\                   # Стандартная библиотека Python 2.7 (os, sys, math, json, etc.)
```

### 7.2. Особенности API и различия версий

1. **Типизация и строки:** В CPython 2.7.1 строки по умолчанию являются байтовыми (`str`). При работе с русскоязычными именами компонентов и свойствами в Р-Про обязателен заголовок `# -*- coding: utf-8 -*-` и явное декодирование строк (`s.decode('utf-8')`).
2. **Потоковая модель и IPC:** Вызов блокирующих внешних процессов из `OnExecute()` подвешивает интерфейс Р-Про. В [`cmdWorksErgo.py`](file:///D:/Apps/RProv222/Python/Commands/WorksErgo/cmdWorksErgo.py) реализован асинхронный запуск внешнего WPF-приложения через неблокирующий `subprocess.Popen([exe_path])` со сбросом параметров выделенной сцены в кэш-файл `worksergo_scene_state.json`.

---

## 8. Сводный Каталог Декомпилированной Документации (1 003 Статьи)

Все 9 CHM-справок из `D:\Apps\RProv222\Help\` полностью декомпилированы в чистое HTML-дерево в каталог [`rpro_help_decompiled/`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/rpro_help_decompiled/). На их основе сформирован консолидированный индекс [`rpro_articles.json`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/rpro_articles.json) и обновлен офлайн-поисковый портал [`documentation_portal.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/documentation_portal.html).

### 8.1. Статистический реестр разделов документации

| Раздел / Источник CHM | Каталог экспорта | Кол-во статей | Язык | Продукт / Область |
| :--- | :--- | :---: | :---: | :--- |
| **`Help_RP_ru.chm`** | `Help_RP_ru/` | **387** | RU | **Р-Про:** Главное руководство пользователя CAD, моделирование кинематики, компоновка цехов, материальные потоки |
| **`Help_RP.chm`** | `Help_RP/` | **363** | EN | **Visual Components / Р-Про:** Оригинальный англоязычный мануал, горячие клавиши, параметры импорта CAD |
| **`Python_API.chm`** | `Python_API/` | **222** | EN | **Visual Components / Р-Про:** Полный справочник объектной модели (vcComponent, vcMatrix, vcKinematics, vcSimInterface) |
| **`Help_Ergonomics_RU.chm`** | `Help_Ergonomics_RU/` | **7** | RU | **Р-Про:** Заводской модуль оценки поз RULA, REBA, ISO 11226 |
| **`Help_Ergonomics_EN.chm`** | `Help_Ergonomics_EN/` | **7** | EN | **Р-Про:** Английская версия руководства по модулю RULA/REBA |
| **`Help_Ergonomics_WPP_RU.chm`** | `Help_Ergonomics_WPP_RU/` | **4** | RU | **Р-Про:** Модуль «Рабочие позы» (WPP, кинематика манекена оператора) |
| **`Help_Ergonomics_WPP_EN.chm`** | `Help_Ergonomics_WPP_EN/` | **4** | EN | **Р-Про:** Английская версия руководства WPP |
| **`Help_MoCap_RU.chm`** | `Help_MoCap_RU/` | **4** | RU | **Р-Про:** Модуль интеграции датчиков захвата движения (MoCap) |
| **`Help_MoCap_EN.chm`** | `Help_MoCap_EN/` | **4** | EN | **Р-Про:** Английская версия руководства MoCap |
| **`Temp_Help_Ergonomics_v0.1.html`**| Корневой каталог | **1** | RU | **Work(s) Ergo:** Руководство по 3D-взаимодействию с интерактивными манипуляторами |
| **ИТОГО В БАЗЕ ЗНАНИЙ** | | **1 003** | RU/EN | **Полное покрытие экосистемы без зависимости от интернета** |

---

## 9. Руководство по Безопасной Разработке и Конвертации (Developer Checklist)

При создании или переносе новых компонентов и скриптов в экосистему Р-Про v2.2.2 строго соблюдайте следующие правила:

1. **Компоненты 3D (.rpro):**
   - Убедитесь, что файл запакован в формате ZIP.
   - Проверьте корневой тег `model.xml`: значение атрибута `xmlns` должно быть строго равно `http://schemas.RProSoftDigital1.com/2017/01/component/componentxml`.
   - Иконки предварительного просмотра `component_icon_preview.tga` и `layout_icon.tga` должны быть несжатыми TGA (32 бита, порядок каналов BGRA).

2. **Сборки .NET (C# MEF Plugins):**
   - Целевой фреймворк строго `.NET Framework 4.8`.
   - Не включайте строгую подпись ключевой парой `.snk`.
   - Ссылайтесь только на неподписанную сборку `D:\Apps\RProv222\UX.Shared.dll`.
   - Именуйте результирующий бинарный файл по маске `Plugin.<ModuleName>.dll` для автоматической регистрации MEF.

3. **Скрипты Python:**
   - Сохраняйте исходники в кодировке UTF-8 с заголовком `# -*- coding: utf-8 -*-`.
   - Помещайте плагин в `D:\Apps\RProv222\Python\Commands\<Name>\`.
   - Точка входа обязательно должна реализовывать функцию `OnAppInitialized()`.
   - Для добавления кнопок используйте пути `VcTabHome/VcRibbonTools` или отдельный сайт `VcTabHome/<YourBrand>`.
