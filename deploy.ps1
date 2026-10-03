# deploy.ps1 - Safe Deployment of Plugin.WorksErgo.dll to R-Pro v2.2.2
$ErrorActionPreference = "Stop"

$projectRoot = $PSScriptRoot
$sourceDll = Join-Path $projectRoot "src\Plugin.WorksErgo\bin\Plugin.WorksErgo.dll"
$targetDir = "D:\Apps\RProv222"
$targetDll = Join-Path $targetDir "Plugin.WorksErgo.dll"

if (!(Test-Path $sourceDll)) {
    Write-Host "Binary not found. Triggering build first..." -ForegroundColor Yellow
    & "$projectRoot\build.ps1"
}

Write-Host "Checking for running R-Pro processes..." -ForegroundColor Cyan
$procs = Get-Process -Name "rpro*", "VisualComponents*" -ErrorAction SilentlyContinue
if ($procs) {
    Write-Host "Stopping running R-Pro processes..." -ForegroundColor Yellow
    $procs | Stop-Process -Force
    Start-Sleep -Seconds 1
}

Write-Host "Deploying $sourceDll -> $targetDll..." -ForegroundColor Green
Copy-Item -Path $sourceDll -Destination $targetDll -Force

Write-Host "Deployment complete! Plugin.WorksErgo.dll is active in R-Pro v2.2.2." -ForegroundColor Green
Get-Item $targetDll | Select-Object Name, Length, LastWriteTime
