# Works Ergo R-Pro Edition • Научно-инженерная база знаний и архитектурный архив

> **Назначение каталога:** Центральное хранилище всех теоретических исследований, математических выводов, инженерных формул, архитектурных контрактов и спецификаций проекта **Works Ergo R-Pro Edition** (нативный плагин биомеханики и эргономики для Р-Про v2.2.2 / Visual Components).  
> **Статус:** Единый источник правды (Single Source of Truth). Регулярно обновляется при каждом новом исследовании, интеграции и расширении функционала.

---

## 🗺️ Карта документации и исследований

| Документ | Раздел | Описание содержания |
| :--- | :--- | :--- |
| [**01_BIOMECHANICAL_ENGINE_SPECIFICATION.md**](01_BIOMECHANICAL_ENGINE_SPECIFICATION.md) | Биомеханика & Математика | Полный математический вывод 7 осей эргономики: L5/S1 Jäger (2023), LCFCD Brinckmann (1988), LM-MMH Potvin (2021), Potvin MAE (2012), AFF ANN (2017), HandPak, шея, 3 стиля подъема (Stoop/Squat), кинематика ног, баланс CofP и площадь опоры BoS. |
| [**02_RPRO_MEF_ARCHITECTURE.md**](02_RPRO_MEF_ARCHITECTURE.md) | Архитектура ПО & .NET MEF | Внутреннее устройство Р-Про v2.2.2: неподписанные сборки CLR (`PublicKeyToken=null`), интеграция в ленту WPF Ribbon, Caliburn.Micro DockableScreen, Infragistics DockManager, мост с CPython 2.7. |
| [**03_AXIS_STUDIO_MOCAP_PIPELINE.md**](03_AXIS_STUDIO_MOCAP_PIPELINE.md) | MOCAP & Записи с датчиков | Анализ реальных файлов захвата движения Axis Studio (Noitom Perception Neuron, 96 Гц) из `D:\Задания, Уроки\Запись с датчиков\`. Парсер BVH, анатомия скелета, устранение артефактов "движения манекена вокруг груза", привязка стоп к полу. |
| [**04_AI_SURROGATE_AND_DATASET_ROADMAP.md**](04_AI_SURROGATE_AND_DATASET_ROADMAP.md) | ИИ-суррогат & Датасет | Пайплайн генерации синтетических выборок, обучение легковесной нейросети (MLP / ONNX) на реальном MOCAP и инференс 60 FPS внутри плагина Р-Про для естественной биодинамики подъема. |
| [**05_HUMAN_POSTURE_BALANCE_AND_COM_DERIVATION.md**](05_HUMAN_POSTURE_BALANCE_AND_COM_DERIVATION.md) | Равновесие, CoM & Кинематика ног | Полный вывод многозвенного центра тяжести (Winter/Dempster), замкнутая кинематическая цепь нижних конечностей (IK), 3 стиля приседаний (тазодоминантный, коленодоминантный, сбалансированный), физическая линия гравитации (Plumb Line) и устранение падения назад на основе MoCap (22.9k кадров). |
| [**06_BIOMECHANICS_TEXTBOOKS_AND_GLOBAL_DATASETS_COMPENDIUM.md**](06_BIOMECHANICS_TEXTBOOKS_AND_GLOBAL_DATASETS_COMPENDIUM.md) | Академический базис & Мировые датасеты | Академический компендиум биомеханических учебников (Winter 2009, Chaffin 2006, Potvin 2012-2026, Jäger 2023), обзор открытых мировых датасетов (AMASS, KIT Whole-Body, HuMoD), влияние анатомии нижних конечностей и архитектурная связь с Р-Про CAD API. |
| [**07_COMPREHENSIVE_SCIENTIFIC_REGISTRY_36_SOURCES.md**](07_COMPREHENSIVE_SCIENTIFIC_REGISTRY_36_SOURCES.md) | Полный реестр & Рецензия ядра | Детальная декомпозиция всех 36 публикаций и 7 эмпирических датасетов Work(s) Ergo, глубокая рецензия текущего кода, анализ ограничений (квазистатика, примитивные коллизии) и манифест архитектуры Next-Gen Ergonomics Engine. |
| [**08_MASTER_ENCYCLOPEDIA_OF_ERGONOMICS_AND_DHM.md**](08_MASTER_ENCYCLOPEDIA_OF_ERGONOMICS_AND_DHM.md) | Генеральная энциклопедия DHM | Фундаментальный синтез 6 дисциплин (вычислительная кинематика FABRIK/QP-IK, пространственная динамика RNEA, мышечные модели Хилла, 3D антропометрия SMPL-X/CAESAR, моторный контроль Flash-Hogan, стандарты ISO/EAWS/OCRA). |
| [**09_CRITICAL_PEER_REVIEW_AND_NEXTGEN_CORE_SPECIFICATION.md**](09_CRITICAL_PEER_REVIEW_AND_NEXTGEN_CORE_SPECIFICATION.md) | Рецензия & Спецификация ядра | Бескомпромиссный аудит кодовой базы, математические уравнения 3D EMA полинома L5/S1, выборки ДКВ, 23 хватов HandPak и пошаговый план модернизации ядра. |
| [**10_INDUSTRIAL_DHM_SOFTWARE_ARCHITECTURE_AND_BEST_PRACTICES.md**](10_INDUSTRIAL_DHM_SOFTWARE_ARCHITECTURE_AND_BEST_PRACTICES.md) | Промышленная архитектура DHM | Анализ передовых систем (Siemens Jack, Dassault DELMIA, AnyBody), 5-уровневая архитектура CAD-аддона, двухуровневая кинематика, динамика $F=m(g+a)$, бессерверное ядро и нулевой опрос сцены. |

---

## 🏛️ Архитектурные принципы проекта

1. **Zero Hallucination (Нулевая аппроксимация «на глаз»):**
   Все расчетные формулы, пределы прочности, коэффициенты и пороги риска строго соответствуют опубликованным рецензируемым статьям (ISO 11228, DIN EN 1005, Jäger 2023, Brinckmann 1988, Potvin 2012/2021) и эталонному мануалу Work(s) Ergo v1.17 (стр. 39–40).

2. **Zero Cloud Dependency (100% автономность):**
   Оригинальный плагин Visual Components требовал платной облачной подписки на `worksergo.com` через WebView2. Наш плагин для Р-Про v2.2.2 является на 100% автономным, работающим на локальном математическом ядре и локальных ИИ-суррогатах без обращения к сети.

3. **Соблюдение стабильности WPF Р-Про:**
   - Все свойства и команды используют исключительно 7-битный ASCII (`[A-Za-z0-9_]`).
   - Сборка компилируется без строгой подписи напрямую против библиотек из `D:\Apps\RProv222\`.
   - Инициализация происходит строго в `OnAppInitialized()`.

4. **Непрерывное сохранение опыта (Continuous Learning):**
   Любые исправления, калибровки формул, новые типы захватов или датчиков фиксируются в этом каталоге и в файле памяти `C:\Users\Jojo\.gemini\config\GEMINI.md`.
