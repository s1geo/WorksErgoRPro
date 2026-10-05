# -*- coding: utf-8 -*-
from vcApplication import *

COMMAND_NAME = 'cmdWorksErgo'
MENU_TITLE = 'Work(s) Ergo'

def OnAppInitialized():
    try:
        cmduri = getApplicationPath() + 'cmdWorksErgo.py'
        cmd = loadCommand(COMMAND_NAME, cmduri)
        if not cmd:
            print("[WorksErgo] Error: Could not load command from " + str(cmduri))
            return

        # 1. Create a dedicated Ribbon Group on Home Tab
        try:
            addUxSite('VcTabHome/Works Ergo', -1)
            addMenuItem('VcTabHome/Works Ergo', MENU_TITLE, -1, COMMAND_NAME)
        except Exception:
            pass

        # 2. Register in Home Tab -> Tools group
        try:
            addMenuItem('VcTabHome/VcRibbonTools', MENU_TITLE, -1, COMMAND_NAME)
        except Exception:
            pass

        # 3. Register in Modeling Wizards menu
        try:
            addMenuItem(VC_MENU_MODELING_WIZARDS + '/Component Wizards', MENU_TITLE, -1, COMMAND_NAME)
        except Exception:
            pass

        # 4. Register in 3D Viewport Context Menu (Right Click in 3D)
        try:
            addMenuItem(VC_MENU_HOME, MENU_TITLE, -1, COMMAND_NAME)
        except Exception:
            pass

        print("[WorksErgo] Work(s) Ergo R-Pro Edition successfully loaded and registered.")
    except Exception as ex:
        print("[WorksErgo] Initialization error: " + str(ex))
