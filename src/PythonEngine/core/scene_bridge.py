# -*- coding: utf-8 -*-
"""
Scene Bridge: 1-Click 3D CAD Geometry & Context Extractor
Integrates CAD objects with Ergonomics Assessment Engines.
"""
try:
    from ergo_types import TaskParameters, ErgonomicsReport
    from anthropometry import get_profile
    from biomechanics_engine import evaluate_lumbar_loading
    from snook_ciriello_engine import evaluate_psychophysical_risk
    from niosh_engine import calculate_niosh_rwl
    from posture_scoring import evaluate_rula, evaluate_reba
except (ImportError, ValueError):
    try:
        from core.ergo_types import TaskParameters, ErgonomicsReport
        from core.anthropometry import get_profile
        from core.biomechanics_engine import evaluate_lumbar_loading
        from core.snook_ciriello_engine import evaluate_psychophysical_risk
        from core.niosh_engine import calculate_niosh_rwl
        from core.posture_scoring import evaluate_rula, evaluate_reba
    except (ImportError, ValueError):
        from .ergo_types import TaskParameters, ErgonomicsReport
        from .anthropometry import get_profile
        from .biomechanics_engine import evaluate_lumbar_loading
        from .snook_ciriello_engine import evaluate_psychophysical_risk
        from .niosh_engine import calculate_niosh_rwl
        from .posture_scoring import evaluate_rula, evaluate_reba


def extract_selected_context(app):
    """
    Extracts 3D coordinates, height, and reach from selected CAD component.
    """
    try:
        sel_mgr = getattr(app, "SelectionManager", None)
        if not sel_mgr:
            return None, "SelectionManager not found."
            
        # 1 = VC_SELECTION_COMPONENT
        selected = list(sel_mgr.getSelection(1))
        if not selected:
            return None, "Select a Box or Object in 3D."
            
        comp = selected[0]
        matrix = getattr(comp, "WorldPositionMatrix", None)
        if not matrix:
            matrix = getattr(comp, "PositionMatrix", None)
            
        p = matrix.P
        x_m = p.X / 1000.0
        y_m = p.Y / 1000.0
        z_m = p.Z / 1000.0
        
        # Calculate horizontal reach from operator origin (assumed (0,0) or operator position)
        reach_h_m = (x_m**2 + y_m**2)**0.5
        reach_h_m = max(min(reach_h_m, 0.70), 0.25)
        
        # Extract mass if property exists
        load_kg = 5.0
        for prop in comp.Properties:
            if "mass" in prop.Name.lower() or "weight" in prop.Name.lower():
                try:
                    load_kg = float(prop.Value)
                except Exception:
                    pass
                    
        return {
            "component_name": comp.Name,
            "start_z_m": z_m,
            "reach_h_m": reach_h_m,
            "load_kg": load_kg
        }, "Extracted from 3D: %s (Z=%.2f m, Reach=%.2f m, Load=%.1f kg)" % (comp.Name, z_m, reach_h_m, load_kg)
        
    except Exception as ex:
        return None, "Error extracting 3D context: " + str(ex)

def evaluate_full_ergonomics(profile_name, task):
    """
    Evaluates all 5 ergonomic engines and produces a unified ErgonomicsReport.
    """
    report = ErgonomicsReport()
    profile = get_profile(profile_name)
    
    # 1. Biomechanics (Spine L5/S1 & Potvin MAE)
    bio = evaluate_lumbar_loading(profile, task)
    report.lcf_newtons = bio["lcf"]
    report.shear_newtons = bio["shear"]
    report.spine_dcr = bio["spine_dcr"]
    report.spine_status = bio["status"]
    report.effective_duration_s = bio["t_eff"]
    report.duty_cycle = bio["duty_cycle"]
    report.mae = bio["mae"]
    trunk_angle = bio["trunk_angle_deg"]
    
    # 2. Psychophysical (Snook & Ciriello)
    snook = evaluate_psychophysical_risk(profile, task)
    report.snook_dcr = snook["dcr"]
    report.snook_status = snook["status"]
    report.mawl_kg = float(snook["capacity_str"].split()[0]) if "kg" in snook["capacity_str"] else 0.0
    
    # 3. NIOSH RNLE
    niosh = calculate_niosh_rwl(task)
    report.rwl_kg = niosh["rwl_kg"]
    report.lifting_index = niosh["lifting_index"]
    report.niosh_status = niosh["status"]
    
    # 4. Posture Scoring (RULA & REBA)
    # Estimate upper arm and forearm angles based on reach
    upper_arm_deg = min(max(int(task.reach_h_m * 100.0), 15), 85)
    forearm_deg = 80
    rula = evaluate_rula(trunk_angle, upper_arm_deg, forearm_deg, task.load_kg)
    report.rula_score = rula["grand_score"]
    report.rula_dcr = rula["dcr"]
    
    reba = evaluate_reba(trunk_angle, upper_arm_deg, forearm_deg, task.load_kg)
    report.reba_score = reba["grand_score"]
    report.reba_dcr = reba["dcr"]
    
    # 5. Unified Overall DCR
    # Overall DCR = max(spine_dcr, snook_dcr, lifting_index, rula_dcr, reba_dcr)
    report.overall_dcr = max([
        report.spine_dcr,
        report.snook_dcr,
        report.lifting_index,
        report.rula_dcr,
        report.reba_dcr
    ])
    
    # Traffic Light Classification
    if report.overall_dcr <= 0.85:
        report.risk_level = "GREEN"
        report.status_text = "[GREEN] SAFE (Compliant for 75%+ of working population)"
    elif report.overall_dcr <= 1.00:
        report.risk_level = "YELLOW"
        report.status_text = "[YELLOW] CAUTION (Borderline: monitor fatigue and posture)"
    else:
        report.risk_level = "RED"
        report.status_text = "[RED] HAZARDOUS (Exceeds physical capacity: engineering redesign required)"
        
    # Recommendations
    recs = []
    if report.spine_dcr > 0.85:
        recs.append("Spinal compression exceeds safety limits. Elevate pickup height or reduce load weight.")
    if task.start_z_m < 0.20:
        recs.append("Lifting directly from floor causes severe lumbar flexion (>60 deg). Install scissor lift table or pallet riser (target height: 600-800 mm).")
    if report.lifting_index > 1.0:
        recs.append("NIOSH Lifting Index > 1.0. Reduce horizontal reach distance (currently %.2f m) to bring load closer to body." % task.reach_h_m)
    if report.rula_score >= 5:
        recs.append("Upper limb postural score (RULA %d) is elevated. Re-position work surface to reduce shoulder abduction." % report.rula_score)
        
    if not recs:
        recs.append("Workstation geometry and manual material handling parameters are within optimal ergonomic limits.")
    report.recommendations = recs
    
    return report
