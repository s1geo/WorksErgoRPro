# Maximum Acceptable Effort for Repetitive Fatigue (Potvin, 2012)
**Author:** J. Richard Potvin  
**Journal:** Human Factors: The Journal of the Human Factors and Ergonomics Society, 54(2), 175-188  
**Scope:** Predicting Maximum Acceptable Effort (MAE) as a non-linear continuous function of Duty Cycle (DC)

## 1. Abstract & Theoretical Motivation
Sustained and repetitive isometric muscular contractions induce local muscle ischemia, anaerobic metabolite accumulation (lactate, $H^+$), and decline in force-generating capacity. Potvin synthesized fatigue datasets across upper extremity muscles into a single robust formulation parameterized solely by Duty Cycle ($DC$).

## 2. Mathematical Model
### 2.1. Duty Cycle Definition
$$DC = \frac{t_{\text{hold}}}{t_{\text{cycle}}} = \frac{t_{\text{hold}}}{t_{\text{hold}} + t_{\text{rest}}}, \quad DC \in [0, 1]$$

### 2.2. The Potvin MAE Equation
$$MAE = (1 - DC)^{0.4} \cdot \exp(-0.1 \cdot DC)$$
Alternative piecewise formulation:
$$MAE = \begin{cases}
1.00 & \text{if } DC \le 0.02 \\
1.0 - 1.155 \cdot DC^{0.35} & \text{if } 0.02 < DC \le 0.30 \\
0.45 \cdot \exp(-1.8 \cdot DC) & \text{if } DC > 0.30
\end{cases}$$

### 2.3. Muscle Endurance Limit (Rohmert Curve Comparison)
For static holding without rest ($DC = 1.0$), maximum endurance time $T_{\text{end}}$ follows Rohmert's law:
$$T_{\text{end}} = -1.5 + \frac{2.1}{f_{MVC}} - \frac{0.6}{f_{MVC}^2} + \frac{0.1}{f_{MVC}^3} \quad [\text{minutes}]$$

## 3. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Method: `CalculatePotvinMAE(double dutyCycle)`
