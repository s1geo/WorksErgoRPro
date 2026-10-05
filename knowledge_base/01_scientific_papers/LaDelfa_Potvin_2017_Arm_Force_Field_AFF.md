# The Arm Force Field (AFF) 3D Isometric Strength Model (LaDelfa & Potvin, 2017)
**Authors:** Nicholas J. LaDelfa, J. Richard Potvin  
**Journal:** Ergonomics, 60(9), 1246-1259  
**Scope:** 3D continuous vector field of hand strength capacity relative to shoulder origin

## 1. Experimental Dataset
The Arm Force Field is derived from 13,460 laboratory trials measuring 3D isometric hand force in female subjects across:
- 36 spatial hand locations spanning the reach envelope (normalized to arm length $L_{\text{arm}}$)
- Up to 26 spatial force directions (push, pull, up, down, medial, lateral, and diagonals).

## 2. Mathematical Formulation
### 2.1. Coordinate System Normalization
The hand position $\mathbf{P}_{\text{hand}} = [x, y, z]^T$ is expressed relative to the glenohumeral joint center $\mathbf{P}_{\text{shoulder}}$:
$$r = \frac{\Vert \mathbf{P}_{\text{hand}} - \mathbf{P}_{\text{shoulder}} \Vert}{L_{\text{arm}}}, \quad r \in [0.2, 1.0]$$
$$\phi = \operatorname{atan2}(y, x), \quad \theta = \operatorname{asin}(z / \Vert \mathbf{P}_{\text{hand}} - \mathbf{P}_{\text{shoulder}} \Vert)$$

### 2.2. Regression Surface for Isometric Capacity
$$\text{Capacity}_{3D}(\mathbf{P}, \mathbf{u}_F) = F_{\text{base}}(\mathbf{u}_F) \cdot f_{\text{radial}}(r) \cdot f_{\text{elevation}}(\theta) \cdot f_{\text{azimuth}}(\phi)$$
where $\mathbf{u}_F$ is the unit vector of applied hand force.

Arm reach degradation factor:
$$f_{\text{radial}}(r) = 1.0 - 0.42 \cdot (r - 0.5)^2 - 0.35 \cdot r^3$$

## 3. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Method: `CalculateArmForceFieldCapacity()`
