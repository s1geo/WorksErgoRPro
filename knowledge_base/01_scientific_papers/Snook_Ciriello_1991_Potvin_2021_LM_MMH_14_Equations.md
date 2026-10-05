# Liberty Mutual MMH Equations (Snook & Ciriello 1991, Potvin et al. 2021)
**Authors:** Stover H. Snook, Vincent M. Ciriello (1991); J. Richard Potvin et al. (2021)  
**Scope:** Psychophysical Maximum Acceptable Weight of Lift (MAWL) and 14 continuous parametric equations

## 1. Background
The classic Snook & Ciriello tables were published as discrete matrices across fixed height tiers (Floor to Knuckle, Knuckle to Shoulder, Shoulder to Reach), fixed frequencies (1 lift/8h to 12 lifts/min), and percentiles (10%, 25%, 50%, 75%, 90%). In 2021, Potvin et al. synthesized 19 Liberty Mutual studies into 14 continuous parametric equations, eliminating discrete interpolation errors.

## 2. Mathematical Formulation
### 2.1. Generalized MMH Capacity Form
$$MAWL = \text{Base} \cdot MF_V \cdot MF_D \cdot MF_F \cdot MF_W \cdot MF_P$$
where:
- $\text{Base}$: baseline capacity at standard reference condition (kg or N)
- $MF_V$: vertical origin height multiplier
- $MF_D$: vertical distance travel multiplier
- $MF_F$: task frequency multiplier ($	ext{lifts/minute}$)
- $MF_W$: object box width multiplier
- $MF_P$: population percentile multiplier ($P_{75\%}$ vs $P_{90\%}$)

### 2.2. Demand-Capacity Ratio (DCR)
$$DCR_{\text{MMH}} = \frac{\text{Actual Load (kg or N)}}{MAWL}$$
- $DCR \le 0.85$: Low risk (Green)
- $0.85 < DCR \le 1.0$: Moderate risk (Yellow)
- $DCR > 1.0$: High risk (Red - action required)

## 3. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Methods: `CalculateSnookCirielloMAWL()`, `CalculateMMHDCR()`
