$serviceName = "ConsumerService"
$serviceDesc = "A testing service to consume rmq messages"
$serviceExecutableDir = Resolve-Path -Path "..\src\Consumer\bin\Debug\netcoreapp3.1"
$serviceExecutablePath = Join-Path $serviceExecutableDir "Consumer.exe"


# Source: https://stackoverflow.com/a/21333052/4023416
function ReinstallService ($serviceName, $binaryPath, $description, $login, $password, $startUpType)
{
    Write-Host "Trying to create service: $serviceName"

    #Check Parameters
    if ((Test-Path $binaryPath)-eq $false)
    {
        Write-Host "BinaryPath to service not found: $binaryPath"
        Write-Host "Service was NOT installed."
        return
    }

    if (("Automatic", "Manual", "Disabled") -notcontains $startUpType)
    {
        Write-Host "Value for startUpType parameter should be (Automatic or Manual or Disabled) and it was $startUpType"
        Write-Host "Service was NOT installed."
        return
    }

    # Verify if the service already exists, and if yes remove it first
    if (Get-Service $serviceName -ErrorAction SilentlyContinue)
    {
        # using WMI to remove Windows service because PowerShell does not have CmdLet for this
        $serviceToRemove = Get-WmiObject -Class Win32_Service -Filter "name='$serviceName'"

        $serviceToRemove.delete()
        Write-Host "Service removed: $serviceName"
    }

    # if password is empty, create a dummy one to allow have credentias for system accounts: 
    #NT AUTHORITY\LOCAL SERVICE
    #NT AUTHORITY\NETWORK SERVICE
    if ($password -eq "") 
    {
        #$secpassword = (new-object System.Security.SecureString)
        # Bug detected by @GaTechThomas
        $secpasswd = (new-object System.Security.SecureString)
    }
    else
    {
        $secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
    }
    $mycreds = New-Object System.Management.Automation.PSCredential ($login, $secpasswd)

    # Creating Windows Service using all provided parameters
    Write-Host "Installing service: $serviceName"
    New-Service -name $serviceName -binaryPathName $binaryPath -Description $description -displayName $serviceName -startupType $startUpType -credential $mycreds

    Write-Host "Installation completed: $serviceName"

    # Trying to start new service
    Write-Host "Trying to start new service: $serviceName"
    $serviceToStart = Get-WmiObject -Class Win32_Service -Filter "name='$serviceName'"
    $serviceToStart.startservice()
    Write-Host "Service started: $serviceName"

    #SmokeTest
    Write-Host "Waiting 5 seconds to give time service to start..."
    Start-Sleep -s 5
    $SmokeTestService = Get-Service -Name $serviceName
    if ($SmokeTestService.Status -ne "Running")
    {
        Write-Host "Smoke test: FAILED. (SERVICE FAILED TO START)"
        Throw "Smoke test: FAILED. (SERVICE FAILED TO START)"
    }
    else
    {
        Write-Host "Smoke test: OK."
    }
}

