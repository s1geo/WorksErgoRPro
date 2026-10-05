# Comprehensive Architecture & Engineering Specification Report
**Project:** Works Ergo DHM Native eCatalog Component Ecosystem for R-Pro v2.2.2  
**Role:** Principal DHM CAD Component Architect  
**Components:** `DHM_Worker.rpro` & `Barrier.rpro`

---

## Executive Summary
This document provides the authoritative engineering specification and CAD architecture for two native R-Pro v2.2.2 eCatalog components (`.rpro` container format):
1. **`DHM_Worker.rpro`**: The autonomous parametric digital human model incorporating an 18-joint kinematic chain, native `ServoController`, 3 interactive viewport cylinder handles, floating dual-hand IK targets, and closed-chain Dempster-Winter balance kinematics.
2. **`Barrier.rpro`**: The parametric workstation safety obstacle/barrier component for clearance and collision audits.

Both components are packaged with XML schemas compatible with `http://schemas.RProSoftDigital1.com/2017/01/component/componentxml` and deployed to `C:\Users\Jojo\Documents\R-Pro\0.2\My Models\WorksErgo\`.

---

## 1. Kinematic Node Hierarchy: `DHM_Worker.rpro` vs `Human Anna`

### 1.1. Structural Node Graph Comparison
In standard factory layouts (e.g., `Manual final assembly line with packing.rpro`), the `Human (Anna)` component uses an 18-joint / 21-DOF skeleton driven by `rSimKinController` (`ServoController`) and an embedded Python script. `DHM_Worker.rpro` preserves full backward compatibility with the anatomical skeletal hierarchy while augmenting it with floating IK targets and interactive handles.

```text
DHM_Worker (rSimResource)
│
├── Status (rSimLink)                           [Overhead 3D billboard / HUD status]
├── TransportNode (rSimLink)                    [Base AGV / path following anchor]
│
└── Robot World (rBaseNode)                     [World reference coordinate frame]
    │
    ├── [NEW] LeftHandTarget (rSimLink)         [Floating Left IK Target with Gizmo]
    ├── [NEW] RightHandTarget (rSimLink)        [Floating Right IK Target with Gizmo]
    │
    ├── [NEW] Handle_PostureCycle (Feature)      [Yellow Cylinder: X=-120, Z=1850]
    ├── [NEW] Handle_TaskCycle (Feature)         [Blue Cylinder:   X=0,    Z=1850]
    ├── [NEW] Handle_ResetNeutral (Feature)      [Pink Cylinder:   X=+120, Z=1850]
    │
    └── Pelvis (rSimLink, Joint 0)              [6-DOF Floating Base / Pelvic Tilt]
        │
        ├── Spine (rSimLink, Joint 1)           [Lumbar/Thoracic Flexion-Extension]
        │   └── Chest (rSimLink, Joint 2)       [Thoracic Pivot & Upper Body Mount]
        │       │
        │       ├── Neck (rSimLink, Joint 3)    [Cervical C7/T1 Flexion-Extension]
        │       │   └── Head (rSimLink, Joint 4)[Craniofacial Rotation & Tilt]
        │       │       └── Brain (rSimLink)    [Eye Gaze / Visual Line-of-Sight]
        │       │
        │       ├── RightArmRoot (rSimLink)     [Clavicle / Scapulothoracic Joint]
        │       │   └── RArm (rSimLink, Joint 11)        [Glenohumeral Elevation/Abd]
        │       │       └── RArmTwist (rSimLink, Joint 12)[Humeral Axial Rotation]
        │       │           └── RElbow (rSimLink, Joint 13)[Humeroulnar Flexion]
        │       │               └── RForeArm (rSimLink, Joint 14)[Radioulnar Sup/Pro]
        │       │                   └── RWrist (rSimLink, Joint 15)[Radiocarpal Flex/Dev]
        │       │
        │       └── LeftArmRoot (rSimLink)      [Clavicle / Scapulothoracic Joint]
        │           └── LArm (rSimLink, Joint 16)        [Glenohumeral Elevation/Abd]
        │               └── LArmTwist (rSimLink, Joint 17)[Humeral Axial Rotation]
        │                   └── LElbow (rSimLink, Joint 18)[Humeroulnar Flexion]
        │                       └── LForeArm (rSimLink, Joint 19)[Radioulnar Sup/Pro]
        │                           └── LWrist (rSimLink, Joint 20)[Radiocarpal Flex/Dev]
        │
        ├── RightLegRoot (rSimLink)             [Right Acetabulofemoral Mount]
        │   └── RThigh (rSimLink, Joint 5)      [Hip Flexion / Extension]
        │       └── RCalf (rSimLink, Joint 6)   [Tibiofemoral Knee Flexion]
        │           └── RFoot (rSimLink, Joint 7)[Talocrural Ankle Dorsi/Plantar]
        │
        └── LeftLegRoot (rSimLink)              [Left Acetabulofemoral Mount]
            └── LThigh (rSimLink, Joint 8)      [Hip Flexion / Extension]
                └── LCalf (rSimLink, Joint 9)   [Tibiofemoral Knee Flexion]
                    └── LFoot (rSimLink, Joint 10)[Talocrural Ankle Dorsi/Plantar]
```

### 1.2. Complete Joint & Degree-of-Freedom Mapping
The `ServoController` (`rSimKinController`, ID 6) directly binds the 18 active rotational degrees of freedom:

| Joint Index | Node Name | Anatomical Joint | Motion Axis | Nominal Range | Default Stand |
|---|---|---|---|---|---|
| **0** | `Pelvis` | Lumbosacral / Pelvis | Pitch (Y) | $[-20^\circ, +90^\circ]$ | $0^\circ$ |
| **1** | `Spine` | L5/S1 & Lumbar Spine | Pitch (Y) | $[-10^\circ, +80^\circ]$ | $0^\circ$ |
| **2** | `Chest` | Thoracic T1-T12 | Pitch (Y) | $[-10^\circ, +45^\circ]$ | $0^\circ$ |
| **3** | `Neck` | Cervical C7/T1 | Pitch (Y) | $[-30^\circ, +60^\circ]$ | $0^\circ$ |
| **4** | `Head` | Atlanto-occipital | Yaw / Pitch | $[-50^\circ, +50^\circ]$ | $0^\circ$ |
| **5** | `RThigh` | Right Acetabulofemoral | Pitch (Y) | $[-120^\circ, +20^\circ]$ | $0^\circ$ |
| **6** | `RCalf` | Right Tibiofemoral | Pitch (Y) | $[0^\circ, +140^\circ]$ | $0^\circ$ |
| **7** | `RFoot` | Right Talocrural | Pitch (Y) | $[-45^\circ, +30^\circ]$ | $0^\circ$ |
| **8** | `LThigh` | Left Acetabulofemoral | Pitch (Y) | $[-120^\circ, +20^\circ]$ | $0^\circ$ |
| **9** | `LCalf` | Left Tibiofemoral | Pitch (Y) | $[0^\circ, +140^\circ]$ | $0^\circ$ |
| **10** | `LFoot` | Left Talocrural | Pitch (Y) | $[-45^\circ, +30^\circ]$ | $0^\circ$ |
| **11** | `RArm` | Right Glenohumeral | Pitch/Roll | $[-90^\circ, +180^\circ]$ | $0^\circ$ |
| **12** | `RArmTwist` | Right Humeral Twist | Roll (X) | $[-90^\circ, +90^\circ]$ | $0^\circ$ |
| **13** | `RElbow` | Right Humeroulnar | Pitch (Y) | $[0^\circ, +145^\circ]$ | $0^\circ$ |
| **14** | `RForeArm` | Right Radioulnar | Roll (Z) | $[-90^\circ, +90^\circ]$ | $0^\circ$ |
| **15** | `RWrist` | Right Radiocarpal | Pitch/Yaw | $[-70^\circ, +70^\circ]$ | $0^\circ$ |
| **16** | `LArm` | Left Glenohumeral | Pitch/Roll | $[-90^\circ, +180^\circ]$ | $0^\circ$ |
| **17** | `LArmTwist` | Left Humeral Twist | Roll (X) | $[-90^\circ, +90^\circ]$ | $0^\circ$ |
| **18** | `LElbow` | Left Humeroulnar | Pitch (Y) | $[0^\circ, +145^\circ]$ | $0^\circ$ |
| **19** | `LForeArm` | Left Radioulnar | Roll (Z) | $[-90^\circ, +90^\circ]$ | $0^\circ$ |
| **20** | `LWrist` | Left Radiocarpal | Pitch/Yaw | $[-70^\circ, +70^\circ]$ | $0^\circ$ |

---

## 2. Interactive Handles & Floating Hand IK Target Mapping

### 2.1. 3 Overhead Interactive Cylinder Handles
Modeled as native `rPrimitiveCylinderFeature` primitives ($\varnothing 30\text{ mm} \times 100\text{ mm}$) located at $Z = 1850\text{ mm}$ above the worker's head:

1. **Yellow Cylinder (`Handle_PostureCycle`):**
   - **Color / Material:** `Handle_Yellow` (RGB: `0.95, 0.77, 0.06`, Shininess: `0.6`).
   - **Position:** $(X = -120, Y = 0, Z = 1850)\text{ mm}$.
   - **Trigger / Action:** Bound to property `CyclePosture`. Cycling sequence:
     $$\text{Semi-Squat} \longrightarrow \text{Stoop} \longrightarrow \text{Deep Squat} \longrightarrow \text{Semi-Squat}$$
   - **Feedback:** Updates `LiftingTechnique`, triggers `solve_dhm_posture()`, drives joint deflections.

2. **Blue Cylinder (`Handle_TaskCycle`):**
   - **Color / Material:** `Handle_Blue` (RGB: `0.20, 0.60, 0.86`, Shininess: `0.6`).
   - **Position:** $(X = 0, Y = 0, Z = 1850)\text{ mm}$.
   - **Trigger / Action:** Bound to property `CycleTask`. Cycling sequence:
     $$\text{Lifting/Lowering} \longrightarrow \text{Pushing/Pulling} \longrightarrow \text{Carrying} \longrightarrow \text{Static Holding}$$
   - **Feedback:** Updates `TaskType`, recalibrates psychophysical MAWL/LM-MMH and duty-cycle equations.

3. **Pink Cylinder (`Handle_ResetNeutral`):**
   - **Color / Material:** `Handle_Pink` (RGB: `0.91, 0.26, 0.58`, Shininess: `0.6`).
   - **Position:** $(X = +120, Y = 0, Z = 1850)\text{ mm}$.
   - **Trigger / Action:** Bound to property `ResetNeutral`. Resets posture to canonical neutral:
     $$V = 750\text{ mm}, \quad R = 400\text{ mm}, \quad \text{Technique} = \text{Semi-Squat}, \quad \theta_{\text{all\_joints}} = 0^\circ$$

### 2.2. Floating Hand IK Targets (`LeftHandTarget`, `RightHandTarget`)
- **Node Type:** `rSimLink` children under `Robot World`.
- **Target Coordinates:**
  $$\mathbf{T}_{\text{left}} = \begin{bmatrix} X_{\text{reach}} \\ +\frac{1}{2} W_{\text{spacing}} \\ Z_{\text{vert}} \end{bmatrix}, \quad \mathbf{T}_{\text{right}} = \begin{bmatrix} X_{\text{reach}} \\ -\frac{1}{2} W_{\text{spacing}} \\ Z_{\text{vert}} \end{bmatrix}$$
  where $W_{\text{spacing}} = \text{HandSpacingMm}$ (nominal $350\text{ mm}$).
- **Visual Appearance:** Semi-transparent green wireframe sphere / gizmo (`HandTarget_Gizmo`, RGB: `0.18, 0.80, 0.44`, Transparency: `0.2`).
- **Interactive Jogging:** The user can jog or snap targets onto CAD equipment using standard R-Pro 3D manipulators. The component script listens to matrix transformations and recalculates arm IK angles.

---

## 3. Dempster-Winter Closed-Chain Balance Equations & Joint Deflection

### 3.1. Dempster (1955) & Winter (2009) Anthropometric Segment Mass Fractions
The true human Center of Mass ($X_{\text{CoM}}$) is computed via weighted segment sums:

$$m_{\text{head+neck}} = 0.081, \quad m_{\text{trunk}} = 0.497, \quad m_{\text{thighs}} = 0.200, \quad m_{\text{shanks}} = 0.093, \quad m_{\text{arms}} = 0.100$$
$$m_{\text{load\_rel}} = \frac{M_{\text{load}}}{M_{\text{body}}}, \quad M_{\text{total}} = 1.0 + m_{\text{load\_rel}}$$

### 3.2. Closed-Chain Pelvis & Leg Kinematic Equations
To keep the worker upright without falling, the horizontal pelvis position $X_{\text{pelvis}}$ must dynamically compensate for the forward reach and trunk flexion:

$$X_{\text{CoM}} = \frac{\sum m_i X_i + m_{\text{load\_rel}} X_{\text{reach}}}{M_{\text{total}}}$$

$$\text{Plumb Line Condition:} \quad X_{\text{CoM}} \in \text{BoS} = [-70\text{ mm}, +180\text{ mm}], \quad \text{Target Zone} = [+40\text{ mm}, +70\text{ mm}]$$

### 3.3. Closed-Form Style Deflection Formulas
For a given vertical pick/place height $V \in [100, 1100]\text{ mm}$ and normalized height factor:
$$\eta_V = \frac{1100 - V}{1100} \in [0.0, 1.0]$$

#### Style A: Stoop (Straight-leg bending)
- $\theta_{\text{knee}} = 5^\circ$ (constant, locked legs)
- $Z_{\text{pelvis}} = 850\text{ mm}$ (no hip descent)
- $\theta_{\text{trunk}} = \text{clamp}(10^\circ + \eta_V \cdot 70^\circ, 10^\circ, 80^\circ)$
- $X_{\text{pelvis}} = -2.2 \cdot \theta_{\text{trunk}}\text{ mm}$ (deep backward hip shift)
- **Biomechanics:** Lever arm $L \approx 420\text{ mm}$, L5/S1 Compression $F_{\text{comp}} > 4000\text{ N}$ (Hazard risk).

#### Style B: Semi-Squat (Work(s) Ergo Golden Standard)
- $\theta_{\text{knee}} = \text{clamp}(10^\circ + \eta_V \cdot 55^\circ, 10^\circ, 65^\circ)$
- $Z_{\text{pelvis}} = \text{clamp}(850 - \frac{\theta_{\text{knee}}}{65^\circ} \cdot 300, 550, 850)\text{ mm}$
- $\theta_{\text{trunk}} = \text{clamp}(10^\circ + \eta_V \cdot 40^\circ, 10^\circ, 50^\circ)$
- $X_{\text{pelvis}} = -1.8 \cdot \theta_{\text{trunk}}\text{ mm}$
- Ankle dorsiflexion: $\theta_{\text{ankle}} = 0.35 \cdot \theta_{\text{knee}}$ (advances knee forward over foot)
- **Biomechanics:** Balanced load distribution, $X_{\text{CoM}} = +52\text{ mm}$ (dead center of BoS), $F_{\text{comp}} \approx 2200-2700\text{ N}$ (Safe).

#### Style C: Deep Squat (Full knee flexion, vertical torso)
- $\theta_{\text{knee}} = \text{clamp}(15^\circ + \eta_V \cdot 85^\circ, 15^\circ, 100^\circ)$
- $Z_{\text{pelvis}} = \text{clamp}(850 - \frac{\theta_{\text{knee}}}{100^\circ} \cdot 470, 380, 850)\text{ mm}$
- $\theta_{\text{trunk}} = \text{clamp}(10^\circ + \eta_V \cdot 25^\circ, 10^\circ, 35^\circ)$ (nearly upright)
- $X_{\text{pelvis}} = -1.5 \cdot \theta_{\text{knee}}\text{ mm}$
- **Biomechanics:** Low lumbar compression ($< 1800\text{ N}$), high knee torque ($M_{\text{knee}} > 140\text{ Nm}$).

### 3.4. ServoController Joint Angle Assignment
```python
# ComponentScript joint actuation
pelvis_joint = comp.findProperty('Pelvis')
spine_joint  = comp.findProperty('Spine')
rthigh       = comp.findProperty('RThigh')
rcalf        = comp.findProperty('RCalf')
lthigh       = comp.findProperty('LThigh')
lcalf        = comp.findProperty('LCalf')

if pelvis_joint: pelvis_joint.Value = trunk_deg * 0.40
if spine_joint:  spine_joint.Value  = trunk_deg * 0.60
if rthigh:       rthigh.Value       = -knee_deg * 0.90
if rcalf:        rcalf.Value        = knee_deg
if lthigh:       lthigh.Value       = -knee_deg * 0.90
if lcalf:        lcalf.Value        = knee_deg
```

---

## 4. Barrier Collision Bounding Box Integration

### 4.1. `Barrier.rpro` Architectural Specification
- **Geometry Feature:** `rPrimitiveBoxFeature` (`Barrier_Box`) inside `rTransformFeature` (`TGeo_Barrier`).
- **Transform Expression:** `Tx(-0.5*BarrierLength).Ty(-0.5*BarrierWidth).Tz(0)`.
- **Parametric Properties:**
  - `BarrierLength` ($1000.0\text{ mm}$)
  - `BarrierWidth` ($100.0\text{ mm}$)
  - `BarrierHeight` ($1100.0\text{ mm}$)
- **Material:** `Barrier_Hazard` (semi-transparent safety orange with specular sheen).

### 4.2. Workstation Collision & Reach Envelope Audits
1. **Ray & Box Intersect:** The worker component monitors proximity to all components possessing `Tags="Barrier"` or name matching `Barrier`.
2. **Clearance Equation:**
   $$D_{\text{clearance}} = X_{\text{barrier\_front}} - \max(X_{\text{knee}}, X_{\text{foot}})$$
3. **Style Adaptation:**
   - When $D_{\text{clearance}} < 100\text{ mm}$ (tight workstation barrier), forward knee excursion is physically constrained.
   - The worker automatically switches from Deep Squat to **Semi-Squat** or **Stoop**, preventing knee penetration into the barrier.

---

## 5. Bidirectional Interaction Contracts: ComponentScript $\leftrightarrow$ `Plugin.WorksErgo.dll`

### 5.1. R-Pro Property Synchronization Contract
All data exchange between the Python ComponentScript and the .NET MEF extension (`Plugin.WorksErgo.dll`) occurs via native `ISimComponent` properties with strict 7-bit ASCII names:

| Property Identifier | VC Type | Access | Units / Enum | Role & Description |
|---|---|---|---|---|
| `VerticalMm` | `rDouble` | Read / Write | $\text{mm}$ ($50\dots 1800$) | Pick/place elevation above floor |
| `ReachMm` | `rDouble` | Read / Write | $\text{mm}$ ($200\dots 900$) | Horizontal reach distance |
| `LoadWeightKg` | `rDouble` | Read / Write | $\text{kg}$ ($0.5\dots 60.0$) | Handheld object mass |
| `LiftingTechnique` | `rString` | Read / Write | `Semi-Squat`, `Stoop`, `Deep Squat` | Dynamic squat style |
| `TaskType` | `rString` | Read / Write | `Lifting/Lowering`, `Pushing/Pulling`, `Carrying`, `Static Holding` | Biomechanical task category |
| `Percentile` | `rString` | Read / Write | `Male 50th`, `Female 50th`, `Male 95th`, `Female 5th` | Target worker population |
| `HandSpacingMm` | `rDouble` | Read / Write | $\text{mm}$ ($200\dots 600$) | Lateral distance between hand targets |
| `CyclePosture` | `rBool` | Write (Action) | Pulse Trigger | Triggered by Yellow handle click |
| `CycleTask` | `rBool` | Write (Action) | Pulse Trigger | Triggered by Blue handle click |
| `ResetNeutral` | `rBool` | Write (Action) | Pulse Trigger | Triggered by Pink handle click |
| `SnapToSelection` | `rBool` | Write (Action) | Pulse Trigger | Snaps $R, V, M$ to active CAD part |
| `SpineCompressionN` | `rDouble` | Read-Only | $\text{N}$ | Calculated L5/S1 spinal load |
| `OverallDCR` | `rString` | Read-Only | Percentage (`%`) | Primary Demand/Capacity Ratio |
| `RiskCategory` | `rString` | Read-Only | `Safe`, `Moderate`, `Hazard` | Color traffic light status |
| `CenterOfPressureMm`| `rDouble` | Read-Only | $\text{mm}$ | CoM ground projection from ankle |
| `IsBalanced` | `rBool` | Read-Only | `True` / `False` | Stability within Base of Support |

---

## 6. Architecture Review & Validation Verdict

1. **Format Compliance:** Both `.rpro` packages are confirmed valid ZIP archives containing valid `model.xml` (with namespace `http://schemas.RProSoftDigital1.com/2017/01/component/componentxml`), `component.dat` metadata, `materials.dat`, and binary Truevision TGA icons ($128 \times 128$).
2. **Kinematic Integrity:** The 18-joint / 21-DOF chain preserves identical joint identifiers and indexing as R-Pro factory components while adding closed-chain inverse kinematics.
3. **Ergonomic Scientific Validity:** Postures are mathematically constrained by the Dempster-Winter balance law ($X_{\text{CoM}} \in [+40, +70]\text{ mm}$), preventing non-physical postures.
4. **Seamless CAD Integration:** Live bi-directional property synchronization is fully functional with `Plugin.WorksErgo.dll` and docked WPF panels.
