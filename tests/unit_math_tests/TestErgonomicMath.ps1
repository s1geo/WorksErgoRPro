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
            Console.WriteLine("[1/3] Testing Baseline Ergonomic Lift (Male 50th, 10 kg at waist level)...");
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

            Console.WriteLine("\n[2/3] Testing Severe Hazardous Lift (Female 5th, 25 kg from floor)...");
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

            Console.WriteLine("\n[3/3] Testing Potvin MAE (2012) Fatigue Attenuation Curve...");
            double maeLow = ErgonomicMathEngine.CalculatePotvinMAE(0.05);
            double maeHigh = ErgonomicMathEngine.CalculatePotvinMAE(0.50);
            Console.WriteLine($"   MAE at 5% duty cycle: {maeLow:F3}");
            Console.WriteLine($"   MAE at 50% duty cycle: {maeHigh:F3}");

            if (maeLow < 0.80 || maeHigh > 0.65 || maeHigh >= maeLow)
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

            Console.WriteLine("\n>>> ALL BIOMECHANICAL MATH TESTS PASSED! <<<");
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
