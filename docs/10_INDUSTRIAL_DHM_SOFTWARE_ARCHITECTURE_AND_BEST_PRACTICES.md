# Руководство по архитектуре и лучшим практикам проектирования промышленных DHM-систем и CAD-аддонов (Industrial DHM Software Architecture & Best Practices)

> **Назначение:** Обобщение передового мирового опыта проектирования систем цифрового моделирования человека (Digital Human Modeling — DHM) в средах CAD/CAM/CAE (Siemens Tecnomatix Jack, Dassault Systèmes DELMIA Human, AnyBody Technology, Ramsis, Work(s) Ergo) и спецификация целевой архитектуры для аддона Р-Про v2.2.2 / Visual Components.

---

## 1. Сравнительный анализ архитектур мировых лидеров DHM

| Система | Архитектурная парадигма | Кинематика и поза | Биомеханика и динамика | Интеграция с CAD/Сценой |
| :--- | :--- | :--- | :--- | :--- |
| **Siemens Tecnomatix Jack (TAT)** | C++/MFC/.NET гибрид. Ядро кинематики жестко отделено от визуализатора OpenSceneGraph. | Empirical Joint Comfort Optimization + Chaffin SSPP Inverse Kinematics. Поддержка предсказания позы по 3D-целям рук. | Статическая модель поясничного отдела L4/L5, Chaffin 3DSSPP, расчет баланса центра тяжести, Rohmert (усталость). | Прямая привязка к геометрии узлов сборки Tecnomatix Process Simulate, коллизии через PQP/V-Clip. |
| **Dassault DELMIA Human (Safework)** | C++ компонентная архитектура (CAA / V5/V6 Object Model). Высокая модульность. | Антропометрическая параметризация (ANSUR, ISO 7250, CAESAR). Кинематические цепочки с ограничениями суставов (ROM). | Статические моменты в суставах, RULA, REBA, Liberty Mutual, NIOSH, Biomechanical Lower Back. | Нативная интеграция в дерево CATIA/DELMIA Product Structure, FCL/SOLID коллизии. |
| **AnyBody Modeling System** | C++ решатель с декларативным языком AnyScript. Высокая степень детализации. | Kinematic analysis of closed/open chains, кватернионные шарниры, сплайновая интерполяция MoCap. | Полносвязная скелетно-мышечная динамика (100+ мышц Хилла), Quadratic Programming (QP) оптимизация сил мышц $\min \sum (F_i/PCSA_i)^2$. | Экспорт/импорт STL/STEP, работа через COM-интерфейс / Python API. |
| **Work(s) Ergo (Visual Components)** | .NET MEF плагин + Infragistics WPF + WebView2 облачный бэкенд (в оригинале). | Стохастическое сэмплирование поз (DKW-сэмплер) вокруг целевой точки захвата. | Статический L5/S1 (Jäger 2023), Snook & Ciriello (2021), HandPak (2017), кумулятивная усталость LCF (Potvin 2026). | Привязка к координатам в пространстве VC, табличный ввод параметров. |
| **Целевой аддон Next-Gen для Р-Про** | **100% Автономный нативный .NET 4.8 / MEF плагин + Headless Math Engine + Python API**. | **FABRIK + Closed-Chain Leg IK + Dempster CoM Balance + Minimum Jerk Trajectories**. | **Динамика $F = m(g + a)$ (Kingma), 3D EMA полином позвоночника, EAWS, ISO 11228-1/2/3, HandPak**. | **1-Click ActiveSelection граббер деталей, авто-распознавание ручек, OBB/BVH коллизии**. |

---

## 2. Пятиуровневая архитектура промышленного DHM-аддона (5-Tier DHM Engine)

Для обеспечения максимальной производительности (60+ FPS в интерактиве), абсолютной стабильности хост-приложения (Р-Про не имеет права упасть) и академической точности расчетов система организуется в 5 строго изолированных уровней:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ 5. Presentation & UI Layer (WPF / Caliburn.Micro / Modern Ribbon)           │
│    - WorksErgoPaneView (XAML/C#) & WorksErgoPaneViewModel                   │
│    - Светофорная цветовая дифференциация: Green / Yellow / Red              │
│    - Интерактивный DataGrid многоэтапных операций (Subtasks Job Manager)    │
│    - Генератор отчетов PDF / HTML с радарными диаграммами рисков            │
└──────────────────────────────────────┬──────────────────────────────────────┘
                                       │ MVVM Data Binding / INotifyPropertyChanged
┌──────────────────────────────────────▼──────────────────────────────────────┐
│ 4. Standardized Ergonomic Evaluator Pipeline (Strategy Pattern)             │
│    - IErgonomicEvaluator: Evaluate(BiomechanicalState, TaskContext)         │
│      ├── NioshLiftingEvaluator (ISO 11228-1 / RNLE)                         │
│      ├── SnookCirielloEvaluator (ISO 11228-2 / LM-MMH 2021)                 │
│      ├── HandPakInterfaceEvaluator (ISO 11228-3 / 23 интерфейса хвата)       │
│      ├── SpineLumbarCompressionEvaluator (Jäger 2023 / 3D EMA / Gelb 34°)   │
│      ├── EawsAutomotiveEvaluator (Sections 1-4: Postures, Forces, Reps)     │
│      └── CumulativeFatigueEvaluator (Potvin-Agnew 2026 LCF_CD, Brinckmann)  │
└──────────────────────────────────────┬──────────────────────────────────────┘
                                       │ Posture & Dynamic Loads
┌──────────────────────────────────────▼──────────────────────────────────────┐
│ 3. Kinematics, Dynamics & Balance Engine (Pure Compute Core)                │
│    - FABRIK Solver (Forward And Backward Reaching Inverse Kinematics)       │
│    - Closed-Chain Leg Kinematics (Ankle Dorsiflexion, Knee, Hip Shift)      │
│    - Dempster-Winter Center of Mass (CoM) & Base of Support (BoS) Tracker   │
│    - Dynamic Inertial Acceleration Factor: F_total = m * (g + a_hands)      │
│    - Trajectory Generator (Minimum Jerk / Quintic Spline)                   │
└──────────────────────────────────────┬──────────────────────────────────────┘
                                       │ Kinematic Tree & Spatial Geometry
┌──────────────────────────────────────▼──────────────────────────────────────┐
│ 2. CAD Scene Graph Adapter & Host Interop Bridge                            │
│    - Р-Про / Visual Components SDK Bridge (UX.Shared, Caliburn.Micro)       │
│    - 1-Click ActiveSelection Snapping (X, Y, Z, BoundingBox, Mass Property) │
│    - Event Listener: vcApplication.ActiveSelectionChanged / OnSimulationTick│
│    - Defensive Sandboxing: try-catch барьер, 7-bit ASCII свойства           │
└──────────────────────────────────────┬──────────────────────────────────────┘
                                       │ Native CAD Hooks
┌──────────────────────────────────────▼──────────────────────────────────────┐
│ 1. Host Application Infrastructure (Р-Про v2.2.2 / Visual Components 4.x)   │
│    - .NET Framework 4.8 Runtime, Unsigned Assemblies, Infragistics DockSite │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Ключевые архитектурные паттерны и лучшие практики

### Паттерн 1: Headless Biomechanical Compute Kernel (Изолированное расчетное ядро)
* **Правило:** Расчетные биомеханические классы (`ErgonomicMathEngine`, `Spine3DPolynomialSolver`, `EawsAssessmentEngine`) не должны ссылаться ни на один UI-класс (`System.Windows.Controls`) и ни на одну сборку Visual Components (`UX.Shared`, `vcApplication`).
* **Преимущества:**
  1. **Мгновенные Unit-тесты:** Запуск тысяч тестов за 5-10 миллисекунд прямо в консоли или CI/CD без подъема тяжелого CAD-движка.
  2. **Многопоточность:** Ядро потокобезопасно (thread-safe), расчет ансамбля из 1000 поз или Монте-Карло сэмплирование выполняется параллельно через `Parallel.For` без риска заблокировать интерфейс Р-Про.
  3. **Переиспользуемость:** Ядро может подключаться как к нативному C# плагину, так и вызываться из Python через `pythonnet` / COM, или в виде автономной консольной утилиты.

### Паттерн 2: Двухуровневая кинематика (Интерактивная vs Сплайновая траекторная)
В промышленных системах (Jack, Delmia) используются два разных кинематических режима:
1. **Interactive Real-Time Mode (< 5 мс на кадр):**
   * Когда инженер перемещает ползунки или манипулятор в окне 3D, требуется мгновенный отклик 60 FPS.
   * Для этого применяется **аналитическая замкнутая кинематика ног** (Closed-Chain Leg IK) + **эвристический FABRIK для позвоночника и рук**. Итерации сходятся за 2-4 прохода.
   * Контроль баланса: Центр масс ($X_{\text{CoM}}$) проецируется на стопу ($BoS = [-70, +180]\text{ мм}$). При выходе за пределы таз автоматически сдвигается по закону баланса рычагов Демпстера-Винтера: $K \cdot m_{\text{legs}} = T \cdot m_{\text{upper}}$.
2. **Simulation Trajectory Mode (Высокоточный расчет рабочего цикла):**
   * При проигрывании симуляции рабочего процесса берется траектория движения рук оператора.
   * По алгоритму Флэша-Хогана (Flash & Hogan 1985) строится квинтический сплайн с минимальным рывком (Minimum Jerk).
   * Вычисляются мгновенные ускорения $\mathbf{a}(t) = \frac{d^2 \mathbf{x}}{dt^2}$. По формуле Кингмы (Kingma 1996) динамическая сила подъема $F = m(g + a)$ передается в расчет компрессии L5/S1, фиксируя динамические пики перегрузки.

### Паттерн 3: Extensible Assessment Pipeline (Расширяемый конвейер стандартов)
Для добавления новых международных стандартов (например, внедрения EAWS или специфического стандарта автоконцерна BMW/VW) используется паттерн **Strategy / Pipeline**:
* Интерфейс `IErgonomicEvaluator`:
  ```csharp
  public interface IErgonomicEvaluator
  {
      string StandardName { get; }
      ErgonomicEvaluationResult Evaluate(PostureState posture, LoadState load, TaskContext context);
  }
  ```
* Конвейер агрегирует оценки всех стандартов в единую карточку `CompositeErgonomicProfile`:
  - `DCR_Biomechanics` (позвоночник)
  - `DCR_Psychophysical` (Snook & Ciriello)
  - `DCR_HandPak` (кисть и предплечье)
  - `DCR_Niosh` (ISO 11228-1)
  - `EAWS_Score` (Автомобильный стандарт)
  - `RULA_Score` / `REBA_Score` (Позы тела)

### Паттерн 4: Событийная интеграция с CAD (Zero-Polling Event Architecture)
* **Антипаттерн:** Крутить бесконечный цикл `while (true)` или опрашивать выбранный объект по таймеру 10 раз в секунду. Это нагружает процессор и вызывает подергивания 3D-сцены.
* **Лучшая практика Р-Про:**
  1. Подписка на нативное событие выбора объекта: пользователь кликает деталь в 3D-сцене $\to$ срабатывает обработчик $\to$ извлекаются габариты коробки (BoundingBox), масса (`Mass` / `Weight`) и мировая координата $Z$ (высота подъема).
  2. При нажатии «Анализировать выбранный объект» аддон считывает текущую геометрию, передает в `ErgonomicMathEngine` и мгновенно отображает полный аудит без перезагрузки сцены.

### Паттерн 5: Безопасность среды выполнения и гигиена типов CLR
* **Защита от сбоев .NET MEF в Р-Про v2.2.2:**
  - Плагин собирается без строгой подписи (`PublicKeyToken=null`), компилируясь строго против `D:\Apps\RProv222\UX.Shared.dll`.
  - Все строковые идентификаторы команд и свойств регистрируются строго в 7-битном ASCII (`[A-Za-z0-9_]`), исключая кириллические символы в системных ID (защита встроенного CPython 2.7 / CLR WPF от фатального падения).
  - Любое обращение к API Р-Про оборачивается в защитный блок `try { ... } catch (Exception ex) { Logger.LogError(ex); }`.

---

## 4. Контрольный лист соответствия промышленным стандартам (Verification Matrix)

| Модуль | Реализовано в ядре | Запланировано в Next-Gen фазе |
| :--- | :--- | :--- |
| **Биомеханика L5/S1** | Сагиттальная компрессия Chaffin / Jäger 2023, возрастные лимиты 18–65 лет. | Полный 3D EMA полином с учетом латерального изгиба ($M_{\text{lat}}$), скручивания ($M_{\text{twist}}$) и крестцового угла $34^\circ$ Gelb. |
| **Психофизика** | Snook & Ciriello (Liberty Mutual MMH 2021) MAWL для 4 типов задач. | Поддержка 23 специализированных интерфейсов HandPak с коэффициентами трения материалов. |
| **Стандарты ISO** | ISO 11228-1 (NIOSH RNLE), ISO 11228-2 (Push/Pull), RULA/REBA. | EAWS (European Assessment Worksheet) для автопрома, ISO 11228-3 (OCRA). |
| **Кинематика ног** | 3 стиля приседа (Stoop, Semi-Squat, Deep Squat), Dempster CoM баланс. | FABRIK на графе скелета человека + динамика Кингмы $F = m(g + a)$. |
| **Многоэтапный аудит** | Одиночные операции подъема/опускания/переноса. | Таблица многоэтапных операций (DataGrid), расчет кумулятивного повреждения смены $LCF_{CD}$ (Potvin 2026). |

---

## 5. Заключение

Спроектированная архитектура объединяет строгость классических расчетных методов Work(s) Ergo и гибкость современных CAD-движков уровня Siemens Jack и AnyBody. Она гарантирует нулевую нагрузку на UI, математическую точность и полную готовность к сертификации по международным стандартам охраны труда.
