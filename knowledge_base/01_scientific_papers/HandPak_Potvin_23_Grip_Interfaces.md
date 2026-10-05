# HandPak: Hand & Finger Grip Interface Biomechanics (Potvin et al.)
**Scope:** 23 Hand/Finger interface force capacities, friction coefficients, and grip span optimizations

## 1. Classification of 23 Hand Interfaces
1. Power Grip (cylindrical full-hand wrap)
2. Chuck Pinch (3-jaw chuck: thumb, index, middle fingertips)
3. Lateral Pinch (key pinch: thumb pulp against radial side of index middle phalanx)
4. Pulp Pinch (2-finger precision tip pinch: thumb and index)
5. Finger Press (single distal finger press: index, middle, ring, little)
6. Palm Push (flat open palm)
... through all 23 distinct industrial interfaces.

## 2. Friction and Contact Multipliers
The effective holding capacity $F_{\text{hold}}$ depends on the coefficient of static friction $\mu$:
$$F_{\text{hold}} = \mu \cdot F_{\text{normal}}$$
- Clean, dry skin on steel: $\mu = 0.35$
- Clean skin on rubber: $\mu = 0.65$
- Oily / wet skin on metal: $\mu = 0.15$
- Nitrile / rubberized gloves: $\mu = 0.75$

Span optimization follows a parabolic curve peaking at $38\text{ mm}$ for cylinder diameter.

## 3. Implementation Mapping in WorksErgo
- Class: `WorksErgo.Biomechanics.ErgonomicMathEngine`
- Method: `CalculateHandGripCapacity()`
