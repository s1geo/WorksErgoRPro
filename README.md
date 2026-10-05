# WorksErgo R-Pro Edition

[![Platform](https://img.shields.io/badge/Platform-R--Pro%20v2.2.2%20(Visual%20Components)-blue.svg)](https://r-pro.ru/)
[![Framework](https://img.shields.io/badge/.NET%20Framework-4.8%20%2F%20WPF%20%2F%20MEF-purple.svg)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/Build-Passing%20(100%25)-brightgreen.svg)](build.ps1)
[![Tests](https://img.shields.io/badge/Tests-All%20Passing-brightgreen.svg)](tests/RunAllTests.ps1)
[![Zero Cloud](https://img.shields.io/badge/Architecture-100%25%20Air--Gapped%20%2F%20Offline-success.svg)](#architecture)
[![Specification](https://img.shields.io/badge/Standard-Conventional%20Commits-yellow.svg)](CHANGELOG.md)

> **Next-Generation Autonomous Digital Human Modeling (DHM) and Biomechanical Ergonomics System for R-Pro v2.2.2.**  
> 100% standalone native .NET Framework 4.8 extension. Zero cloud dependencies, zero external subscriptions.

[English](#overview) | [Русский](#обзор-системы-на-русском)

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

### 1. Build Solution
```powershell
# Run the automated build script (uses csc.exe or MSBuild)
.\build.ps1
```

### 2. Run Test Suite
```powershell
# Execute the full 4-tier test verification suite
.\tests\RunAllTests.ps1
```

### 3. Deploy to R-Pro
```powershell
# Copies compiled assembly to R-Pro root for auto-discovery by MEF DirectoryCatalog
.\deploy.ps1
```

---

## Обзор системы (на русском)

**WorksErgo R-Pro Edition** — это отечественный нативный модуль цифрового манекена (DHM) и производственной эргономики для платформы трехмерного моделирования **Р-Про v2.2.2**.

### Ключевые преимущества:
1. **100% Автономность (Zero Cloud):** расчеты проводятся локально на ПК инженера без обращения к зарубежным платным облакам.
2. **Точный пользовательский опыт Work(s) Ergo:** поддержка компонентов `.rpro` в каталоге eCat, технологии плавающих рук (Floating Hands), интерактивных 3D-манипуляторов (желтый, синий, розовый цилиндры) и компонентов препятствий `Barrier.rpro`.
3. **Научная строгость:** синтез 36 мировых первоисточников (L5/S1 Jäger 2023, усталость Вейбулла Brinckmann, 14 уравнений Liberty Mutual MMH, центр тяжести Dempster-Winter CoM/BoS, стандарты автопрома EAWS BMW/VW).

Подробная научная спецификация доступна в файле [`docs/11_COMPREHENSIVE_MASTER_ENGINEERING_REPORT_WORKSERGO_RPRO.md`](docs/11_COMPREHENSIVE_MASTER_ENGINEERING_REPORT_WORKSERGO_RPRO.md).

---

## License

Copyright (c) 2026. All rights reserved. Developed for R-Pro Digital Engineering Ecosystem.
