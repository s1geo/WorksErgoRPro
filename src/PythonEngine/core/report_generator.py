# -*- coding: utf-8 -*-
"""
Report Generator for WorksErgo R-Pro Edition
Exports detailed ergonomics evaluation reports.
"""
import os, time

def generate_text_report(profile_name, task, report, output_path=None):
    """
    Generates a full multi-section ergonomics audit report.
    """
    lines = []
    lines.append("=" * 65)
    lines.append("        WORKS ERGO R-PRO EDITION - ERGONOMIC AUDIT REPORT")
    lines.append("=" * 65)
    lines.append("Generated: %s" % time.strftime("%Y-%m-%d %H:%M:%S"))
    lines.append("Profile:   %s" % profile_name)
    lines.append("Task Type: %s" % task.task_type)
    lines.append("-" * 65)
    
    # 1. Inputs Block
    lines.append("1. WORKSTATION & TASK PARAMETERS:")
    lines.append("   - Load Mass:            %.2f kg" % task.load_kg)
    lines.append("   - Start Pick Height (Z): %.3f m (%.1f mm)" % (task.start_z_m, task.start_z_m * 1000.0))
    lines.append("   - End Place Height (Z):  %.3f m (%.1f mm)" % (task.end_z_m, task.end_z_m * 1000.0))
    lines.append("   - Horizontal Reach (H):  %.3f m (%.1f mm)" % (task.reach_h_m, task.reach_h_m * 1000.0))
    lines.append("   - Task Frequency:        %.2f lifts/min" % task.freq_per_min)
    lines.append("   - Shift Duration:        %.1f hours" % task.duration_h)
    lines.append("-" * 65)
    
    # 2. Demand / Capacity Ratios (DCR)
    lines.append("2. DEMAND / CAPACITY EVALUATION (DCR):")
    lines.append("   - OVERALL DCR:          %.2f  [%s]" % (report.overall_dcr, report.risk_level))
    lines.append("   - Status:                %s" % report.status_text)
    lines.append("   - Lumbar Spine DCR:      %.2f  (Compression: %.0f N vs TLV)" % (report.spine_dcr, report.lcf_newtons))
    lines.append("   - Snook / LM-MMH DCR:    %.2f  (Load: %.1f kg vs MAWL: %.1f kg)" % (report.snook_dcr, task.load_kg, report.mawl_kg))
    lines.append("   - NIOSH Lifting Index:   %.2f  (RWL: %.2f kg)" % (report.lifting_index, report.rwl_kg))
    lines.append("   - RULA Score / DCR:      %d / %.2f" % (report.rula_score, report.rula_dcr))
    lines.append("   - REBA Score / DCR:      %d / %.2f" % (report.reba_score, report.reba_dcr))
    lines.append("-" * 65)
    
    # 3. Biomechanics & Fatigue
    lines.append("3. DETAILED BIOMECHANICS & FATIGUE (Potvin 2012):")
    lines.append("   - Effective Duration:    %.3f seconds" % report.effective_duration_s)
    lines.append("   - Duty Cycle (DC):       %.4f (%.2f%%)" % (report.duty_cycle, report.duty_cycle * 100.0))
    lines.append("   - Max Acceptable Effort: %.3f (MAE)" % report.mae)
    lines.append("   - Peak Shear Force:      %.1f N" % report.shear_newtons)
    lines.append("-" * 65)
    
    # 4. Recommendations
    lines.append("4. ERGONOMIC RECOMMENDATIONS & CORRECTIVE ACTIONS:")
    for i, r in enumerate(report.recommendations):
        lines.append("   [%d] %s" % (i + 1, r))
    lines.append("=" * 65)
    
    report_text = "\n".join(lines)
    
    if output_path:
        try:
            with open(output_path, "w") as f:
                f.write(report_text)
        except Exception:
            pass
            
    return report_text
