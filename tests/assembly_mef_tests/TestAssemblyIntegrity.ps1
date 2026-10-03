# TestAssemblyIntegrity.ps1 - Level 2 MEF Contract & CLR Integrity Test
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$targetDll = Join-Path $projectRoot "src\Plugin.WorksErgo\bin\Plugin.WorksErgo.dll"

Write-Host "Running Level 2 MEF Assembly Integrity Test..." -ForegroundColor Cyan

if (!(Test-Path $targetDll)) {
    throw "Target assembly does not exist: $targetDll. Run build.ps1 first."
}

# 1. Verify Unsigned Assembly (StrongName = null)
[System.Reflection.Assembly]::LoadFrom("D:\Apps\RProv222\UX.Shared.dll") | Out-Null
[System.Reflection.Assembly]::LoadFrom("D:\Apps\RProv222\UX.Ribbon.dll") | Out-Null
[System.Reflection.Assembly]::LoadFrom("D:\Apps\RProv222\Create3D.Shared.dll") | Out-Null

$asm = [System.Reflection.Assembly]::LoadFrom($targetDll)
$token = $asm.GetName().GetPublicKeyToken()

if ($token -ne $null -and $token.Length -gt 0) {
    throw "Integrity Failure: Assembly has Strong Name token, which will cause CLR 0x80131044 in R-Pro v2.2.2!"
}
Write-Host " [PASS] Assembly is unsigned (PublicKeyToken=null) matching R-Pro CLR environment." -ForegroundColor Green

# 2. Verify MEF Exports
$types = $asm.GetTypes()
$exportMap = @{}

foreach ($t in $types) {
    $exports = $t.GetCustomAttributes([System.ComponentModel.Composition.ExportAttribute], $false)
    foreach ($exp in $exports) {
        $exportMap[$exp.ContractType.FullName] = $t.FullName
    }
}

$requiredContracts = @(
    "RProSoftDigital1.UX.Shared.IPlugin",
    "RProSoftDigital1.UX.Shared.IRibbonGroup",
    "RProSoftDigital1.UX.Shared.IActionItem"
)

foreach ($req in $requiredContracts) {
    if (!$exportMap.ContainsKey($req)) {
        throw "Integrity Failure: Missing required MEF export contract: $req"
    }
    Write-Host " [PASS] Found MEF Export for $req -> $($exportMap[$req])" -ForegroundColor Green
}

# 3. Verify MEF DirectoryCatalog Discovery (Simulating R-Pro Bootstrapper)
Add-Type -AssemblyName System.ComponentModel.Composition
$binDir = Split-Path $targetDll -Parent
$catalog = New-Object System.ComponentModel.Composition.Hosting.DirectoryCatalog($binDir, "Plugin.WorksErgo.dll")

foreach ($req in $requiredContracts) {
    $part = $catalog.Parts | Where-Object { $_.ExportDefinitions.ContractName -eq $req }
    if (!$part) {
        throw "DirectoryCatalog Failure: Part for $req was not discovered by MEF scanner!"
    }
    Write-Host " [PASS] MEF DirectoryCatalog discovered part for $req" -ForegroundColor Green
}

Write-Host "`n>>> All Level 2 Assembly Integrity Tests PASSED! <<<`n" -ForegroundColor Green

