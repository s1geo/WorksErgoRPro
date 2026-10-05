# Multi-Body Spatial Dynamics and Dynamic Lifting Surge (Featherstone 2008, Kingma et al. 1996)
**Authors:** Roy Featherstone (2008), Idsart Kingma et al. (1996)  
**Scope:** Recursive Newton-Euler Algorithm (RNEA) and dynamic acceleration inertial surge

## 1. Dynamic Load Escalation in Lifting (Kingma 1996)
In dynamic lifting of objects from floor or low surfaces, acceleration at liftoff ($\ddot{z} = 1.5\dots 3.0\text{ m/s}^2$) and rotational acceleration of the torso ($\alpha = 2.0\dots 4.5\text{ rad/s}^2$) escalate L5/S1 spinal compression by **30% to 60%** above quasi-static estimates:
$$F_{\text{dynamic}} = m \cdot (g + \ddot{z}) + \frac{I_{\text{trunk}} \cdot \alpha}{h_m}$$

## 2. Spatial Vector Algebra (Featherstone 2008)
Spatial motion vectors $\hat{\mathbf{v}}$ and force vectors $\hat{\mathbf{f}}$:
$$\hat{\mathbf{v}} = \begin{bmatrix} \boldsymbol{\omega} \\ \mathbf{v} \end{bmatrix}, \quad \hat{\mathbf{f}} = \begin{bmatrix} \mathbf{n} \\ \mathbf{f} \end{bmatrix}$$
Spatial Inertia Matrix $\mathbf{I}$:
$$\mathbf{I} = \begin{bmatrix} \bar{\mathbf{I}} + m \mathbf{c}^\times \mathbf{c}^{\times T} & m \mathbf{c}^\times \\ m \mathbf{c}^{\times T} & m \mathbf{1}_{3\times 3} \end{bmatrix}$$

## 3. Recursive Newton-Euler Algorithm (RNEA)
1. **Forward Kinematic Pass (Base to Leaves):**
   $$\hat{\mathbf{v}}_i = \mathbf{X}_i \hat{\mathbf{v}}_{p(i)} + \hat{\mathbf{s}}_i \dot{q}_i$$
   $$\hat{\mathbf{a}}_i = \mathbf{X}_i \hat{\mathbf{a}}_{p(i)} + \hat{\mathbf{s}}_i \ddot{q}_i + \hat{\mathbf{v}}_i \times \hat{\mathbf{s}}_i \dot{q}_i$$
2. **Backward Dynamic Pass (Leaves to Base):**
   $$\hat{\mathbf{f}}_i = \mathbf{I}_i \hat{\mathbf{a}}_i + \hat{\mathbf{v}}_i \times^* \mathbf{I}_i \hat{\mathbf{v}}_i - \hat{\mathbf{f}}_i^{\text{ext}} + \sum_{j \in c(i)} \mathbf{X}_j^* \hat{\mathbf{f}}_j$$
   $$\tau_i = \hat{\mathbf{s}}_i^T \hat{\mathbf{f}}_i$$
