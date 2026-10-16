[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidateSet('setup', 'verify', 'cleanup')][string]$Action,
    [Parameter(Mandatory = $true)][guid]$RunId,
    [Parameter(Mandatory = $true)][string]$OwnedInstallRoot,
    [Parameter(Mandatory = $true)][string]$FirewallManifest
)

$ErrorActionPreference = 'Stop'
if ($RunId -eq [guid]::Empty) { throw 'RunId must be a non-empty GUID.' }
$run = $RunId.ToString('D')
$root = [IO.Path]::GetFullPath($OwnedInstallRoot)
$manifestPath = [IO.Path]::GetFullPath($FirewallManifest)
$exe = Join-Path $root 'iisexpress.exe'
$original = Join-Path ${env:ProgramFiles} 'IIS Express\iisexpress.exe'
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())

function Get-Sha256([string]$Path) {
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try {
        $stream = [IO.File]::OpenRead($Path)
        try { return ([BitConverter]::ToString($algorithm.ComputeHash($stream))).Replace('-', '') }
        finally { $stream.Dispose() }
    }
    finally { $algorithm.Dispose() }
}
function Assert-Administrator {
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'FIREWALL_BLOCKED: run this exact operation in an elevated PowerShell session; no fallback or host launch is allowed.'
    }
}
function Get-RuleNames {
    @(
        "NemeStats-jq3-$run-IPv4-low",
        "NemeStats-jq3-$run-IPv4-high",
        "NemeStats-jq3-$run-IPv6-unspecified",
        "NemeStats-jq3-$run-IPv6-nonloopback"
    )
}
function Get-ExpectedAddresses {
    @(
        @('0.0.0.0-126.255.255.255'),
        @('128.0.0.0-255.255.255.255'),
        @('::/128'),
        @('::2-ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff')
    )
}
function Assert-OwnedManifest {
    if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) { throw "Owned firewall manifest is missing: $manifestPath" }
    $state = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    if ($state.runId -ne $run -or [IO.Path]::GetFullPath($state.executable) -ne $exe) { throw 'Firewall manifest run/executable identity mismatch.' }
    if ((Get-Sha256 $exe) -ne $state.executableSha256) { throw 'Owned executable hash changed; refusing to use recorded firewall ownership.' }
    $expectedNames = @(Get-RuleNames)
    if (@($state.ruleNames).Count -ne $expectedNames.Count -or (@($state.ruleNames) -join '|') -cne ($expectedNames -join '|')) {
        throw 'Firewall manifest rule-name set mismatch.'
    }
    $expectedAddresses = @(Get-ExpectedAddresses)
    if (@($state.remoteAddress).Count -ne $expectedAddresses.Count) { throw 'Firewall manifest remote-address set mismatch.' }
    for ($i = 0; $i -lt $expectedAddresses.Count; $i++) {
        Assert-AddressSet @($state.remoteAddress[$i]) $expectedAddresses[$i] $expectedNames[$i]
    }
    return $state
}
function Get-ObservedRule {
    param([string]$Name)
    $r = @(Get-NetFirewallRule -PolicyStore PersistentStore -Name $Name -ErrorAction Stop)
    if ($r.Count -ne 1) { throw "Expected exactly one firewall rule named '$Name'." }
    $app = $r[0] | Get-NetFirewallApplicationFilter
    $addr = $r[0] | Get-NetFirewallAddressFilter
    $port = $r[0] | Get-NetFirewallPortFilter
    $enabledType = $r[0].Enabled.GetType()
    $profileType = $r[0].Profile.GetType()
    $expectedProfile = [Enum]::Parse($profileType, 'Domain, Private, Public')
    if ($r[0].Direction -ne 'Outbound' -or $r[0].Action -ne 'Block' -or $r[0].Enabled -ne [Enum]::Parse($enabledType, 'True') -or $r[0].Profile -ne $expectedProfile -or $app.Program -ne $exe -or $port.Protocol -ne 'Any') {
        throw "Firewall rule '$Name' does not match the owned outbound executable block contract."
    }
    [pscustomobject]@{ Rule = $r[0]; RemoteAddress = @($addr.RemoteAddress) }
}
function Assert-AddressSet([string[]]$Actual, [string[]]$Expected, [string]$Name) {
    $actualCanonical = @($Actual | ForEach-Object {
        $address = $_
        if ($address -eq '::') { $address = '::/128' }
        $address
    } | Sort-Object)
    $expectedCanonical = @($Expected | ForEach-Object {
        $address = $_
        if ($address -eq '::') { $address = '::/128' }
        $address
    } | Sort-Object)
    if (($actualCanonical -join '|') -ne ($expectedCanonical -join '|')) {
        throw "Rule remote-address set mismatch: $Name"
    }
}

if ($Action -eq 'setup') {
    Assert-Administrator
    if (-not (Test-Path -LiteralPath $root -PathType Container)) { throw "Owned install root does not exist: $root" }
    if (-not (Test-Path -LiteralPath $exe -PathType Leaf) -or -not (Test-Path -LiteralPath $original -PathType Leaf)) { throw 'Owned and source IIS Express executables must both exist.' }
    if ($root.Equals((Split-Path $original -Parent), [StringComparison]::OrdinalIgnoreCase)) { throw 'The shared IIS Express installation cannot be used as the owned install.' }
    $ownedHash = Get-Sha256 $exe
    $sourceHash = Get-Sha256 $original
    if ($ownedHash -ne $sourceHash) { throw 'Owned IIS Express executable differs from the source installation executable.' }
    if (Test-Path -LiteralPath $manifestPath) { throw 'Firewall manifest already exists; refusing to adopt resources.' }
    $names = Get-RuleNames
    foreach ($name in $names) { if (Get-NetFirewallRule -PolicyStore PersistentStore -Name $name -ErrorAction SilentlyContinue) { throw "Rule name already exists and is not owned by this run: $name" } }
    $profiles = @(Get-NetFirewallProfile -PolicyStore ActiveStore)
    if (@($profiles | Where-Object { -not $_.Enabled }).Count -gt 0) { throw 'At least one active Windows Firewall profile is disabled.' }
    if ((Get-Service MpsSvc).Status -ne 'Running') { throw 'Windows Firewall service is not running.' }

    $addresses = Get-ExpectedAddresses
    $created = [System.Collections.Generic.List[string]]::new()
    try {
        for ($i = 0; $i -lt $names.Count; $i++) {
            New-NetFirewallRule -PolicyStore PersistentStore -Name $names[$i] -DisplayName $names[$i] -Direction Outbound -Action Block -Enabled True -Profile Domain,Private,Public -Program $exe -Protocol Any -RemoteAddress $addresses[$i] | Out-Null
            $created.Add($names[$i])
        }
        $state = [ordered]@{ schemaVersion = 1; runId = $run; executable = $exe; executableSha256 = $ownedHash; ruleNames = $names; remoteAddress = $addresses; createdUtc = [DateTime]::UtcNow.ToString('o') }
        $parent = Split-Path $manifestPath -Parent
        if (-not (Test-Path -LiteralPath $parent -PathType Container)) { throw "Manifest directory must already exist: $parent" }
        $state | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
        for ($i = 0; $i -lt $names.Count; $i++) {
            $observed = Get-ObservedRule $names[$i]
            Assert-AddressSet $observed.RemoteAddress $addresses[$i] $names[$i]
        }
        Write-Output 'FirewallContainment=verified'
        Write-Output "FirewallManifest=$manifestPath"
        Write-Output "OwnedExecutable=$exe"
    } catch {
        foreach ($name in $created) { Remove-NetFirewallRule -PolicyStore PersistentStore -Name $name -ErrorAction SilentlyContinue }
        if (Test-Path -LiteralPath $manifestPath) { Remove-Item -LiteralPath $manifestPath -Force }
        throw
    }
    return
}

if ($Action -eq 'cleanup') { Assert-Administrator }
$state = Assert-OwnedManifest
if ($Action -eq 'verify') {
    for ($i = 0; $i -lt $state.ruleNames.Count; $i++) {
        $observed = Get-ObservedRule $state.ruleNames[$i]
        Assert-AddressSet $observed.RemoteAddress $state.remoteAddress[$i] $state.ruleNames[$i]
    }
    Write-Output 'FirewallContainment=verified'
    Write-Output "OwnedExecutable=$exe"
    Write-Output "FirewallManifest=$manifestPath"
    return
}

foreach ($name in $state.ruleNames) { [void](Get-ObservedRule $name) }
foreach ($name in $state.ruleNames) { Remove-NetFirewallRule -PolicyStore PersistentStore -Name $name -ErrorAction Stop }
Remove-Item -LiteralPath $manifestPath -Force
Write-Output 'FirewallContainment=cleaned'
Write-Output "OwnedExecutable=$exe"
