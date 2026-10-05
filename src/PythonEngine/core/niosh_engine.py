# -*- coding: utf-8 -*-
"""
Revised NIOSH Lifting Equation (RNLE)
Reference: Waters, Putz-Anderson, Garg, Fine (1993) Ergonomics, 36(7), 749-776.
"""

LOAD_CONSTANT_KG = 23.0 # 51 lbs

def calculate_niosh_rwl(task):
    """
    Calculates Recommended Weight Limit (RWL) and Lifting Index (LI).
    RWL = LC * HM * VM * DM * AM * FM * CM
    """
    # 1. Horizontal Multiplier (H in cm: 25 <= H <= 63)
    h_cm = max(min(task.reach_h_m * 100.0, 63.0), 25.0)
    hm = 25.0 / h_cm
    
    # 2. Vertical Multiplier (V in cm: 0 <= V <= 175)
    v_cm = max(min(task.start_z_m * 100.0, 175.0), 0.0)
    vm = max(1.0 - (0.003 * abs(v_cm - 75.0)), 0.0)
    
    # 3. Distance Multiplier (D in cm: 25 <= D <= 175)
    d_cm = max(min(abs(task.end_z_m - task.start_z_m) * 100.0, 175.0), 25.0)
    dm = 0.82 + (4.5 / d_cm)
    dm = min(dm, 1.0)
    
    # 4. Asymmetric Multiplier (A in degrees: 0 <= A <= 135)
    a_deg = max(min(abs(task.asymmetry_deg), 135.0), 0.0)
    am = max(1.0 - (0.0032 * a_deg), 0.0)
    
    # 5. Frequency Multiplier (FM) - simplified table approximation
    f_min = task.freq_per_min
    if f_min <= 0.2:
        fm = 1.00
    elif f_min <= 1.0:
        fm = 0.94
    elif f_min <= 2.0:
        fm = 0.91
    elif f_min <= 4.0:
        fm = 0.84
    elif f_min <= 6.0:
        fm = 0.75
    elif f_min <= 8.0:
        fm = 0.60
    elif f_min <= 10.0:
        fm = 0.45
    else:
        fm = 0.30
        
    # 6. Coupling Multiplier (CM: Good=1.0, Fair=0.95, Poor=0.90)
    cm = 0.95
    
    # Recommended Weight Limit
    rwl = LOAD_CONSTANT_KG * hm * vm * dm * am * fm * cm
    rwl = max(rwl, 1.0)
    
    # Lifting Index (LI)
    li = task.load_kg / rwl
    
    if li <= 1.0:
        status = "GREEN (Nominal: Low risk for most healthy workers)"
    elif li <= 2.0:
        status = "YELLOW (Increased risk: Ergonomic redesign suggested)"
    else:
        status = "RED (High risk: Substantial proportion of workers at risk)"
        
    return {
        "rwl_kg": rwl,
        "lifting_index": li,
        "hm": hm,
        "vm": vm,
        "dm": dm,
        "am": am,
        "fm": fm,
        "cm": cm,
        "status": status
    }
