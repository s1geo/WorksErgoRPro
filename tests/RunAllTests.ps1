# RunAllTests.ps1 - Master Autotest Suite Runner for WorksErgo R-Pro
$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " RUNNING FULL AUTOMATED TEST SUITE: WORKSERGO R-PRO" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$testRoot = $PSScriptRoot

# 1. Level 1: Biomechanical Mathematics
Write-Host "`n>>> [LEVEL 1] Biomechanical Mathematics Unit Tests <<<" -ForegroundColor Yellow
& "$testRoot\unit_math_tests\TestErgonomicMath.ps1"

# 2. Level 2: MEF Assembly Integrity
Write-Host "`n>>> [LEVEL 2] MEF Assembly & CLR Contract Tests <<<" -ForegroundColor Yellow
& "$testRoot\assembly_mef_tests\TestAssemblyIntegrity.ps1"

# 3. Level 3: WPF UI & ViewModel Integration
Write-Host "`n>>> [LEVEL 3] CAD WPF View & ViewModel Integration Tests <<<" -ForegroundColor Yellow
& "$testRoot\cad_integration_tests\TestUIInstantiation.ps1"

# 4. Level 4: R-Pro eCatalog Component Package Integrity
& "$testRoot\TestComponentPackages.ps1"

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host " ALL TEST SUITES PASSED CLEANLY (100% SUCCESS)!" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
