# -*- coding: utf-8 -*-
"""
Psychophysical Engine (Snook & Ciriello / Liberty Mutual LM-MMH 2021)
References:
- Potvin, Ciriello, Snook, Maynard, Brogmus (2021)
- Snook & Ciriello (1991) Ergonomics, 34(9), 1197-1213
"""
import math

def calculate_snook_mawl(profile, task):
    """
    Calculates Maximum Acceptable Weight of Lift (MAWL) for 75% population capable.
    """
    sex = profile.sex
    # Base capacity at knuckle height (kg)
    base_mawl = 17.5 if sex == "F" else 29.0
    
    # 1. Horizontal Multiplier (0.25m <= H <= 0.60m)
    h_m = max(min(task.reach_h_m, 0.60), 0.25)
    f_h = 0.25 / h_m
    
    # 2. Vertical Start Multiplier (V = start_z_m)
    v_m = max(min(task.start_z_m, 1.75), 0.0)
    f_v = max(1.0 - 0.0028 * abs(v_m - 0.75), 0.5)
    
    # 3. Distance Multiplier (D = vertical displacement)
    d_m = max(abs(task.end_z_m - task.start_z_m), 0.25)
    f_d = min(0.85 + (0.045 / d_m), 1.0)
    
    # 4. Frequency Multiplier
    f_min = max(task.freq_per_min, 0.1)
    f_f = max(1.0 - 0.04 * math.log(1.0 + f_min), 0.4)
    
    # Computed MAWL (kg)
    mawl = base_mawl * f_h * f_v * f_d * f_f
    mawl = max(mawl, 2.0)
    
    dcr_snook = task.load_kg / mawl
    
    return mawl, dcr_snook

def calculate_snook_push_pull(profile, task):
    """
    Evaluates push/pull force limits.
    """
    sex = profile.sex
    # Initial / Sustained limits (N)
    limit_init = 200.0 if sex == "F" else 300.0
    limit_sust = 100.0 if sex == "F" else 160.0
    
    force_n = task.push_pull_force_n
    if force_n <= 0.0:
        # Default estimation based on load weight (rolling friction coeff ~ 0.05)
        force_n = task.load_kg * 9.81 * 0.05
        
    dcr_push = force_n / limit_sust
    return limit_sust, dcr_push

def evaluate_psychophysical_risk(profile, task):
    """
    Evaluates psychophysical capacity (MAWL or Push/Pull).
    """
    if "push" in task.task_type.lower() or "pull" in task.task_type.lower():
        limit, dcr = calculate_snook_push_pull(profile, task)
        capacity_str = "%.1f N" % limit
        demand_str = "%.1f N" % task.push_pull_force_n
    else:
        mawl, dcr = calculate_snook_mawl(profile, task)
        capacity_str = "%.1f kg" % mawl
        demand_str = "%.1f kg" % task.load_kg
        
    if dcr <= 0.85:
        status = "GREEN (Within 75% acceptable psychophysical capacity)"
    elif dcr <= 1.0:
        status = "YELLOW (Near maximum acceptable threshold)"
    else:
        status = "RED (Exceeds acceptable manual handling limits)"
        
    return {
        "capacity_str": capacity_str,
        "demand_str": demand_str,
        "dcr": dcr,
        "status": status
    }
