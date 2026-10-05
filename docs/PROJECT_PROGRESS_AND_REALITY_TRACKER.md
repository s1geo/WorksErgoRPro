# Единый Реестр Прогресса, Архитектуры и Аудита Реализации (Master Progress & Reality Tracker)

> **Назначение документа:** Центральный, бескомпромиссный источник правды о фактическом состоянии проекта **WorksErgo R-Pro Edition**. Никакого ложного оптимизма: здесь зафиксировано ровно то, что создано, что работает, что находится в процессе реализации и что еще предстоит сделать.

---

## 1. Сводная панель статуса проекта (High-Level Scorecard)

| Направление / Подсистема | Готовность | Статус | Артефакты и верификация |
| :--- | :---: | :---: | :--- |
| **1. Биомеханическое математическое ядро (`ErgonomicMathEngine.cs`)** | **95%** | **Готово к бою** | 10 научных бенчмарков проходят со 100% точностью (L5/S1 Jäger 2023, Weibull Brinckmann/Potvin 2026, 14 уравнений LM-MMH, Potvin MAE, RULA, REBA, EAWS, Dempster-Winter CoM/BoS, Kingma RNEA динамика). |
| **2. База знаний и справочник CAD Р-Про & VC (1 081 тема)** | **100%** | **Завершено** | Все 9 CHM-справок Р-Про v2.2.2 декомпилированы (387 RU + 363 EN + 222 Python API + 30 эргономика/WPP/MoCap). Построен автономный портал [`knowledge_base/index.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/index.html) с поиском и мультиформатным вьюером. |
| **3. Научные первоисточники и реестр литературы (70+ источников)** | **100%** | **Завершено** | Реестр всех 70+ источников разобран в монографиях. Скачано **26 полных академических PDF** (~180 МБ: Waters NIOSH 1994, NIOSH 1981, Dempster 1955, Grenier 1991, USAF 1964, Robinette CAESAR, de Leva 1996, Potvin 2021, Loper SMPL, Pavlakos SMPL-X, Aristidou FABRIK, Delp OpenSim). |
| **4. Аналитический разбор 28 видеодемонстраций Work(s) Ergo (@idkfa3)** | **100%** | **Завершено** | Исчерпывающий 75 КБ отчет [`MASTER_28_VIDEOS_ANALYSIS.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/00_video_analysis/MASTER_28_VIDEOS_ANALYSIS.md) с посекундным разбором Floating Hands, Snapping, Body Bracing, Straight Legs, LM-MMH, Office Ergo и ссылками на видео. |
| **5. Портативная .NET MEF архитектура плагина (`Plugin.WorksErgo.dll`)** | **90%** | **Готово** | Сборка компилируется без строгой подписи под .NET 4.8 / `UX.Shared.dll` Р-Про v2.2.2. Библиотеки вендора архивированы в `lib/` для сборки на любом ПК. Level 2 MEF-тесты проходят. Док-панель Caliburn.Micro интегрирована. |
| **6. Нативные eCat-компоненты `.rpro` (`Barrier.rpro`, `DHM_Worker.rpro`)** | **65%** | **В разработке** | Пакеты `.rpro` сгенерированы со структурой RSC/VCMD, валидированы тестами Level 4 и развернуты в eCat Р-Про. Базовое кинематическое дерево построено. В разработке: тонкая доводка скрипта `Attach Hands?` и перехвата кликов на интерактивных цилиндрах. |
| **7. ИИ-суррогат позы и динамики (PyTorch / ONNX)** | **15%** | **В разработке** | Датасет MoCap пользователя (12 BVH, 22 907 кадров @ 96 Гц) проверен и структурирован. Написан парсер `mocap_extractor.py`. Предстоит: обучение MLP-сети и экспорт весов `.onnx` для 60 FPS инференса в C#. |
| **ОБЩИЙ ПРОГРЕСС ВСЕГО ПРОЕКТА** | **~67%** | **Активная разработка** | Фундаментальная научная, справочная, архитектурная и тестовая база построена полностью. Фокус смещен на поведение компонентов в 3D и обучение нейросети. |

---

## 2. Полная инвентарная структура репозитория (Repository Map)

```text
D:\Git\WorksErgoRPro\
├── README.md                                                 # Главная страница: быстрый старт в 1 клик, архитектура
├── CHANGELOG.md                                              # История версий (SemVer 2.0.0, Keep a Changelog)
├── LICENSE                                                   # Лицензия проекта
├── build.ps1                                                 # Автоматическая сборка решения через csc.exe / MSBuild
├── deploy.ps1                                                # Развертывание плагина в D:\Apps\RProv222\
├── WorksErgoRPro.sln                                         # Решение Visual Studio / Rider
│
├── bin/                                                      # Скомпилированные бинарные сборки
│   └── Plugin.WorksErgo.dll                                  # Готовый нативный плагин для Р-Про
│
├── components/                                               # Каталог нативных компонентов Р-Про (.rpro)
│   ├── Barrier.rpro                                          # Параметрическое препятствие и зона коллизий
│   └── DHM_Worker.rpro                                       # 18-сочлененный манекен человека с кинематикой
│
├── lib/                                                      # Автономные сборки Р-Про для независимой сборки
│   ├── UX.Shared.dll                                         # Базовые MEF-интерфейсы Р-Про v2.2.2
│   ├── UX.Ribbon.dll                                         # Контракты ленты Ribbon
│   ├── Create3D.Shared.dll                                   # Геометрическое ядро и ISimComponent
│   └── Caliburn.Micro.dll                                    # MVVM-фреймворк пользовательского интерфейса
│
├── src/                                                      # Исходный код системы
│   ├── Plugin.WorksErgo/                                     # C# .NET Framework 4.8 плагин
│   │   ├── Biomechanics/ErgonomicMathEngine.cs               # Аналитическое ядро (L5/S1, Weibull, LM-MMH, CoM)
│   │   ├── Core/                                             # MEF-экспорты (WorksErgoPlugin, ActionItem, RibbonGroup)
│   │   ├── ViewModels/WorksErgoPaneViewModel.cs              # ViewModel боковой док-панели
│   │   ├── Views/WorksErgoPaneView.xaml                      # XAML разметка интерфейса
│   │   └── Resources/                                        # Меши и геометрические примитивы
│   ├── AISurrogate/                                          # Модуль обучения нейросетевого суррогата
│   └── PythonEngine/                                         # Внутрикомпонентные Python-скрипты (.rpro)
│
├── tests/                                                    # 4-уровневая пирамида автоматизированных тестов
│   ├── RunAllTests.ps1                                       # Главный раннер всех тестов (100% успех)
│   ├── unit_math_tests/TestBiomechanicalEngine.ps1           # Level 1: 10 математических бенчмарков
│   ├── assembly_mef_tests/TestAssemblyIntegrity.ps1          # Level 2: Проверка MEF-контрактов и CLR
│   ├── cad_integration_tests/TestWpfViews.ps1                # Level 3: Проверка WPF View и ViewModel
│   └── TestComponentPackages.ps1                             # Level 4: Проверка целостности пакетов .rpro
│
├── tools/                                                    # Инженерные скрипты и конвейеры
│   ├── build_barrier_component.py                            # Генератор компонента Barrier.rpro
│   ├── build_dhm_worker_component.py                         # Генератор компонента DHM_Worker.rpro
│   ├── build_master_portal.py                                # Сборщик справочного портала (1 081 тема)
│   ├── mocap_extractor.py                                    # Извлечение кадров из BVH Perception Neuron
│   └── dump_assemblies.cs                                    # Утилита декомпиляции сборок Р-Про
│
├── docs/                                                     # Техническая документация Diátaxis
│   ├── PROJECT_PROGRESS_AND_REALITY_TRACKER.md               # [ЭТОТ ФАЙЛ] Единый журнал прогресса и архитектуры
│   ├── 00_OFFICIAL_SCIENTIFIC_REGISTRY_EXTRACT.md            # Выжимка формул из мануала Work(s) v1.17
│   ├── 01_BIOMECHANICAL_ENGINE_SPECIFICATION.md              # Математическая спецификация ядерных модулей
│   ├── 02_RPRO_MEF_ARCHITECTURE.md                           # Руководство по архитектуре MEF Р-Про
│   ├── 03_AXIS_STUDIO_MOCAP_PIPELINE.md                      # Конвейер обработки 22 907 кадров MoCap
│   ├── 04_AI_SURROGATE_AND_DATASET_ROADMAP.md                # План обучения MLP/ONNX суррогата
│   ├── 05_HUMAN_POSTURE_BALANCE_AND_COM_DERIVATION.md        # Математический вывод CoM и кинематики ног
│   ├── 06_BIOMECHANICS_TEXTBOOKS_AND_GLOBAL_DATASETS_COMPENDIUM.md # Компендиум глобальных датасетов
│   ├── 07_COMPREHENSIVE_SCIENTIFIC_REGISTRY_36_SOURCES.md    # Академический реестр 36 источников
│   ├── 08_MASTER_ENCYCLOPEDIA_OF_ERGONOMICS_AND_DHM.md       # Главная энциклопедия эргономики
│   ├── 09_CRITICAL_PEER_REVIEW_AND_NEXTGEN_CORE_SPECIFICATION.md # Критическое академическое ревью
│   ├── 10_INDUSTRIAL_DHM_SOFTWARE_ARCHITECTURE_AND_BEST_PRACTICES.md # Архитектурные стандарты Big Tech
│   ├── 11_COMPREHENSIVE_MASTER_ENGINEERING_REPORT_WORKSERGO_RPRO.md # Исчерпывающий мастер-отчет (86 КБ)
│   ├── 12_DHM_WORKER_AND_BARRIER_COMPONENT_SPECIFICATION.md  # Спецификация компонентов eCat
│   └── 13_AUTHENTIC_WORKSERGO_VC_INTERACTION_AND_CAD_PARADIGM.md # Парадигма 3D-взаимодействия
│
└── knowledge_base/                                           # База знаний и интерактивный портал
    ├── index.html                                            # ГЛАВНЫЙ ОФЛАЙН-ПОРТАЛ (1 081 документ)
    ├── master_knowledge_index.json                           # Машинночитаемый индекс портала
    ├── 00_video_analysis/MASTER_28_VIDEOS_ANALYSIS.md        # 75 КБ посекундный анализ 28 видео @idkfa3
    ├── 01_scientific_papers/                                 # 26 скачанных академических PDF (~180 МБ)
    │   ├── DOWNLOADED_PAPERS_MANIFEST.md                     # Опись всех загруженных PDF
    │   ├── Waters_1994_Applications_Manual_Revised_NIOSH_Lifting_Equation.pdf (4.0 МБ)
    │   ├── NIOSH_1981_Work_Practices_Guide_Manual_Lifting.pdf (15.4 МБ)
    │   ├── Dempster_1955_Space_Requirements_Seated_Operator.pdf (17.8 МБ)
    │   ├── Grenier_1991_Hand_Anthropometry_US_Army.pdf (44.9 МБ)
    │   ├── USAF_1964_Human_Mechanics_Four_Monographs.pdf (17.0 МБ)
    │   ├── Robinette_2002_CAESAR_Final_Report.pdf (5.5 МБ)
    │   ├── Loper_2015_SMPL_Body_Model.pdf (39.8 МБ)
    │   ├── Pavlakos_2019_SMPL-X.pdf (9.9 МБ)
    │   ├── Schulman_2014_TrajOpt_Motion_Planning.pdf (3.5 МБ)
    │   ├── Dvoretzky_Kiefer_Wolfowitz_1956_DKW_Inequality.pdf (2.7 МБ)
    │   ├── Potvin_2021_LM_MMH_Equations.pdf (2.1 МБ)
    │   ├── Aristidou_2011_FABRIK_IK.pdf (1.5 МБ)
    │   ├── Pan_2012_FCL_Collision_Queries.pdf (1.4 МБ)
    │   ├── Delp_2007_OpenSim_Biomechanical_Software.pdf (1.1 МБ)
    │   ├── de_Leva_1996_Segment_Inertia_Parameters.pdf (1.0 МБ)
    │   └── ... (еще 11 PDF статей)
    ├── 01_scientific_papers_and_datasets/
    │   └── EXHAUSTIVE_70_SOURCES_ANALYSIS_AND_EXTRACTION.md  # Детальный разбор всех 70+ источников
    ├── 02_datasets/
    │   ├── ANSUR_II_MALE_Public.csv (2.0 МБ, 4 082 человека, 93 замера)
    │   ├── ANSUR_II_FEMALE_Public.csv (1.0 МБ, 1 986 человек, 93 замера)
    │   ├── Rajagopal2016.osim (875 КБ, OpenSim 80 DOF, 138 мышц)
    │   └── RajagopalLaiUhlrich2023.osim (923 КБ)
    ├── 03_cad_documentation_and_help/
    │   ├── documentation_portal.html                         # Локальный портал документации
    │   ├── RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md # Анализ различий Р-Про vs VC
    │   ├── Temp_Help_Ergonomics_v0.1.html                    # Интерактивное руководство Work(s)
    │   └── rpro_help_decompiled/                             # 2 422 декомпилированных файла из CHM
    │       ├── Help_RP_ru/ (387 статей Р-Про RU)
    │       ├── Help_RP/ (363 статьи VC Core EN)
    │       ├── Python_API/ (222 статьи Python API)
    │       ├── Help_Ergonomics_RU/ & Help_Ergonomics/ (14 статей)
    │       ├── Help_Ergonomics_WPP_RU/ & Help_Ergonomics_WPP/ (8 статей)
    │       └── Help_MoCap_RU/ & Help_MoCap/ (8 статей)
    └── 04_mindmap_and_navigator/MIND_MAP.md                  # Интеллектуальная карта знаний
```

---

## 3. Детализация готовности по ключевым вехам (Milestone Breakdown)

### Веха 1: Биомеханическое аналитическое ядро (`ErgonomicMathEngine.cs`) — 95%
* [x] Расчет компрессии L5/S1 по Дортмундскому атласу (Jäger 2023) с полиномом плеча выпрямителя спины 4-го порядка.
* [x] Нормативные пределы $US_{75\%} = 4\,360\text{ Н}$ (женщины) и $5\,210\text{ Н}$ (мужчины) для возраста 42 лет.
* [x] Кумулятивная усталость замыкательных пластинок Вейбулла (Brinckmann 1988, Potvin & Agnew 2026).
* [x] 14 непрерывных уравнений Liberty Mutual MMH (Potvin et al. 2021) для подъема, опускания, толкания, тяги и переноски.
* [x] Предел выносливости Potvin MAE (2012) в зависимости от Duty Cycle.
* [x] 3D-силовое поле руки AFF (LaDelfa & Potvin 2017).
* [x] База 23 хватов кисти HandPak с учетом трения и перчаток.
* [x] Истинный закон сохранения центра тяжести Dempster-Winter ($X_{\text{CoM}} \in [-70, +180]\text{ мм}$) для Stoop, Semi-Squat и Deep Squat.
* [x] Динамический всплеск $F = m(g + a)$ по Kingma (1996) и Featherstone RNEA.
* [x] Матрицы RULA, REBA и стандарт автопрома EAWS.
* [ ] *Осталось (5%):* Тонкая калибровка параметров шеи по Vasavada (2001) при крайних углах наклона головы.

### Веха 2: Декомпиляция и структурирование базы знаний CAD — 100%
* [x] Все 9 CHM-архивов из `D:\Apps\RProv222\Help\` полностью декомпилированы (2 422 файла, 1 002 уникальные статьи).
* [x] Проведен сравнительный анализ отличий Р-Про v2.2.2 от Visual Components 4.8+ (`RPRO_VS_VISUAL_COMPONENTS_DIFFERENCES_AND_DOCS.md`).
* [x] Сформирован структурированный индекс `rpro_articles.json`.

### Веха 3: Научные первоисточники и датасеты — 100%
* [x] Реестр всех 70+ источников полностью изучен с извлечением математики.
* [x] 26 полных томов книг, нормативных отчетов и статей загружены в локальный репозиторий (~180 МБ).
* [x] 6 068 замеров ANSUR II (CSV) и модели OpenSim Rajagopal (.osim) зафиксированы в `02_datasets/`.
* [x] Составлена опись [`DOWNLOADED_PAPERS_MANIFEST.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/01_scientific_papers/DOWNLOADED_PAPERS_MANIFEST.md).

### Веха 4: Анализ 28 видеодемонстраций Work(s) Ergo — 100%
* [x] Все 28 видео с YouTube `@idkfa3` посекундно проанализированы.
* [x] Составлен 75 КБ аналитический регистр [`MASTER_28_VIDEOS_ANALYSIS.md`](file:///D:/Git/WorksErgoRPro/knowledge_base/00_video_analysis/MASTER_28_VIDEOS_ANALYSIS.md).
* [x] Вскрыты 6 ключевых механик взаимодействия: Floating Hands, Zero Manual Joint Posing, 3D Interact Cylinders, Body Bracing, Straight Legs, LM-MMH.

### Веха 5: .NET MEF архитектура и инфраструктура сборки — 90%
* [x] Настроен `Plugin.WorksErgo.csproj` с компиляцией против неподписанной `UX.Shared.dll` (PublicKeyToken=null).
* [x] Вендорные сборки Р-Про архивированы в `lib/` для независимой компиляции на любых машинах.
* [x] Автоматический скрипт сборки `build.ps1` и скрипт деплоя `deploy.ps1`.
* [x] Док-панель `WorksErgoPaneViewModel` на базе Caliburn.Micro.
* [ ] *Осталось (10%):* Финальная стыковка ленты Ribbon Р-Про с вызовом контекстной панели манекена.

### Веха 6: Нативные компоненты Р-Про `.rpro` в eCat — 65%
* [x] Разработаны генераторы `build_barrier_component.py` и `build_dhm_worker_component.py`.
* [x] Сгенерированы валидные пакеты `Barrier.rpro` и `DHM_Worker.rpro` с бинарным форматом RSC (магическая сигнатура `VCMD002804`).
* [x] Компоненты развернуты в пользовательский eCat (`C:\Users\Jojo\Documents\R-Pro\0.2\My Models\WorksErgo\`).
* [x] Написаны тесты проверки целостности Level 4 (`TestComponentPackages.ps1`).
* [ ] *В процессе (35%):* Вживление скрипта плавающих рук (`Attach Hands?`) и перехвата кликов Interact на желтом, синем и розовом манипуляторах в 3D-сцене Р-Про.

### Веха 7: Обучение ИИ-суррогата на MoCap-датасете — 15%
* [x] 12 файлов BVH (22 907 кадров @ 96 Гц) из `D:\Задания, Уроки\Запись с датчиков\` проинспектированы.
* [x] Написан экстрактор кадров и суставных углов `tools/mocap_extractor.py`.
* [ ] *Предстоит (85%):* Обучение компактной нейросети MLP (PyTorch) по предсказанию 24 суставных углов и L5/S1 по координатам кистей рук.
* [ ] Экспорт весов в `worksergo_surrogate.onnx`.
* [ ] Интеграция `Microsoft.ML.OnnxRuntime` в плагин для 60 FPS инференса в 3D-сцене.

---

## 4. Следующие конкретные шаги разработки (Action Plan)

1. **Спринт A (Интерактивность в 3D-сцене):**
   - Настроить в графе `DHM_Worker.rpro` узлы `LeftHandTarget` и `RightHandTarget` с двухзвенным аналитическим IK, чтобы при снаппинге кистей к деталям в сцене торс и ноги автоматически сгибались.
   - Подключить циклический перебор поз InteliPose при клике на желтый цилиндр в режиме Interact.
2. **Спринт B (Обучение ИИ-суррогата):**
   - Запустить конвейер обучения MLP на извлеченных 22 907 кадрах MoCap.
   - Сгенерировать файл `models/worksergo_surrogate.onnx` (< 500 КБ).
   - Подключить ONNX Runtime в C# ядро плагина.
3. **Спринт C (Полномасштабный релиз 1.0.0):**
   - Финальная интеграция и демонстрация в Р-Про v2.2.2.
