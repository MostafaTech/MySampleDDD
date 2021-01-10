. ".\_common.ps1"

& ReinstallService `
    $serviceName `
    $serviceExecutablePath `
    $serviceDesc `
    "NT AUTHORITY\LOCAL SERVICE" `
    "" `
    "Automatic"
