# -*- coding: utf-8 -*-
"""
Verification Test Suite for WorksErgo R-Pro Edition
Validates against Work(s) User Manual v1.17 scientific benchmarks.
"""
import os, sys

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
CORE_DIR = os.path.join(BASE_DIR, "core")
if BASE_DIR not in sys.path:
    sys.path.insert(0, BASE_DIR)
if CORE_DIR not in sys.path:
    sys.path.insert(0, CORE_DIR)

from ergo_types import TaskParameters
from scene_bridge import evaluate_full_ergonomics
from report_generator import generate_text_report

def run_tests():
    print("==================================================")
    print("RUNNING WORKSERGO R-PRO SCIENTIFIC VERIFICATION...")
    print("==================================================")

    # Benchmark test from Page 39 of Work(s) User Manual:
    # 7.5 kg load lifted from 0.08m (floor) to 0.85m (table) at reach 0.35m
    # 630 lifts per 8 hour shift = 1.31 lifts/min
    task = TaskParameters(
        task_type="Lift Load",
        load_kg=7.5,
        start_z_m=0.08,
        end_z_m=0.85,
        reach_h_m=0.35,
        freq_per_min=1.31,
        duration_h=8.0
    )

    report = evaluate_full_ergonomics("Female 50th", task)

    print("\n--- Benchmark Results ---")
    print("Calculated L5/S1 Compression (LCF): %.1f N" % report.lcf_newtons)
    print("Spine DCR:                         %.2f [%s]" % (report.spine_dcr, report.spine_status))
    print("Snook MAWL:                        %.1f kg" % report.mawl_kg)
    print("Snook DCR:                         %.2f" % report.snook_dcr)
    print("NIOSH RWL:                         %.1f kg (Lifting Index: %.2f)" % (report.rwl_kg, report.lifting_index))
    print("RULA Score:                        %d / 7" % report.rula_score)
    print("REBA Score:                        %d / 15" % report.reba_score)
    print("OVERALL DCR:                       %.2f [%s]" % (report.overall_dcr, report.risk_level))

    # Assertions
    # 1. Spinal compression should be roughly 2100-2800 N
    assert 2000.0 <= report.lcf_newtons <= 2800.0, "Spine compression out of expected range: %f" % report.lcf_newtons
    # 2. Overall DCR should be Yellow (0.85 - 1.0)
    assert report.risk_level == "YELLOW", "Expected YELLOW risk level, got: %s" % report.risk_level
    # 3. Recommendations should identify floor lift
    assert any("floor" in r.lower() or "z=" in r.lower() or "height" in r.lower() for r in report.recommendations), "Expected floor recommendation"

    # Test report generation
    out_report = os.path.join(BASE_DIR, "test_output_report.txt")
    generate_text_report("Female 50th (162cm/62kg)", task, report, out_report)
    assert os.path.exists(out_report), "Report file was not created"
    os.remove(out_report)

    print("\n>>> ALL SCIENTIFIC BENCHMARKS & VERIFICATIONS PASSED 100%! <<<\n")

if __name__ == "__main__":
    run_tests()
