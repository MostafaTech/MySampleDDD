. ".\_common.ps1"

#Remove-Service -Name $serviceName
$serviceToRemove = & Get-WmiObject `
    -Class Win32_Service `
    -Filter "name='$serviceName'"
$serviceToRemove.stopservice()

Write-Host "Waiting 5 seconds to give time service to stop..."
Start-Sleep -s 5
$serviceToRemove.delete()
Write-Host "Service removed."
