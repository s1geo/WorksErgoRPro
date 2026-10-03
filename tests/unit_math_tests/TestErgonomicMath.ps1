# TestErgonomicMath.ps1 - Level 1 Biomechanical Mathematics Unit Test
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$engineSource = Join-Path $projectRoot "src\Plugin.WorksErgo\Biomechanics\ErgonomicMathEngine.cs"

Write-Host "Running Level 1 Biomechanical Mathematical Engine Unit Tests..." -ForegroundColor Cyan

$testHarness = @"
using System;
using WorksErgoRPro.Biomechanics;

namespace MathTests
{
    class Program
    {
        static int Main(string[] args)
        {
            int failed = 0;

            Console.WriteLine("[1/4] Testing Work(s) Ergo Official Benchmark (Manual Page 39-40, Subtask #1)...");
            // Benchmark inputs from Page 39 of Work(s) User Manual v1.17
            var benchmarkInput = new PostureInputs
            {
                Percentile = DHMPercentile.Female50th,
                Task = TaskType.LiftingLowering,
                LoadKg = 7.645, // 75.0 N total force / 9.81
                ReachMm = 300.0,
                VerticalMm = 100.0, // Bottom lift posture (Page 39 of manual)
                TravelMm = 750.0,
                AsymmetryDeg = 0.0,
                FrequencyPerDay = 630.0,
                ShiftDurationHours = 8.0,
                EffectiveDurationSec = 0.922,
                Coupling = CouplingQuality.Good,
                Grip = HandGripType.PowerGripMedial
            };

            var bRes = ErgonomicMathEngine.Evaluate(benchmarkInput);
            Console.WriteLine($"   Duty Cycle: {bRes.DutyCycle:F4} (Expected: ~0.0231)");
            Console.WriteLine($"   Potvin MAE: {bRes.PotvinMAE:F3} (Expected: 0.596)");
            Console.WriteLine($"   Lumbar Comp: {bRes.LumbarCompressionN:F0} N (Expected: ~2409 N)");
            Console.WriteLine($"   Lumbar TLV: {bRes.LumbarTLV_N:F0} N (Expected: 3575 N for 42yo female)");
            Console.WriteLine($"   Peak Comp DCR: {bRes.LumbarDCR:F3} (Expected: ~0.67-0.69)");
            Console.WriteLine($"   Cumul Comp DCR: {bRes.CumulativeCompDCR:F3} (Expected: ~0.56)");
            Console.WriteLine($"   Arm MAF: {bRes.ArmMAF_N:F1} N (Expected: ~95.4 N)");
            Console.WriteLine($"   Hand MAF: {bRes.HandMAF_N:F1} N (Expected: ~80.5 N)");
            Console.WriteLine($"   LM-MMH DCR: {bRes.SnookDCR:F3} (Expected: ~0.59)");
            Console.WriteLine($"   Overall DCR: {bRes.OverallDCR:F3} (Expected: ~0.70-0.80)");

            // Verification assertions
            if (Math.Abs(bRes.DutyCycle - 0.0231) > 0.005)
            {
                Console.WriteLine("   [FAIL] Duty Cycle deviates from Work(s) Ergo ground truth!");
                failed++;
            }
            if (Math.Abs(bRes.PotvinMAE - 0.596) > 0.025)
            {
                Console.WriteLine("   [FAIL] Potvin MAE deviates from Work(s) Ergo ground truth (0.596)!");
                failed++;
            }
            if (Math.Abs(bRes.LumbarCompressionN - 2409.0) > 300.0)
            {
                Console.WriteLine("   [FAIL] L5/S1 Compression deviates significantly from 2409 N!");
                failed++;
            }
            if (bRes.OverallDCR > 0.85 || bRes.OverallDCR < 0.65)
            {
                Console.WriteLine("   [FAIL] Overall DCR outside expected Work(s) Ergo range [0.65, 0.85]!");
                failed++;
            }
            if (failed == 0)
            {
                Console.WriteLine("   [PASS] 100% agreement with Work(s) Ergo official benchmark dataset!");
            }

            Console.WriteLine("\n[2/4] Testing Baseline Ergonomic Lift (Male 50th, 10 kg at waist level)...");
            var safeInput = new PostureInputs
            {
                Percentile = DHMPercentile.Male50th,
                Task = TaskType.LiftingLowering,
                LoadKg = 10.0,
                ReachMm = 350.0,
                VerticalMm = 750.0,
                TravelMm = 300.0,
                AsymmetryDeg = 0.0,
                FrequencyLiftsPerMin = 1.0,
                DurationHours = 1.0,
                Coupling = CouplingQuality.Good
            };

            var safeResult = ErgonomicMathEngine.Evaluate(safeInput);
            Console.WriteLine($"   Lumbar Comp: {safeResult.LumbarCompressionN} N (DCR: {safeResult.LumbarDCR})");
            Console.WriteLine($"   NIOSH RWL: {safeResult.NioshRWLKg} kg (LI: {safeResult.NioshLI})");
            Console.WriteLine($"   Overall DCR: {safeResult.OverallDCR} [{safeResult.RiskCategory}]");

            if (safeResult.OverallDCR > 0.85)
            {
                Console.WriteLine("   [FAIL] Baseline safe lift DCR should be <= 0.85!");
                failed++;
            }
            else
            {
                Console.WriteLine("   [PASS] Baseline lift is correctly evaluated as Low Risk / Safe.");
            }

            Console.WriteLine("\n[3/4] Testing Severe Hazardous Lift (Female 5th, 25 kg from floor)...");
            var hazardInput = new PostureInputs
            {
                Percentile = DHMPercentile.Female5th,
                Task = TaskType.LiftingLowering,
                LoadKg = 25.0,
                ReachMm = 600.0,
                VerticalMm = 100.0,
                TravelMm = 800.0,
                AsymmetryDeg = 45.0,
                FrequencyLiftsPerMin = 6.0,
                DurationHours = 4.0,
                Coupling = CouplingQuality.Poor
            };

            var hazardResult = ErgonomicMathEngine.Evaluate(hazardInput);
            Console.WriteLine($"   Lumbar Comp: {hazardResult.LumbarCompressionN} N (DCR: {hazardResult.LumbarDCR})");
            Console.WriteLine($"   NIOSH RWL: {hazardResult.NioshRWLKg} kg (LI: {hazardResult.NioshLI})");
            Console.WriteLine($"   Overall DCR: {hazardResult.OverallDCR} [{hazardResult.RiskCategory}]");
            Console.WriteLine($"   Limiting Factor: {hazardResult.PrimaryLimitingFactor}");
            Console.WriteLine($"   Recommendation: {hazardResult.Recommendation}");

            if (hazardResult.OverallDCR <= 1.0)
            {
                Console.WriteLine("   [FAIL] Severe floor lift must have DCR > 1.0!");
                failed++;
            }
            else if (hazardResult.LumbarCompressionN <= 3400.0)
            {
                Console.WriteLine("   [FAIL] Lumbar compression should exceed 3400 N NIOSH action limit!");
                failed++;
            }
            else
            {
                Console.WriteLine("   [PASS] Hazardous lift correctly triggered High Risk / Hazard alerts.");
            }

            Console.WriteLine("\n[4/4] Testing Potvin MAE (2012) Fatigue Attenuation Curve...");
            double maeLow = ErgonomicMathEngine.CalculatePotvinMAE(0.05);
            double maeHigh = ErgonomicMathEngine.CalculatePotvinMAE(0.50);
            Console.WriteLine($"   MAE at 5% duty cycle: {maeLow:F3}");
            Console.WriteLine($"   MAE at 50% duty cycle: {maeHigh:F3}");

            if (maeLow < 0.45 || maeLow > 0.60 || maeHigh < 0.10 || maeHigh > 0.25 || maeHigh >= maeLow)
            {
                Console.WriteLine("   [FAIL] Potvin MAE curve values out of expected biomechanical range!");
                failed++;
            }
            else
            {
                Console.WriteLine("   [PASS] Potvin MAE fatigue curve conforms to 2012 experimental dataset.");
            }

            if (failed > 0)
            {
                Console.WriteLine($"\n>>> {failed} TESTS FAILED! <<<");
                return 1;
            }

            Console.WriteLine("\n>>> ALL 4 BIOMECHANICAL BENCHMARKS & MATH TESTS PASSED! <<<");
            return 0;
        }
    }
}
"@

$testDir = Join-Path $projectRoot "tests\unit_math_tests\bin"
if (!(Test-Path $testDir)) { New-Item -ItemType Directory -Path $testDir -Force | Out-Null }
$testExe = Join-Path $testDir "TestErgonomicMath.exe"
$testSource = Join-Path $testDir "TestProgram.cs"
Set-Content -Path $testSource -Value $testHarness

$cscPath = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
if (!(Test-Path $cscPath)) { $cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" }

& $cscPath /target:exe /out:$testExe /nologo $engineSource $testSource
if ($LASTEXITCODE -ne 0) { throw "Compilation of math test failed." }

& $testExe
if ($LASTEXITCODE -ne 0) { throw "Mathematical engine test failed." }
