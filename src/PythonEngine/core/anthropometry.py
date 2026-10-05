# -*- coding: utf-8 -*-
"""
Anthropometry and 15-Segment Linked Model
References: NHANES (Fryar et al., 2021); Tilley (1993); de Leva (1996); Chaffin et al. (2006)
"""
try:
    from ergo_types import AnthropometryProfile
except (ImportError, ValueError):
    try:
        from core.ergo_types import AnthropometryProfile
    except (ImportError, ValueError):
        from .ergo_types import AnthropometryProfile


PROFILES = {
    "Male 50th (175cm/78kg)": AnthropometryProfile("Male 50th", 1.750, 78.0, "M"),
    "Male 95th (187cm/98kg)": AnthropometryProfile("Male 95th", 1.870, 98.0, "M"),
    "Female 5th (152cm/50kg)": AnthropometryProfile("Female 5th", 1.520, 50.0, "F"),
    "Female 50th (162cm/62kg)": AnthropometryProfile("Female 50th", 1.620, 62.0, "F"),
    "Female 50th (162cm/73.5kg)": AnthropometryProfile("Female 50th Work(s)", 1.620, 73.5, "F")
}

def get_profile(name_or_key):
    if name_or_key in PROFILES:
        return PROFILES[name_or_key]
    for k in PROFILES:
        if name_or_key.lower() in k.lower():
            return PROFILES[k]
    return PROFILES["Male 50th (175cm/78kg)"]

def calculate_segment_parameters(profile):
    """
    Computes masses (kg), lengths (m), and center of mass offsets for the 15 segments.
    """
    H = profile.stature_m
    M = profile.mass_kg
    
    segments = {
        "head_neck": {
            "mass": 0.067 * M,
            "length": 0.107 * H,
            "com_ratio": 0.500
        },
        "torso": {
            "mass": 0.433 * M,
            "length": 0.288 * H,
            "com_ratio": 0.440 # L5/S1 to mid-torso
        },
        "upper_arm": {
            "mass": 0.027 * M,
            "length": 0.186 * H,
            "com_ratio": 0.436
        },
        "forearm": {
            "mass": 0.016 * M,
            "length": 0.146 * H,
            "com_ratio": 0.430
        },
        "hand": {
            "mass": 0.006 * M,
            "length": 0.060 * H,
            "com_ratio": 0.506
        },
        "thigh": {
            "mass": 0.142 * M,
            "length": 0.245 * H,
            "com_ratio": 0.433
        },
        "shank": {
            "mass": 0.043 * M,
            "length": 0.246 * H,
            "com_ratio": 0.433
        },
        "foot": {
            "mass": 0.014 * M,
            "length": 0.152 * H,
            "com_ratio": 0.500
        }
    }
    return segments
