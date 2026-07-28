$ErrorActionPreference = 'Stop'

$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectFile = Join-Path $projectDir 'RealtimeService.Api.csproj'
$apiPort = 5101

# Stop only dotnet processes that were started for this project.
Get-CimInstance Win32_Process |
    Where-Object {
        ($_.Name -eq 'dotnet.exe' -or $_.Name -eq 'RealtimeService.Api.exe') -and
        $_.CommandLine -like "*$projectDir*"
    } |
    ForEach-Object {
        $processId = $_.ProcessId
        $processName = $_.Name

        try {
            Stop-Process -Id $processId -Force -ErrorAction Stop
            Write-Host ("Stopped PID {0} ({1})" -f $processId, $processName)
        }
        catch {
            Write-Warning ("Could not stop PID {0} ({1}): {2}" -f $processId, $processName, $_.Exception.Message)
        }
    }

# Also release the configured HTTP port if another process is listening.
$portPids = Get-NetTCPConnection -LocalPort $apiPort -State Listen -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique |
    Where-Object { $_ -is [int] -and $_ -gt 0 }

foreach ($portPid in $portPids) {
    try {
        $proc = Get-Process -Id $portPid -ErrorAction Stop
        Stop-Process -Id $portPid -Force -ErrorAction Stop
        Write-Host ("Stopped PID {0} ({1}) listening on port {2}" -f $portPid, $proc.ProcessName, $apiPort)
    }
    catch {
        Write-Warning ("Could not stop PID {0} on port {1}: {2}" -f $portPid, $apiPort, $_.Exception.Message)
    }
}

Set-Location $projectDir
dotnet run --project $projectFile
