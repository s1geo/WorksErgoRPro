# RunVisualTests.ps1 - Compiles and Launches Native Desktop Manikin Visualizer
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$targetDll = Join-Path $projectRoot "src\Plugin.WorksErgo\bin\Plugin.WorksErgo.dll"

if (!(Test-Path $targetDll)) {
    Write-Host "Compiling Plugin.WorksErgo.dll first..." -ForegroundColor Cyan
    & "$projectRoot\build.ps1"
}

$binDir = Join-Path $PSScriptRoot "bin"
if (!(Test-Path $binDir)) { New-Item -ItemType Directory -Path $binDir -Force | Out-Null }
$targetExe = Join-Path $binDir "VisualTestApp.exe"
$sourceFile = Join-Path $PSScriptRoot "VisualTestApp.cs"

Write-Host "Compiling Visual Manikin Autotest GUI Application..." -ForegroundColor Cyan

$cscPath = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
if (!(Test-Path $cscPath)) { $cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" }

$netFw = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
$wpfDir = "$netFw\WPF"
$rproDir = "D:\Apps\RProv222"

& $cscPath /target:winexe /out:$targetExe /nologo `
  /reference:"$targetDll" `
  /reference:"$rproDir\UX.Shared.dll" `
  /reference:"$rproDir\Caliburn.Micro.dll" `
  /reference:"$wpfDir\PresentationCore.dll" `
  /reference:"$wpfDir\PresentationFramework.dll" `
  /reference:"$wpfDir\WindowsBase.dll" `
  /reference:"$netFw\System.Xaml.dll" `
  $sourceFile

if ($LASTEXITCODE -ne 0) { throw "Compilation of VisualTestApp failed." }

# Copy referenced DLLs to bin for local execution
Copy-Item -Path $targetDll -Destination $binDir -Force
Copy-Item -Path "$rproDir\UX.Shared.dll" -Destination $binDir -Force
Copy-Item -Path "$rproDir\Caliburn.Micro.dll" -Destination $binDir -Force

Write-Host "Visual Test App compiled cleanly -> $targetExe" -ForegroundColor Green
Write-Host "Launching Visual Manikin Autotest Window..." -ForegroundColor Green

Start-Process -FilePath $targetExe
