# -*- coding: utf-8 -*-
"""
Data types and transfer objects for WorksErgo R-Pro Edition.
Python 2.7 and 3.x compatible.
"""

class AnthropometryProfile(object):
    def __init__(self, name, stature_m, mass_kg, sex="M"):
        self.name = name
        self.stature_m = float(stature_m)
        self.mass_kg = float(mass_kg)
        self.sex = sex # "M" or "F"

class TaskParameters(object):
    def __init__(self, task_type="Lift", load_kg=5.0, start_z_m=0.0, end_z_m=0.8,
                 reach_h_m=0.35, freq_per_min=1.0, duration_h=8.0, asymmetry_deg=0.0,
                 push_pull_force_n=0.0):
        self.task_type = task_type # Lift, Lower, Push, Pull, Carry, Static
        self.load_kg = float(load_kg)
        self.start_z_m = float(start_z_m)
        self.end_z_m = float(end_z_m)
        self.reach_h_m = float(reach_h_m)
        self.freq_per_min = float(freq_per_min)
        self.duration_h = float(duration_h)
        self.asymmetry_deg = float(asymmetry_deg)
        self.push_pull_force_n = float(push_pull_force_n)

class ErgonomicsReport(object):
    def __init__(self):
        self.overall_dcr = 0.0
        self.risk_level = "GREEN" # GREEN, YELLOW, RED
        self.status_text = "OK"
        
        # Biomechanics (Spine)
        self.lcf_newtons = 0.0
        self.shear_newtons = 0.0
        self.spine_dcr = 0.0
        self.spine_status = "OK"
        
        # Psychophysical (Snook / LM-MMH)
        self.mawl_kg = 0.0
        self.snook_dcr = 0.0
        self.snook_status = "OK"
        
        # NIOSH RNLE
        self.rwl_kg = 0.0
        self.lifting_index = 0.0
        self.niosh_status = "OK"
        
        # Posture Scoring
        self.rula_score = 1
        self.rula_dcr = 0.0
        self.reba_score = 1
        self.reba_dcr = 0.0
        
        # Fatigue (Potvin MAE)
        self.effective_duration_s = 0.0
        self.duty_cycle = 0.0
        self.mae = 1.0
        
        # Diagnostic messages
        self.recommendations = []
