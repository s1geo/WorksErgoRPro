# Cumulative Spinal Fatigue Failure & Weibull Distribution (Brinckmann 1988, Potvin & Agnew 2026)
**Authors:** P. Brinckmann, M. Biggemann, D. Hilweg (1988); J. R. Potvin, M. J. Agnew (2026)  
**Scope:** In vitro cyclic fatigue testing of lumbar functional units and 2-parameter Weibull CDF modeling

## 1. Experimental Methodology
Brinckmann et al. subjected 70 human lumbar motion segments (35 anatomical donors) to cyclic compressive fatigue loading up to 5,000 cycles at varying load-to-capacity fractions ($LCF = F_{\text{cyclic}} / US$). A refined subset of 47 motion segments with exact specimen-matched ultimate strength data was utilized to formulate the cumulative damage CDF.

## 2. Mathematical Formulation
### 2.1. 2-Parameter Weibull Cumulative Failure Probability
$$P_{\text{fail}}(N, LCF) = 1 - \exp\left( - \left( \frac{N}{\alpha(LCF)} \right)^\beta \right)$$
where:
- $N$: cumulative completed lift cycles across the work shift
- $\beta$: Weibull shape parameter (slope of fatigue failure dispersion)
  $$\beta = 1.34$$
- $\alpha(LCF)$: Weibull scale parameter (characteristic fatigue life to 63.2% failure)
  $$\log_{10}(\alpha) = a \cdot LCF + b$$
  with empirical regression coefficients:
  $$a = -3.874, \quad b = 5.219$$

### 2.2. Cumulative Damage Ratio (Miner's Rule)
For multi-task shift operations with varying loads $i$:
$$D_{\text{cum}} = \sum_{i=1}^{k} \frac{n_i}{N_i}$$
where failure occurs when $D_{\text{cum}} \ge 1.0$.

## 3. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Method: `CalculateCumulativeSpinalFatigue(double peakCompressionN, double ultimateStrengthN, int cycleCount)`
