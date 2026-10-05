# tests/TestComponentPackages.ps1 - Validates .rpro component archives for R-Pro v2.2.2
$ErrorActionPreference = "Stop"

Write-Host ">>> [LEVEL 4] R-Pro eCatalog Component Package Integrity Tests <<<" -ForegroundColor Cyan

$projectRoot = Split-Path -Parent $PSScriptRoot
$componentsDir = Join-Path $projectRoot "components"
$myModelsDir = "C:\Users\Jojo\Documents\R-Pro\0.2\My Models\WorksErgo"

$components = @("Barrier.rpro", "DHM_Worker.rpro")

Add-Type -AssemblyName System.IO.Compression.FileSystem

foreach ($comp in $components) {
    $repoFile = Join-Path $componentsDir $comp
    $deployedFile = Join-Path $myModelsDir $comp

    Write-Host "[Testing $comp]..." -ForegroundColor Yellow

    if (!(Test-Path $repoFile)) {
        throw "Component package not found in repo: $repoFile"
    }
    if (!(Test-Path $deployedFile)) {
        throw "Component package not deployed in My Models: $deployedFile"
    }

    # Test ZIP integrity
    $zip = [System.IO.Compression.ZipFile]::OpenRead($repoFile)
    $entryNames = $zip.Entries | ForEach-Object { $_.FullName }
    $zip.Dispose()

    # Core files check
    $requiredEntries = @("model.xml", "component.dat", "component.rsc", "materials.dat", "component_icon_preview.tga", "layout_icon.tga")
    foreach ($req in $requiredEntries) {
        if ($entryNames -notcontains $req) {
            throw "Missing required entry '$req' in $comp"
        }
    }
    Write-Host "   [PASS] Core entries verified: $($requiredEntries -join ', ')" -ForegroundColor Green

    # Inspect model.xml namespace
    $zip = [System.IO.Compression.ZipFile]::OpenRead($repoFile)
    $xmlEntry = $zip.GetEntry("model.xml")
    $reader = New-Object System.IO.StreamReader($xmlEntry.Open())
    $xmlContent = $reader.ReadToEnd()
    $reader.Dispose()
    $zip.Dispose()

    if ($xmlContent -notmatch "schemas\.RProSoftDigital1\.com") {
        throw "Invalid XML namespace in model.xml for $comp"
    }
    Write-Host "   [PASS] XML namespace conforms to RProSoftDigital1 standard." -ForegroundColor Green

    # Inspect component.rsc header
    $zip = [System.IO.Compression.ZipFile]::OpenRead($repoFile)
    $rscEntry = $zip.GetEntry("component.rsc")
    $reader = New-Object System.IO.StreamReader($rscEntry.Open())
    $rscHeader = $reader.ReadLine()
    $reader.Dispose()
    $zip.Dispose()

    if ($rscHeader -notmatch "VCMD002804") {
        throw "Invalid binary/ASCII RSC header in $($comp): $rscHeader"
    }
    Write-Host "   [PASS] RSC magic signature verified: $rscHeader" -ForegroundColor Green
}

Write-Host ">>> Level 4 Component Package Integrity Tests PASSED (100%)! <<<`n" -ForegroundColor Green
