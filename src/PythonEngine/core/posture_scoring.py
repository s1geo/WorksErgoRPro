# -*- coding: utf-8 -*-
"""
Posture Scoring Engines: RULA & REBA
References:
- McAtamney & Corlett (1993) Applied Ergonomics, 24(2), 91-99 (RULA)
- Hignett & McAtamney (2000) Applied Ergonomics, 31(2), 201-216 (REBA)
"""

def evaluate_rula(trunk_flexion_deg, upper_arm_deg, forearm_deg, load_kg, is_muscle_repetitive=True):
    """
    Computes RULA Grand Score (1 to 7).
    """
    # 1. Upper Arm (1-6)
    if upper_arm_deg < 20:
        ua = 1
    elif upper_arm_deg <= 45:
        ua = 2
    elif upper_arm_deg <= 90:
        ua = 3
    else:
        ua = 4
    if upper_arm_deg > 45:
        ua += 1 # shoulder raised
        
    # 2. Lower Arm / Forearm (1-3)
    if 60 <= forearm_deg <= 100:
        fa = 1
    else:
        fa = 2
        
    # 3. Wrist (1-4)
    wrist = 2
    
    # 4. Wrist Twist (1-2)
    twist = 1
    
    # Table A Score (Simplified matrix)
    score_a = min(max(ua + (fa - 1) + (wrist - 1), 1), 8)
    
    # Muscle & Load for Upper Extremities
    muscle_a = 1 if is_muscle_repetitive else 0
    load_a = 0
    if 2.0 <= load_kg <= 10.0:
        load_a = 1
    elif load_kg > 10.0:
        load_a = 2
    wrist_arm_score = min(score_a + muscle_a + load_a, 8)
    
    # 5. Neck (1-6)
    neck = 2 if trunk_flexion_deg > 30 else 1
    
    # 6. Trunk (1-6)
    if trunk_flexion_deg <= 10:
        trunk = 1
    elif trunk_flexion_deg <= 20:
        trunk = 2
    elif trunk_flexion_deg <= 60:
        trunk = 3
    else:
        trunk = 4
        
    # 7. Legs (1-2)
    legs = 1
    
    # Table B Score
    score_b = min(max(neck + (trunk - 1) + (legs - 1), 1), 7)
    
    # Muscle & Load for Trunk/Neck
    neck_trunk_score = min(score_b + muscle_a + load_a, 7)
    
    # Table C Grand Score (1 to 7)
    # Average interpolation
    grand_score = int(round((wrist_arm_score + neck_trunk_score) / 2.0))
    grand_score = min(max(grand_score, 1), 7)
    
    rula_dcr = float(grand_score) / 7.0 # Normalized against max score of 7
    
    if grand_score <= 2:
        status = "GREEN: Acceptable posture"
    elif grand_score <= 4:
        status = "YELLOW: Further investigation, change may be needed"
    elif grand_score <= 6:
        status = "ORANGE: Investigation and change required soon"
    else:
        status = "RED: Investigation and change required immediately"
        
    return {
        "grand_score": grand_score,
        "dcr": rula_dcr,
        "status": status
    }

def evaluate_reba(trunk_flexion_deg, upper_arm_deg, forearm_deg, load_kg, is_coupling_good=True):
    """
    Computes REBA Grand Score (1 to 15).
    """
    # Group A: Trunk (1-5), Neck (1-3), Legs (1-4)
    if trunk_flexion_deg <= 10:
        trunk = 1
    elif trunk_flexion_deg <= 20:
        trunk = 2
    elif trunk_flexion_deg <= 60:
        trunk = 3
    else:
        trunk = 4
        
    neck = 2 if trunk_flexion_deg > 30 else 1
    legs = 2 if trunk_flexion_deg > 45 else 1 # knee flexion
    
    table_a = min(trunk + (neck - 1) + (legs - 1), 9)
    load_score = 0
    if 5.0 <= load_kg <= 10.0:
        load_score = 1
    elif load_kg > 10.0:
        load_score = 2
    score_a = table_a + load_score
    
    # Group B: Upper Arm (1-6), Lower Arm (1-2), Wrist (1-3)
    if upper_arm_deg <= 20:
        ua = 1
    elif upper_arm_deg <= 45:
        ua = 2
    elif upper_arm_deg <= 90:
        ua = 3
    else:
        ua = 4
    fa = 1 if (60 <= forearm_deg <= 100) else 2
    wrist = 1
    
    table_b = min(ua + (fa - 1) + (wrist - 1), 9)
    coupling_score = 0 if is_coupling_good else 1
    score_b = table_b + coupling_score
    
    # Table C + Activity Score
    table_c = int(round((score_a + score_b) / 1.5))
    activity_score = 1 # repetitive movement
    grand_score = min(max(table_c + activity_score, 1), 15)
    
    reba_dcr = float(grand_score) / 15.0 # Normalized against max score of 15
    
    if grand_score <= 1:
        status = "GREEN: Negligible risk"
    elif grand_score <= 3:
        status = "GREEN: Low risk, change may be needed"
    elif grand_score <= 7:
        status = "YELLOW: Medium risk, further investigation needed"
    elif grand_score <= 10:
        status = "ORANGE: High risk, investigate and implement change"
    else:
        status = "RED: Very high risk, implement change immediately"
        
    return {
        "grand_score": grand_score,
        "dcr": reba_dcr,
        "status": status
    }
