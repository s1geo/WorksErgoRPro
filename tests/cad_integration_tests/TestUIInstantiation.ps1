# TestUIInstantiation.ps1 - Level 3 WPF View & ViewModel Integration Test
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$targetDll = Join-Path $projectRoot "src\Plugin.WorksErgo\bin\Plugin.WorksErgo.dll"

Write-Host "Running Level 3 WPF View & ViewModel Integration Test..." -ForegroundColor Cyan

$testScript = @"
using System;
using System.Threading;
using System.Windows;
using WorksErgoRPro.Biomechanics;
using WorksErgoRPro.ViewModels;
using WorksErgoRPro.Views;

namespace UITests
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var requestedName = new System.Reflection.AssemblyName(eventArgs.Name).Name;
                var pluginBin = @"D:\Git\WorksErgoRPro\src\Plugin.WorksErgo\bin\" + requestedName + ".dll";
                if (System.IO.File.Exists(pluginBin)) return System.Reflection.Assembly.LoadFrom(pluginBin);
                var rproBin = @"D:\Apps\RProv222\" + requestedName + ".dll";
                if (System.IO.File.Exists(rproBin)) return System.Reflection.Assembly.LoadFrom(rproBin);
                return null;
            };

            return RunTest();
        }

        static int RunTest()
        {
            try
            {
                Console.WriteLine("[1/3] Instantiating WorksErgoPaneViewModel...");
                var vm = new WorksErgoPaneViewModel();
                Console.WriteLine(string.Format("   Default DCR: {0}, Risk: {1}", vm.OverallDcrText, vm.RiskCategory));

                Console.WriteLine("[2/3] Instantiating WorksErgoPaneView and binding DataContext...");
                var view = new WorksErgoPaneView { DataContext = vm };

                Console.WriteLine("[3/3] Simulating interactive user parameter changes...");
                // Increase load to 25 kg
                vm.LoadWeightKg = 25.0;
                Console.WriteLine(string.Format("   After 25kg load: DCR = {0}, Risk = {1}, Limiting = {2}", vm.OverallDcrText, vm.RiskCategory, vm.LimitingFactor));
                if (vm.OverallDCR <= 0.85)
                {
                    Console.WriteLine("   [FAIL] 25kg load should trigger warning or hazard!");
                    return 1;
                }

                // Change percentile to Female 5th
                vm.SelectedPercentile = DHMPercentile.Female5th;
                Console.WriteLine(string.Format("   Female 5th percentile: DCR = {0}, RWL = {1} kg", vm.OverallDcrText, vm.NioshRWLKg));

                // Lower vertical height to floor (150 mm)
                vm.VerticalMm = 150.0;
                Console.WriteLine(string.Format("   Floor pick (150mm): DCR = {0}, Recommendation: {1}", vm.OverallDcrText, vm.Recommendation));

                // Test Subtasks Multi-Task Job Manager
                Console.WriteLine("\n[4/4] Testing Multi-Subtask Job Manager and CAD Snap in UI...");
                vm.AddCurrentAsSubtask();
                vm.SelectedTask = TaskType.Carrying;
                vm.AddCurrentAsSubtask();
                Console.WriteLine(string.Format("   Subtasks Count: {0}", vm.Subtasks.Count));
                if (vm.Subtasks.Count != 2)
                {
                    Console.WriteLine("   [FAIL] Subtasks collection did not increment!");
                    return 1;
                }

                vm.EvaluateShiftJob();
                Console.WriteLine(string.Format("   Shift Composite DCR: {0}, LCFCD: {1:F3}, EAWS: {2:F1}", vm.CompositeOverallDcrText, vm.CompositeLcfcd, vm.CompositeEawsScore));

                // Test 3D CAD Snap
                vm.SnapSelected3DObject();
                Console.WriteLine(string.Format("   CAD Status: {0}", vm.CadStatusMessage));

                // Test EAWS properties
                Console.WriteLine(string.Format("   Current EAWS Score: {0:F1} ({1})", vm.EawsScore, vm.EawsTrafficLight));

                Console.WriteLine("\n>>> Level 3 WPF UI & ViewModel Integration Tests PASSED! <<<");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("FATAL UI EXCEPTION: {0}\n{1}", ex.Message, ex.StackTrace));
                return 2;
            }
        }
    }
}
"@

$testDir = Join-Path $projectRoot "tests\cad_integration_tests\bin"
if (!(Test-Path $testDir)) { New-Item -ItemType Directory -Path $testDir -Force | Out-Null }
$testExe = Join-Path $testDir "TestUI.exe"
$testSource = Join-Path $testDir "TestUIProgram.cs"
Set-Content -Path $testSource -Value $testScript

$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (!(Test-Path $cscPath)) { $cscPath = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe" }

$netFw = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
$wpfDir = "$netFw\WPF"
$rproDir = "D:\Apps\RProv222"

& $cscPath /target:exe /out:$testExe /nologo `
  /reference:"$targetDll" `
  /reference:"$rproDir\UX.Shared.dll" `
  /reference:"$rproDir\Caliburn.Micro.dll" `
  /reference:"$wpfDir\PresentationCore.dll" `
  /reference:"$wpfDir\PresentationFramework.dll" `
  /reference:"$wpfDir\WindowsBase.dll" `
  /reference:"$netFw\System.Xaml.dll" `
  $testSource

if ($LASTEXITCODE -ne 0) { throw "Compilation of UI test failed." }

& $testExe
if ($LASTEXITCODE -ne 0) { throw "Level 3 UI integration test failed." }
