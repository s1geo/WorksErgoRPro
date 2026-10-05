# -*- coding: utf-8 -*-
"""
Spinal Biomechanics Engine (L5/S1 Bone-on-Bone Loading & Potvin Fatigue)
References:
- Jäger (2023) Dortmund Lumbar Load Atlas
- Chaffin et al. (2006) Occupational Biomechanics
- Potvin (2012) Maximum Acceptable Effort (MAE)
- Waters et al. (1991, 1993) NIOSH
"""
import math
try:
    from anthropometry import calculate_segment_parameters
except (ImportError, ValueError):
    try:
        from core.anthropometry import calculate_segment_parameters
    except (ImportError, ValueError):
        from .anthropometry import calculate_segment_parameters


GRAVITY = 9.81 # m/s^2

# Population Tolerance Limits (75% capable TLVs)
TLV_LCF_FEMALE = 3575.0 # Newtons (Jäger 2023)
TLV_LCF_MALE   = 4270.0 # Newtons (Jäger 2023)
NIOSH_AL_LCF   = 3400.0 # Action Limit

def estimate_trunk_flexion_angle(pick_z_m, reach_h_m, stature_m):
    """
    Estimates torso sagittal flexion angle (degrees) based on hand contact
    height and reach relative to ankle origin.
    """
    # Shoulder height is roughly 0.80 * Stature
    shoulder_z = 0.80 * stature_m
    dz = shoulder_z - pick_z_m
    
    if dz <= 0.0:
        # Reaching overhead
        return 0.0
    
    # Calculate geometric flexion angle needed to reach target
    # Lower pick height requires higher flexion
    arm_reach = 0.40 * stature_m # effective arm length
    
    if pick_z_m < 0.20:
        # Reaching near or at floor -> severe flexion + knee bend
        return 65.0
    elif pick_z_m < 0.60:
        # Knee to mid-thigh
        return 40.0
    elif pick_z_m <= 1.00:
        # Table / knuckle height
        return 15.0
    else:
        # Shoulder / eye level
        return 5.0

def calculate_potvin_mae(frequency_per_min, displacement_m, shift_hours=8.0):
    """
    Calculates Effective Duration (t_eff), Duty Cycle (DC), and MAE (Potvin 2012).
    Matches benchmark: at DC=0.0231 -> MAE = 0.596.
    """
    # Effective duration equation for lift/lower (Manual p. 50):
    # t_eff = 0.46 + 0.605 * D_v (seconds)
    t_eff = 0.46 + 0.605 * abs(displacement_m)
    
    # Total shift seconds
    shift_s = max(shift_hours * 3600.0, 3600.0)
    
    # Daily cycle count
    daily_cycles = frequency_per_min * 60.0 * shift_hours
    
    # Duty Cycle
    duty_cycle = (daily_cycles * t_eff) / shift_s
    duty_cycle = min(max(duty_cycle, 0.0001), 0.99)
    
    # Potvin MAE: MAE = 1.0 - DC^0.24
    mae = 1.0 - (duty_cycle ** 0.24)
    mae = max(mae, 0.05)
    
    return t_eff, duty_cycle, mae

def evaluate_lumbar_loading(profile, task):
    """
    Computes 3D moment, erector muscle force, and resultant L5/S1 compression and shear.
    Returns: dict with LCF, shear, spine_dcr, mae, duty_cycle, t_eff.
    """
    H = profile.stature_m
    M = profile.mass_kg
    sex = profile.sex
    
    # Estimate trunk flexion angle
    trunk_angle_deg = estimate_trunk_flexion_angle(task.start_z_m, task.reach_h_m, H)
    trunk_angle_rad = math.radians(trunk_angle_deg)
    
    # Segment masses
    segs = calculate_segment_parameters(profile)
    upper_body_mass = segs["head_neck"]["mass"] + segs["torso"]["mass"] + 2.0 * (segs["upper_arm"]["mass"] + segs["forearm"]["mass"] + segs["hand"]["mass"])
    
    # Moment arms from L5/S1
    # Torso COM is roughly 0.20 * H from L5/S1 along torso axis
    d_torso_com = (0.20 * H) * math.sin(trunk_angle_rad)
    # Load moment arm: horizontal reach from L5/S1
    d_load = max(task.reach_h_m, 0.25)
    
    # 3D Resultant Moment at L5/S1 (Nm)
    m_body = upper_body_mass * GRAVITY * d_torso_com
    m_load = task.load_kg * GRAVITY * d_load
    m_resultant = m_body + m_load
    
    # Muscle moment arm (erector spinae) with flexion adjustment:
    # d_arm = 0.050 + 0.00015 * theta (meters)
    d_muscle_arm = 0.050 + (0.00015 * trunk_angle_deg)
    
    # Erector spinae muscle force
    f_muscle = m_resultant / d_muscle_arm
    
    # Sacral decomposition (alpha = 34 deg sacral angle)
    sacral_angle_rad = math.radians(34.0 + trunk_angle_deg)
    f_reaction = (upper_body_mass + task.load_kg) * GRAVITY
    f_reaction_comp = f_reaction * math.cos(sacral_angle_rad)
    f_shear = f_reaction * math.sin(sacral_angle_rad)
    
    # Total Lumbar Compression Force (LCF)
    lcf = f_muscle + f_reaction_comp
    
    # Potvin MAE calculation
    displacement = abs(task.end_z_m - task.start_z_m)
    t_eff, dc, mae = calculate_potvin_mae(task.freq_per_min, displacement, task.duration_h)
    
    # Select TLV
    tlv = TLV_LCF_FEMALE if sex == "F" else TLV_LCF_MALE
    
    # Spine DCR
    spine_dcr = lcf / tlv
    
    # Status
    if spine_dcr <= 0.85:
        status = "GREEN (Safe: within 75% population capacity)"
    elif spine_dcr <= 1.0:
        status = "YELLOW (Moderate load: ergonomic monitoring recommended)"
    else:
        status = "RED (Hazardous: exceeds spine capacity limit)"
        
    return {
        "lcf": lcf,
        "shear": f_shear,
        "tlv": tlv,
        "spine_dcr": spine_dcr,
        "trunk_angle_deg": trunk_angle_deg,
        "t_eff": t_eff,
        "duty_cycle": dc,
        "mae": mae,
        "status": status
    }
