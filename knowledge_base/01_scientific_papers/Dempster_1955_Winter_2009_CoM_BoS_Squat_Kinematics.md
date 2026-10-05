# Whole-Body Center of Mass (CoM) and Base of Support (BoS) Equilibrium (Dempster 1955, Winter 2009)
**Authors:** Wilfred T. Dempster (1955), David A. Winter (2009)  
**Scope:** Exact multi-link segmental center of mass coordinates and closed-loop squat balance

## 1. Segmental Mass Fractions (Winter 2009 / Dempster 1955)
Total body mass $M_{\text{body}}$ is partitioned into canonical anatomical rigid segments:
- Head and Neck: $m_{\text{head}} = 0.081 \cdot M_{\text{body}}$ (CoM at 0.500 of segment length)
- Torso (Thorax + Abdomen + Pelvis): $m_{\text{trunk}} = 0.497 \cdot M_{\text{body}}$ (CoM at 0.500 of segment length)
- Thighs (bilateral): $m_{\text{thighs}} = 0.200 \cdot M_{\text{body}}$ (CoM at 0.433 from hip)
- Shanks (bilateral): $m_{\text{shanks}} = 0.093 \cdot M_{\text{body}}$ (CoM at 0.433 from knee)
- Feet (bilateral): $m_{\text{feet}} = 0.029 \cdot M_{\text{body}}$ (CoM at 0.500 from heel)
- Upper Arms + Forearms + Hands: $m_{\text{arms}} = 0.100 \cdot M_{\text{body}}$
- External Box Load: $m_{\text{load}}$ at hand grip center.

## 2. Exact CoM Projection Equation
$$X_{\text{CoM}} = \frac{\sum_{i} m_i \cdot X_i + m_{\text{load}} \cdot X_{\text{load}}}{\sum_i m_i + m_{\text{load}}}$$

## 3. Base of Support (BoS) Equilibrium Condition
For an operator standing on both feet with ankles aligned at $X = 0$:
$$\text{BoS} = [-70\text{ mm}, +180\text{ mm}]$$
- Heel boundary: $X_{\text{heel}} = -70\text{ mm}$
- Toe boundary: $X_{\text{toe}} = +180\text{ mm}$
- Optimal neutral mid-foot corridor: $X_{\text{mid}} \in [+40\text{ mm}, +70\text{ mm}]$

Equilibrium Criterion:
$$X_{\text{heel}} \le X_{\text{CoM}} \le X_{\text{toe}}$$

## 4. The 3 Squat Lifting Mechanics
1. **Hip-Dominant (Stoop / Romanian Lift):** Knees maintain near-zero flexion ($	heta_k \approx 5^\circ$). Ankle remains neutral ($0^\circ$). Pelvis retreats to $X = -220\text{ mm}$. Torso inclines up to $80^\circ$. High spinal compression ($> 4,200\text{ N}$), zero knee moment.
2. **Knee-Dominant (Deep Squat):** Dorsiflexion of ankle advances knees forward to $X_K = +160\text{ mm}$. Torso remains upright ($20^\circ\dots 30^\circ$). Pelvis drops vertically. High knee extensor moment, low spinal compression ($< 2,200\text{ N}$).
3. **Balanced (Semi-Squat / Work(s) Standard):** Symmetric moment arms $T \approx K$. Plumb line drops through $+50\text{ mm}$ mid-foot. Minimizes total joint strain.

## 5. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Methods: `CalculateCenterOfMass()`, `ValidateBaseOfSupport()`, `SolveEquilibriumSquat()`
