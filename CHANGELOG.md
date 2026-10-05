# Changelog

All notable changes to the **WorksErgo R-Pro Edition** project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planned
- Assembly of parametric `DHM_Worker.rpro` eCat component with 3D in-scene handle cylinders (Yellow, Blue, Pink).
- Implementation of Floating Hands auto-snapping to workpiece geometry along the $-X$ normal.
- Assembly of `Barrier.rpro` scalable bounding-box collision component.
- System Ribbon integration via `SiteSetupWorksErgo : IPlugin` and `WorksErgoPaneViewModel : DockableScreen`.

---

## [0.1.0] - 2026-10-05

### Added
- **Core Biomechanics Engine (`ErgonomicMathEngine.cs`):**
  - Lumbar spine L5/S1 compression solver based on Dortmund Lumbar Load Atlas (Jäger 2023) with 4th-order polynomial for erector spinae lever arm $h_m(\theta)$ and intra-abdominal pressure ($IAP$) relief.
  - 75th percentile population protection thresholds ($US_{75\%} = 4\,360\text{ N}$ female, $5\,210\text{ N}$ male).
  - Cumulative spinal fatigue failure modeling using 2-parameter Weibull CDF (Brinckmann 1988, Potvin & Agnew 2026: $\beta = 1.34$, $\log_{10}\alpha = -3.874 \cdot LCF + 5.219$) and Palmgren-Miner linear damage summation.
  - 14 continuous parametric Liberty Mutual MMH equations (Potvin et al. 2021) for Lift, Lower, Push ($F_{\text{init}}/F_{\text{sust}}$), Pull, and Carry.
  - Maximum Acceptable Effort (Potvin 2012 MAE) continuous duty cycle function for localized muscle fatigue.
  - Arm Force Field (AFF) 3D isometric reach capacity surface (LaDelfa & Potvin 2017).
  - 23 HandPak hand/finger grip interfaces with friction coefficient scaling ($\mu$).
  - Whole-body Center of Mass ($X_{\text{CoM}}$) closed-loop balance calculation using Dempster (1955) and Winter (2009) segmental masses.
  - Verification against Base of Support (BoS: $[-70\text{ mm}, +180\text{ mm}]$ from ankle).
  - Support for 3 distinct lifting mechanics: Stoop (hip-dominant), Semi-Squat (balanced), and Deep Squat (knee-dominant).
  - European Assessment Worksheet (EAWS v1.3.4) industrial scoring matrix.
- **Automated Verification Test Suite (`tests/`):**
  - Unified colorized test runner `tests/RunAllTests.ps1`.
  - Math unit tests verifying L5/S1, Weibull, Snook, and CoM boundary conditions.
  - MEF reflection and assembly integrity tests.
  - UI ViewModel instantiation tests.
  - 60 FPS latency benchmark.
- **Knowledge Base & Academic Repository (`knowledge_base/`):**
  - Downloaded open-access textbooks: Gordon et al. 2012 (ANSUR II book, 7.3 MB), Hotzman et al. 2011 (5.2 MB), Work(s) User Manual v1.17 (10.8 MB).
  - Downloaded datasets: ANSUR II Male CSV (2.0 MB), ANSUR II Female CSV (1.0 MB), OpenSim Rajagopal 2016/2023 `.osim` models.
  - Downloaded academic papers: FABRIK IK (Aristidou 2011), TrajOpt (Schulman 2014), Buss IK DLS (2004), SMPL-X (Pavlakos 2019), Optimal Feedback Control (Todorov 2002).
  - Indexed local user MoCap dataset: 12 BVH files (22,907 frames @ 96 Hz) in `axis_studio_bvh_manifest.json`.
  - Created Windows directory junction link from `knowledge_base` into user Documents.
- **Documentation Compendium (`docs/`):**
  - 11 formal technical documents covering biomechanics, MEF architecture, MoCap calibration, and industry comparisons.
  - Master Engineering Report (`docs/11_COMPREHENSIVE_MASTER_ENGINEERING_REPORT_WORKSERGO_RPRO.md`, 6,943 words / 35 pages).
  - Full API dumps of decompiled R-Pro plugins: `Plugin.Ergonomics.dll` and `Plugin.ErgonomicsWPP.dll`.
- **Project Infrastructure:**
  - Modern Visual Studio solution `WorksErgoRPro.sln`.
  - SDK-style project file `src/Plugin.WorksErgo/Plugin.WorksErgo.csproj`.
  - Automated build script `build.ps1` and deployment script `deploy.ps1`.
  - Enterprise-grade `.gitignore` and `enterprise-git-and-docs` skill.

### Changed
- Decoupled all domain biomechanics equations from UI/WPF and CAD APIs into pure C# static and instance methods.
- Standardized all CAD components to native `.rpro` packages (replacing Visual Components `.vcmx`).

### Fixed
- Resolved CLR `0x80131044` assembly load crash by targeting unsigned `UX.Shared.dll` (v4.5.0.0) from R-Pro v2.2.2.
- Prevented WPF reflection crashes by enforcing 7-bit ASCII identifiers on all commands and properties.
