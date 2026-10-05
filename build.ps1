# build.ps1 - Automated Build Script for WorksErgo R-Pro Edition
param (
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Building WorksErgo R-Pro Edition (.NET Framework 4.8 / MEF)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$cscPath = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
if (!(Test-Path $cscPath)) {
    $cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
}
Write-Host "[1/3] Using C# Compiler: $cscPath" -ForegroundColor Green

$projectRoot = $PSScriptRoot
$outDir = Join-Path $projectRoot "src\Plugin.WorksErgo\bin"
if (!(Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir -Force | Out-Null }
$targetDll = Join-Path $outDir "Plugin.WorksErgo.dll"

$netFw = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
$wpfDir = "$netFw\WPF"
$libDir = Join-Path $projectRoot "lib"
if (Test-Path (Join-Path $libDir "UX.Shared.dll")) {
    $rproDir = $libDir
    Write-Host "[1.1] Using portable reference assemblies from: $libDir" -ForegroundColor DarkCyan
} else {
    $rproDir = "D:\Apps\RProv222"
    Write-Host "[1.1] Using host reference assemblies from: $rproDir" -ForegroundColor DarkCyan
}

$references = @(
    "$rproDir\UX.Shared.dll",
    "$rproDir\UX.Ribbon.dll",
    "$rproDir\Create3D.Shared.dll",
    "$rproDir\Caliburn.Micro.dll",
    "$netFw\System.ComponentModel.Composition.dll",
    "$wpfDir\PresentationCore.dll",
    "$wpfDir\PresentationFramework.dll",
    "$wpfDir\WindowsBase.dll",
    "$netFw\System.Xaml.dll"
)

foreach ($ref in $references) {
    if (!(Test-Path $ref)) {
        Write-Error "Required reference assembly not found: $ref"
    }
}

$sourceFiles = Get-ChildItem -Path "$projectRoot\src\Plugin.WorksErgo" -Filter "*.cs" -Recurse | Select-Object -ExpandProperty FullName

Write-Host "[2/3] Compiling $($sourceFiles.Count) source files..." -ForegroundColor Green
$refArgs = $references | ForEach-Object { "/reference:`"$_`"" }

& $cscPath /target:library /out:"$targetDll" /optimize+ /nologo $refArgs $sourceFiles

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
}

Write-Host "[3/3] Build succeeded -> $targetDll" -ForegroundColor Green
Get-Item $targetDll | Select-Object Name, Length, LastWriteTime
