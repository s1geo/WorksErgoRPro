$dllPath1 = "C:\Users\Jojo\Documents\Для work(s) ergo\Plugin.WorksErgoAPI_.dll"
$dllPath2 = "C:\Users\Jojo\Documents\Для work(s) ergo\Plugin.WorksErgoAPI.dll"

Write-Host "=== INSPECTING DLL 1: $dllPath1 ==="
try {
    $bytes = [System.IO.File]::ReadAllBytes($dllPath1)
    $asm = [System.Reflection.Assembly]::Load($bytes)
    Write-Host "Assembly Name: $($asm.FullName)"
    foreach ($t in $asm.GetTypes()) {
        Write-Host "  Type: $($t.FullName) [Base: $($t.BaseType)]"
        foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly')) {
            Write-Host "    Method: $($m.Name)"
        }
    }
} catch {
    Write-Host "Error DLL 1: $_"
}

Write-Host "`n=== INSPECTING DLL 2: $dllPath2 ==="
try {
    $bytes2 = [System.IO.File]::ReadAllBytes($dllPath2)
    $asm2 = [System.Reflection.Assembly]::Load($bytes2)
    Write-Host "Assembly Name: $($asm2.FullName)"
    $types = $asm2.GetTypes()
    Write-Host "Total types in DLL 2: $($types.Count)"
    foreach ($t in $types | Select-Object -First 30) {
        Write-Host "  Type: $($t.FullName)"
    }
} catch {
    Write-Host "Error DLL 2: $_"
}
