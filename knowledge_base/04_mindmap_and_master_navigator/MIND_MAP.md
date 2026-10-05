# Интеллект-Карта Экосистемы WorksErgo R-Pro Edition (Master Mind Map)

> **Назначение документа:** Центральный навигатор по всей экосистеме проекта «WorksErgo R-Pro Edition». Связывает воедино 3D-механику CAD, 28 видеоисследований, академическую базу из 36 первоисточников, датасеты, бинарные сборки и портал документации.

---

## 1. Графическая диаграмма связей (Mermaid Mind Map)

```mermaid
mindmap
  root((WorksErgo R-Pro Edition))
    [3D CAD Взаимодействие]
      Floating Hands
        Snapping инструмент
        Align Axis '-X' ладонь перпендикулярно
        3D векторы сил синий/оранжевый
        Attach Hands переключатель
      3D Напольный Контроллер
        Зеленая пластина Work-s Ergo
        Желтый цилиндр MultiPose
        Синий цилиндр Task Types
        Розовый цилиндр Reset Ready Pose
      Интерактивный Барьер
        Ресайз 6 граней мышью
        Центр origin всегда фиксирован
        Body Bracing упор телом
      Парящие HUD и Разметка
        Body Map Whiteboard планшет
        MultiPose floating селектор
        COP разметка на полу 4 квадранта
      Отчетность
        Экспорт в Excel .xlsx
        Кривая перцентилей DCR
        Кнопка Open Subfolder
    [28 Демонстрационных Видео @idkfa3]
      Базовые манипуляции 01-09
        01 Floating Hands и MultiPose
        02 Copy and Paste
        03 Obstructed Eye Gaze
        04-05 Hand Tasks и Векторы Сил
        06 Body Bracing
        07 Import Environment
        08 Straight Legs
        09 Batch Analysis
      Индустриальные кейсы 51-60
        51-52 Multiple DHM
        53-54 Task Types и MultiPose
        55 Embed Images and Videos
        56-57 item верстаки v1 и v2
        58 Office Ergonomics
        59 Candy-Wrapper Tests
        60 Teamwork in Office
      Интеграция 61-68
        61 Bosch Rexroth
        62 Detroit Murals
        63 Experience Web
        64 NVIDIA Omniverse USD
        65 Pose-to-Pose Animation
        66 Above Shoulder Correction
        67 LM-MMH Snook
        68 VR testing
    [Научное Ядро 36 Источников]
      Вычислительная Кинематика
        FABRIK Aristidou 2011
        DLS Buss 2004
        TrajOpt Schulman 2014
      Биомеханика L5/S1
        Chaffin SSPP 3400 Н
        Potvin 2012 MAE усталость
        Jager Dortmund Atlas
        Brinckmann усталость позвонков
      Баланс CoM и BoS
        Dempster-Winter CoM
        Kingma F=ma динамика
        BoS лодыжка -70 до +180 мм
      Психофизика MMH
        Snook-Ciriello LM-MMH
        NIOSH RWL и LI
        HandPak 23 хвата
        LaDelfa AFF силовые поля
      Стандарты Аудита
        EAWS автоконцернов BMW VW
        ISO 11228-1/2/3 и ISO 11226
        ГОСТ Р 56644-2015
        RULA и REBA
    [Датасеты и ИИ-Суррогат]
      MoCap Пользователя
        D:/Задания Уроки/Запись с датчиков
        12 BVH файлов 22907 кадров 96 Гц
        Perception Neuron датчики
      Мировые Датасеты
        ANSUR II 6068 замеров
        CAESAR 4400 3D сканов
        AMASS SMPL-X
        OpenSim Rajagopal
      Пайплайн Нейросети
        PyTorch MLP
        worksergo_surrogate.onnx
        C# ONNX Runtime 60 FPS
    [Системная Архитектура]
      Нативный .NET MEF
        Plugin.WorksErgo.dll .NET 4.8
        UX.Shared.dll unsigned
        WorksErgoSuite.exe
      Компоненты eCat
        DHM_Worker.rpro
        Barrier.rpro
      Портал Документации
        documentation_portal.html 625 статей
        Help_RP_RU руководство CAD
        Python_API справочник
```

---

## 2. Индекс Ключевых Файлов и Ресурсов

### Раздел 0: Анализ 28 Видео
* 📄 [`knowledge_base/00_video_analysis/MASTER_28_VIDEOS_ANALYSIS.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/00_video_analysis/MASTER_28_VIDEOS_ANALYSIS.md) — Исчерпывающий разбор механики, логики и визуального поведения всех 28 видео с временными метками.
* 📄 [`knowledge_base/00_video_analysis/all_28_videos.json`](file:///C:/Users/Jojo/.gemini/antigravity/scratch/all_28_videos.json) — Метаданные, описания и длительности видео.

### Раздел 1: Научные Труды и Датасеты
* 📄 [`knowledge_base/01_scientific_papers_and_datasets/SCIENTIFIC_REGISTRY_AND_DATASETS_INDEX.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/01_scientific_papers_and_datasets/SCIENTIFIC_REGISTRY_AND_DATASETS_INDEX.md) — Мастер-реестр 36 первоисточников и датасетов.
* 📂 `D:\Задания, Уроки\Запись с датчиков\` — Локальный датасет пользователя (12 BVH-файлов, 22 907 кадров).
* 📂 `knowledge_base/02_datasets/` — ANSUR II (CSV-таблицы мужских и женских антропометрических параметров), OpenSim Rajagopal 2016/2023.

### Раздел 2: Бинарные Сборки и Контракты
* 📄 [`knowledge_base/02_rpro_and_vc_binaries/BINARIES_ARCHITECTURE_AND_API_DUMP.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/02_rpro_and_vc_binaries/BINARIES_ARCHITECTURE_AND_API_DUMP.md) — Декомпиляция и архитектурный анализ.
* 📦 `knowledge_base/02_rpro_and_vc_binaries/Plugin.WorksErgoAPI.dll` — Оригинал Work(s) Ergo (облачный клиент).
* 📦 `knowledge_base/02_rpro_and_vc_binaries/Plugin.WorksErgo.dll` — Наше автономное локальное ядро Р-Про v2.2.2.
* 📦 `knowledge_base/02_rpro_and_vc_binaries/UX.Shared.dll` — Системная сборка Р-Про (v4.5.0.0, unsigned).

### Раздел 3: Портал Документации Р-Про и Visual Components
* 🌐 [`knowledge_base/03_cad_documentation_and_help/documentation_portal.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/documentation_portal.html) — **Интерактивный локальный поисковый портал** по 625 темам документации Р-Про и Visual Components.
* 📄 [`knowledge_base/03_cad_documentation_and_help/Temp_Help_Ergonomics_v0.1.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/Temp_Help_Ergonomics_v0.1.html) — Официальное руководство пользователя Work(s) Ergo по 3D-взаимодействию.
* 📄 [`knowledge_base/03_cad_documentation_and_help/Work(s) User Manual v1.17 (3).pdf`](file:///D:/Git/WorksErgoRPro/knowledge_base/03_cad_documentation_and_help/Work(s)%20User%20Manual%20v1.17%20(3).pdf) — 50-страничный регламент расчетов и формул.
* 📂 `knowledge_base/03_cad_documentation_and_help/Python_API/` — 222 декомпилированные страницы официального Python API Visual Components.
* 📂 `knowledge_base/03_cad_documentation_and_help/Help_RP_RU/` — 387 декомпилированных страниц полного руководства пользователя Р-Про.

### Раздел 4: Интеллект-Карта и Интерактивный Навигатор
* 🌐 [`knowledge_base/04_mindmap_and_master_navigator/interactive_mindmap.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/04_mindmap_and_master_navigator/interactive_mindmap.html) — Интерактивная веб-версия интеллект-карты с фильтрацией и живыми переходами.
