# -*- coding: utf-8 -*-
"""
WorksErgo R-Pro Edition - Main Command Engine
Launches the authentic Work(s) Ergo Task Analysis & Biomechanics Suite with 3D Scene synchronization.
Zero-cloud, air-gapped, standalone engineering solution.
"""
from vcCommand import *
import os
import sys
import math
import json
import subprocess

cmd = getCommand()
app = cmd.Application
sel = app.SelectionManager

def get_active_selection_parameters():
    data = {
        "load_kg": 12.0,
        "reach_mm": 400.0,
        "vert_mm": 750.0,
        "object_name": "None",
        "has_selection": False
    }
    try:
        if sel and sel.Selection and len(sel.Selection) > 0:
            target = sel.Selection[0]
            data["object_name"] = str(target.Name)
            data["has_selection"] = True

            # Extract 3D World Position
            mat = target.WorldPositionMatrix
            data["vert_mm"] = max(50.0, round(mat.P.Z, 1))
            data["reach_mm"] = max(150.0, round(math.sqrt(mat.P.X**2 + mat.P.Y**2), 1))

            # Extract Mass or Weight properties if available
            p_mass = target.findProperty("Mass") or target.findProperty("Weight") or target.findProperty("LoadWeightKg")
            if p_mass and p_mass.Value:
                try:
                    data["load_kg"] = float(p_mass.Value)
                except Exception:
                    pass
    except Exception as ex:
        print("[WorksErgo] Selection scan notice: " + str(ex))
    return data

def OnStart():
    pass

def OnExecute():
    print("[WorksErgo] Launching Work(s) Ergo Task Analysis Suite (R-Pro Edition)...")

    # 1. Capture coordinates from selected 3D object / manikin in R-Pro
    data = get_active_selection_parameters()

    # 2. Write real-time sync state
    tmp_dir = os.environ.get("TEMP", r"C:\Windows\Temp")
    state_file = os.path.join(tmp_dir, "worksergo_scene_state.json")
    try:
        with open(state_file, "w") as f:
            json.dump(data, f)
    except Exception as ex:
        print("[WorksErgo] State sync write error: " + str(ex))

    # 3. Launch authentic Work(s) Ergo Suite Window
    exe_path = r"D:\Apps\RProv222\WorksErgoSuite.exe"
    if not os.path.isfile(exe_path):
        exe_path = r"D:\Git\WorksErgoRPro\bin\WorksErgoSuite.exe"

    if os.path.isfile(exe_path):
        try:
            subprocess.Popen([exe_path])
            print("[WorksErgo] Work(s) Ergo Suite successfully displayed.")
        except Exception as ex:
            print("[WorksErgo] Launch error: " + str(ex))
    else:
        print("[WorksErgo] Error: WorksErgoSuite.exe not found at " + str(exe_path))
