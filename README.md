# WorksErgo R-Pro Edition

> **Next-Generation Autonomous Digital Human Modeling (DHM) and Biomechanical Ergonomics System for R-Pro v2.2.2.**  
> 100% standalone native .NET Framework 4.8 extension. Zero cloud dependencies, zero external subscriptions.

---

## Overview

**WorksErgo R-Pro Edition** is an industrial-grade Digital Human Modeling (DHM) and workplace ergonomics CAD plugin engineered natively for **R-Pro v2.2.2** (Visual Components Core). 

While commercial legacy add-ons rely on paid cloud SaaS backends via embedded browser frames (WebView2), **WorksErgo R-Pro Edition** executes all biomechanical solvers, inverse kinematics, spinal compression, psychophysics, and balance physics **100% locally on the host CPU in real time (< 0.1 ms latency, 60+ FPS)**.

### Key Capabilities

| Subsystem | Scientific Foundation | Realized Capabilities |
| :--- | :--- | :--- |
| **Spine Compression** | Jäger (2023) Dortmund Lumbar Load Atlas | L5/S1 compressive force, non-linear 4th-order erector spinae lever arm *h*ₘ(θ), intra-abdominal pressure (*IAP*) relief, US₇₅% population protection thresholds (4,360 N female / 5,210 N male). |
| **Spinal Fatigue** | Brinckmann et al. (1988), Potvin & Agnew (2026) | 2-parameter Weibull CDF cumulative microfracture probability (β = 1.34, log₁₀α = −3.874·LCF + 5.219), Palmgren-Miner damage accumulation across shift cycles. |
| **Psychophysics MMH** | Snook & Ciriello (1991), Potvin et al. (2021) | 14 continuous parametric equations for Maximum Acceptable Weight of Lift (MAWL): Lift, Lower, Push (*F*\_init / *F*\_sust), Pull, and Carry. |
| **Muscle Fatigue** | Potvin (2012) MAE | Maximum Acceptable Effort continuous duty cycle (*DC*) function preventing localized muscle ischemia. |
| **3D Hand Strength** | LaDelfa & Potvin (2017) | Arm Force Field (AFF) 3D isometric hand strength vector prediction relative to shoulder origin. |
| **Grip Biomechanics**| HandPak (23 Industrial Interfaces) | Power grip, chuck pinch, lateral pinch, finger presses with friction coefficient (μ) scaling for oiled steel, dry metal, and rubber gloves. |
| **Closed-Loop Balance**| Dempster (1955), Winter (2009) | Exact multi-link segmental Center of Mass (*X*\_CoM) tracking against Base of Support (BoS: [−70 mm, +180 mm]). Support for 3 squat styles: Stoop, Semi-Squat, Deep Squat. |
| **Spatial Dynamics** | Featherstone (2008), Kingma et al. (1996) | Recursive Newton-Euler Algorithm (RNEA) accounting for dynamic acceleration lift surge (*F* = *m*(*g* + *a*)). |
| **Automotive Audit** | Schaub et al. (2012) EAWS, ISO 11228 | European Assessment Worksheet (BMW, VW, Stellantis standard) and ISO 11228-1/2/3 compliance reporting. |

---

## Repository Architecture

```text
WorksErgoRPro/
├── WorksErgoRPro.sln         # Standard Visual Studio / Rider solution
├── build.ps1                 # Automated build entrypoint
├── deploy.ps1                # 1-click deployment into D:\Apps\RProv222\
├── CHANGELOG.md              # Keep a Changelog (SemVer 2.0.0)
├── README.md                 # Project hero page
│
├── src/                      # Production source code
│   ├── Plugin.WorksErgo/     # Native C# .NET 4.8 / MEF / WPF assembly
│   │   ├── Plugin.WorksErgo.csproj
│   │   ├── Biomechanics/     # Decoupled domain physics (ErgonomicMathEngine.cs)
│   │   ├── Core/             # MEF contracts (SiteSetup, ActionItem, IoC)
│   │   ├── ViewModels/       # Caliburn.Micro DockableScreen viewmodels
│   │   ├── Views/            # WPF XAML views and controls
│   │   └── Resources/        # Embedded 3D manikin geometry meshes
│   ├── AISurrogate/          # ML surrogate model training (PyTorch/ONNX)
│   └── PythonEngine/         # In-CAD component controllers (.rpro)
│
├── tests/                    # Enterprise test pyramid
│   ├── RunAllTests.ps1       # Master test runner (Zero-friction verification)
│   ├── unit_math_tests/      # Mathematical boundary condition validation
│   ├── assembly_mef_tests/   # MEF reflection, assembly loading, export tests
│   └── cad_integration_tests/# UI instantiation and 3D scene binding tests
│
├── docs/                     # Diátaxis-compliant technical documentation
│   ├── 00_OFFICIAL_SCIENTIFIC_REGISTRY_EXTRACT.md
│   ├── 01_BIOMECHANICAL_ENGINE_SPECIFICATION.md
│   ├── 02_RPRO_MEF_ARCHITECTURE.md
│   ├── ...
│   ├── 11_COMPREHENSIVE_MASTER_ENGINEERING_REPORT_WORKSERGO_RPRO.md
│   └── 00_archive/           # Historical extracts and reference dumps
│
├── knowledge_base/           # Local academic papers, datasets & standards
│   ├── 01_scientific_papers/ # Open-access papers (FABRIK, TrajOpt, Buss IK, SMPL-X)
│   ├── 02_datasets/          # ANSUR II CSVs, OpenSim Rajagopal .osim models
│   ├── 03_standards_and_guidelines/ # ISO 11228, ISO 11226, EAWS monographs
│   ├── 04_rpro_cad_internals/# Decompiled R-Pro plugin signatures
│   └── 05_textbooks_and_manuals/    # Gordon 2012 book, Work(s) v1.17 manual
│
└── tools/                    # Automated tooling and pipeline utilities
    ├── build_knowledge_base.py
    ├── dump_assemblies.cs
    └── mocap_extractor.py
```

---

## Quick Start

### Prerequisites
* Windows 10/11 (64-bit)
* .NET Framework 4.8 Developer Pack
* R-Pro v2.2.2 installed at `D:\Apps\RProv222\` (or Visual Components 4.5+)

### 1. Interactive Knowledge Portal (1-Click Start)
To explore all 1,081 topics of official CAD documentation, 28 video analyses, 70+ scientific papers, 26 downloaded PDFs, datasets, and API references:
```powershell
# Open the master offline portal directly in your default browser:
Start-Process "knowledge_base\index.html"
```
Or open [`knowledge_base/index.html`](file:///D:/Git/WorksErgoRPro/knowledge_base/index.html) in Chrome / Edge.

### 2. Build Solution
```powershell
# Run the automated build script (uses csc.exe or MSBuild)
.\build.ps1
```

### 3. Run Test Suite
```powershell
# Execute the full 4-tier test verification suite
.\tests\RunAllTests.ps1
```

### 4. Deploy to R-Pro
```powershell
# Copies compiled assembly to R-Pro root for auto-discovery by MEF DirectoryCatalog
.\deploy.ps1
```

---

## Единый Портал Базы Знаний и Документации

Для быстрого погружения в проект, изучения архитектуры Р-Про и Work(s) Ergo в репозитории развернут **автономный офлайн-портал**:
👉 **[`knowledge_base/index.html`](knowledge_base/index.html)** (или `knowledge_base/03_cad_documentation_and_help/documentation_portal.html`)


### Содержимое портала (1 101 документ с поиском в реальном времени):
1. **Конспекты & Анализ (8 документов):** Мастер-Инженерный отчет (86 КБ), 75 КБ анализ 28 видео (@idkfa3), монография 70+ научных источников, энциклопедия DHM, математический вывод CoM и кинематики ног, критическое академическое ревью, интеллектуальная карта знаний.
2. **Карта действий & Роадмап (4 документа):** Главный журнал прогресса и реальности (Reality Tracker), дорожная карта обучения нейросетевого ИИ-суррогата на MoCap, конвейер извлечения 22 907 кадров Perception Neuron, аудит расхождений и архитектурных пробелов.
3. **Спецификации & Архитектура (8 документов):** Спецификация пакетов компонентов `.rpro` (`DHM_Worker` и `Barrier`), аутентичная парадигма 3D-взаимодействия (Floating Hands, Interact Cylinders), сравнительный анализ Р-Про vs Visual Components (31 КБ), архитектура .NET MEF, декомпилированные API-дампы `Plugin.Ergonomics.dll` и `Plugin.ErgonomicsWPP.dll`.
4. **Р-Про CAD Core Manual RU (387 статей):** Полная русскоязычная документация платформы Р-Про v2.2.2 (кинематика, поведение, сигналы, моделирование процессов, свойства).
5. **CAD EN Manual (363 статьи):** Англоязычная эталонная документация Visual Components Core.
6. **Python API Reference (222 статьи):** Официальный справочник всех классов и методов vcScript, vcApplication, vcComponent, vcMatrix, vcMotion, vcSimObject.
7. **Нативные модули Р-Про (30 статей):** Декомпилированная документация встроенных заводских модулей «Эргономика», «Рабочие позы (WPP)» и «Захват движения (MoCap)».
8. **Work(s) Ergo Руководство пользователя (2 документа):** Интерактивный HTML-гайд v0.1 и официальный 50-страничный PDF-мануал v1.17 со всеми формулами DCR.
9. **28 Демо-Видео (@idkfa3):** Посекундный инженерный разбор 3D-манипулирования (Floating Hands, Snapping, Body Bracing, Straight Legs, LM-MMH, Office Ergonomics) с просмотром видео прямо в модальном окне портала.
10. **70+ Научных Первоисточников:** Полные формулы, биомеханические пределы и **26 загруженных PDF-книг и статей** (~180 МБ: Waters NIOSH 1994, NIOSH 1981, Dempster 1955, Grenier 1991, USAF 1964, Robinette CAESAR, de Leva 1996, Potvin 2021, Loper SMPL, Pavlakos SMPL-X, Aristidou FABRIK, Schulman TrajOpt, Delp OpenSim).
11. **Экспериментальные Датасеты:** 6 068 замеров ANSUR II (CSV), 22 907 кадров MoCap Axis Studio (BVH), многозвенные модели OpenSim Rajagopal (.osim).
12. **Бинарные Сборки & Контракты:** Сигнатуры и правила интеграции MEF (`UX.Shared.dll`, `Plugin.Ergonomics.dll`, `VisualComponents.Create3D.dll`).


---

## Лицензия

Copyright (c) 2026. All rights reserved. Разработано для экосистемы цифрового инжиниринга Р-Про.

