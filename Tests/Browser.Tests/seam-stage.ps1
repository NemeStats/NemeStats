[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('stage','seed','check','guard','refresh','diagnostic','settle','configure')][string]$Phase,
    [Parameter(Mandatory)][guid]$RunId,
    [Parameter(Mandatory)][string]$EvidenceRoot,
    [ValidateSet('debug','optimized')][string]$Bundle = 'debug',
    [ValidateSet('app','published')][string]$Stage = 'app',
    [ValidateSet('debug-desktop','debug-mobile','optimized-desktop','optimized-mobile')][string]$FixtureCell
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$run = $RunId.ToString('D')
$root = [IO.Path]::GetFullPath($EvidenceRoot)
$base = [IO.Path]::GetFullPath((Join-Path $repoRoot '.omo\evidence\jquery-3-migration')) + '\'
if (-not $root.StartsWith($base, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unowned evidence root.' }
$script:runManifestPath = Join-Path $root ($run + '-run-manifest.json')
$script:manifest = Get-Content -LiteralPath $script:runManifestPath -Raw | ConvertFrom-Json -AsHashtable
$sid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
$acl = Get-Acl -LiteralPath $script:runManifestPath
if (-not $acl.AreAccessRulesProtected -or @($acl.Access | Where-Object {
    $_.AccessControlType -eq 'Allow' -and $_.IdentityReference.Translate([Security.Principal.SecurityIdentifier]).Value -cne $sid
}).Count) { throw 'Manifest access is not restricted to the owner.' }
$m = $script:manifest
if ($m.runId -cne $run -or $m.evidenceOwnerSid -cne $sid -or $m.repositoryRoot -cne $repoRoot -or
    $m.evidenceRoot -cne $root -or $m.stage -cne $Stage -or ($Phase -ne 'configure' -and $m.bundle -cne $Bundle) -or
    -not $m.preparationReady -or -not $m.migrationsApplied -or -not $m.firewallVerified -or
    $m.applicationStarted -or $m.cleanupComplete -or $m.database.ownerRunId -cne $run -or
    $m.stagedHost -cne (Join-Path $root ('staged-host\' + $run))) { throw 'Prepared manifest identity/readiness mismatch.' }
$ownerPath = if ($Stage -eq 'published') { Join-Path $root 'stage-owner.json' } else { Join-Path $m.stagedHost '.task1-owner.json' }
$owner = Get-Content -LiteralPath $ownerPath -Raw | ConvertFrom-Json
if ($owner.runId -cne $run -or $owner.stagedHost -cne $m.stagedHost) { throw 'Staged ownership mismatch.' }
. (Join-Path $repoRoot 'Tests\Browser.Support\PrivateLocalDb.ps1')
$infrastructure = Get-OwnedInfrastructureManifest
$fw = @(& (Join-Path $PSScriptRoot 'firewall-containment.ps1') -Action verify -RunId ([guid]$infrastructure.runId) `
    -OwnedInstallRoot ([IO.Path]::GetDirectoryName($m.ownedIisExpress)) -FirewallManifest $m.firewallManifest)
if ($fw -notcontains 'FirewallContainment=verified') { throw 'Active external containment not verified.' }
function Save-RunManifest {
    $script:manifest | ConvertTo-Json -Depth 15 | Set-Content -LiteralPath $script:runManifestPath -Encoding utf8
}
function Invoke-LocalSqlScalar(
    [string]$RawConnectionString, [string]$InitialCatalog, [string]$CommandText,
    [string]$ParameterName, [string]$ParameterValue
) {
    $builder = [System.Data.SqlClient.SqlConnectionStringBuilder]::new($RawConnectionString)
    $builder['Initial Catalog'] = $InitialCatalog
    $connection = [System.Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $CommandText
        $command.CommandTimeout = 30
        if ($ParameterName) {
            $parameter = $command.Parameters.Add($ParameterName, [System.Data.SqlDbType]::NVarChar, 128)
            $parameter.Value = $ParameterValue
        }
        return $command.ExecuteScalar()
    } finally { $connection.Dispose() }
}
$script:sqlLocalDbPath = $m.tools.sqlLocalDb.path
$script:effectiveDatabase = "Data Source=$($m.database.server);Initial Catalog=$($m.database.catalog);Integrated Security=True;MultipleActiveResultSets=True"
& (Join-Path $PSScriptRoot 'prepare.ps1') -Phase effective -RunId $RunId -EvidenceRoot $root -Bundle $Bundle -Stage $Stage
if ($LASTEXITCODE -ne 0) { throw 'Exact effective CloudConfigurationManager guard failed before bootstrap.' }
Start-OwnedPrivateInstance
Assert-OwnedPrivateSql
if (@(Get-CimInstance Win32_Process | Where-Object { $_.ExecutablePath -ceq $m.ownedIisExpress }).Count) {
    throw 'An owned IIS worker is still running; configuration cannot be changed beneath it.'
}
if ($Phase -eq 'settle') {
    $sqlPid = [int](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' "SELECT CONVERT(int,SERVERPROPERTY('ProcessID'))" '' '')
    $sqlProcess = [Diagnostics.Process]::GetProcessById($sqlPid)
    $sqlProcess.EnableRaisingEvents = $true
    & $script:sqlLocalDbPath stop $m.localDbInstance.name | Out-Null
    if ($LASTEXITCODE -ne 0 -or -not $sqlProcess.WaitForExit(30000)) { throw 'Exact owned SQL process did not stop.' }
    $m.localDbInstance.startedByRun = $false
    $m.localDbInstance.stopExitCode = 0
    Save-RunManifest
    Write-Output "Task2SqlSettled=stopped;processId=$sqlPid"
    exit 0
}
if ($Phase -eq 'guard') { Write-Output 'Task2Guard=verified'; exit 0 }
if ($Phase -eq 'configure') {
    if (-not $m.ContainsKey('qaPipeSecret')) { throw 'Bundle configuration requires the owned staged seam.' }
    [xml]$config = Get-Content -LiteralPath $m.stagedWebConfig -Raw
    $config.configuration.'system.web'.compilation.SetAttribute('debug', $(if ($Bundle -eq 'debug') { 'true' } else { 'false' }))
    $config.Save($m.stagedWebConfig)
    $m.bundle = $Bundle
    if ($FixtureCell) { $m.qaFixtureCell = $FixtureCell }
    $m.qaFixtureGeneration = if ($m.ContainsKey('qaFixtureGeneration')) { [int]$m.qaFixtureGeneration + 1 } else { 1 }
    if ($m.qaFixtureGeneration -gt 1000) { throw 'Owned fixture generation limit reached.' }
    Save-RunManifest
    Write-Output "Task3BundleConfigured=$Bundle;fresh-AppDomain-required"
    exit 0
}
if ($Phase -eq 'diagnostic') {
    [xml]$config = Get-Content -LiteralPath $m.stagedWebConfig -Raw
    $custom = $config.configuration.'system.web'.customErrors
    if ($null -eq $custom) {
        $custom = $config.CreateElement('customErrors')
        [void]$config.configuration.'system.web'.AppendChild($custom)
    }
    $custom.SetAttribute('mode','Off')
    $config.configuration.'system.webServer'.httpErrors.SetAttribute('existingResponse','PassThrough')
    $config.configuration.'system.webServer'.httpErrors.SetAttribute('errorMode','DetailedLocalOnly')
    # CommonRegistry constructs RollbarSharp before module Init. Its constructor
    # requires this distinct key, but this synthetic value cannot authenticate.
    # The task1-owned executable firewall remains enforced before bootstrap.
    $rollbar = @($config.configuration.appSettings.add | Where-Object { $_.key -ceq 'Rollbar.AccessToken' })
    if (-not $rollbar.Count) {
        $rollbar = $config.CreateElement('add')
        $rollbar.SetAttribute('key','Rollbar.AccessToken')
        [void]$config.configuration.appSettings.AppendChild($rollbar)
    } else { $rollbar = $rollbar[0] }
    $rollbar.SetAttribute('value','local-test-only-not-a-real-token')
    $config.Save($m.stagedWebConfig)
    Write-Output 'Task2Diagnostic=owned-config-only'
    exit 0
}
if ($Phase -eq 'refresh') {
    if (-not $m.ContainsKey('qaPipeSecret')) { throw 'Refresh requires an already-owned staged seam.' }
    & $m.tools.msbuild.path (Join-Path $repoRoot 'Tests\Browser.Support\NemeStats.Browser.Support.csproj') `
        /t:Build /p:Configuration=Release /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) { throw 'Support refresh build failed.' }
    Copy-Item -LiteralPath (Join-Path $repoRoot 'Tests\Browser.Support\bin\Release\NemeStats.Browser.Support.dll') `
        -Destination (Join-Path $m.stagedHost 'bin\NemeStats.Browser.Support.dll') -Force
    Copy-Item -LiteralPath (Join-Path $repoRoot 'Source\UI\Scripts\GameDefinition\createGameDefinitionPartial.js') `
        -Destination (Join-Path $m.stagedHost 'Scripts\GameDefinition\createGameDefinitionPartial.js') -Force
    [xml]$config = Get-Content -LiteralPath $m.stagedWebConfig -Raw
    $qaModules = @($config.configuration.'system.webServer'.modules.add | Where-Object {
        $_.name -ceq 'NemeStatsOwnedBrowserQa' -and
        $_.type -ceq 'NemeStats.Browser.Support.StagedQaModule, NemeStats.Browser.Support'
    })
    if ($qaModules.Count -ne 1) { throw 'Expected exactly one owned staged QA module.' }
    $qaModules[0].SetAttribute('preCondition', 'managedHandler')
    $config.Save($m.stagedWebConfig)
    Write-Output 'Task2Refresh=owned-assembly-only'
    exit 0
}

if ($Phase -eq 'stage') {
    & $m.tools.msbuild.path (Join-Path $repoRoot 'Tests\Browser.Support\NemeStats.Browser.Support.csproj') `
        /t:Build /p:Configuration=Release /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) { throw 'Support build failed.' }
    if ($m.ContainsKey('qaPipeSecret')) { throw 'QA seam is already staged; refusing to rotate ownership.' }
    $bytes = New-Object byte[] 32
    [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    $m.qaPipeSecret = [Convert]::ToBase64String($bytes)
    $m.qaPipeName = 'NemeStatsQa_' + $RunId.ToString('N')
    $m.qaFixturePassword = 'Qa9_' + [Convert]::ToBase64String($bytes).Replace('/','x').Replace('+','y')
    $m.qaStagedUtc = [DateTime]::UtcNow.ToString('o')
    $certificate = Get-Item -LiteralPath ('Cert:\LocalMachine\My\' + $m.certificate.thumbprint)
    $pem = "-----BEGIN CERTIFICATE-----`n" + [Convert]::ToBase64String(
        $certificate.RawData, [Base64FormattingOptions]::InsertLineBreaks) + "`n-----END CERTIFICATE-----`n"
    $m.qaCertificatePem = Join-Path $root 'task-2-owned-public-certificate.pem'
    [IO.File]::WriteAllText($m.qaCertificatePem, $pem)
    $m | ConvertTo-Json -Depth 15 | Set-Content -LiteralPath $script:runManifestPath -Encoding utf8
    Copy-Item -LiteralPath (Join-Path $repoRoot 'Tests\Browser.Support\bin\Release\NemeStats.Browser.Support.dll') `
        -Destination (Join-Path $m.stagedHost 'bin\NemeStats.Browser.Support.dll')
    [xml]$config = Get-Content -LiteralPath $m.stagedWebConfig -Raw
    $entry = $config.CreateElement('add')
    $entry.SetAttribute('key','BrowserQa.RunManifest')
    $entry.SetAttribute('value',$script:runManifestPath)
    [void]$config.configuration.appSettings.AppendChild($entry)
    $rollbars = @($config.configuration.appSettings.add | Where-Object key -CEQ 'Rollbar.AccessToken')
    if ($rollbars.Count -gt 1) { throw 'Duplicate copied Rollbar setting.' }
    if ($rollbars.Count) { $rollbar = $rollbars[0] } else {
        $rollbar = $config.CreateElement('add')
        $rollbar.SetAttribute('key','Rollbar.AccessToken')
        [void]$config.configuration.appSettings.AppendChild($rollbar)
    }
    $rollbar.SetAttribute('value','local-test-only-not-a-real-token')
    $module = $config.CreateElement('add')
    $module.SetAttribute('name','NemeStatsOwnedBrowserQa')
    $module.SetAttribute('type','NemeStats.Browser.Support.StagedQaModule, NemeStats.Browser.Support')
    $module.SetAttribute('preCondition','managedHandler')
    [void]$config.configuration.'system.webServer'.modules.AppendChild($module)
    $config.Save($m.stagedWebConfig)
    Write-Output 'Task2Stage=staged-before-bootstrap'
    exit 0
}
if (-not $m.ContainsKey('qaPipeSecret')) { throw 'QA seam must be staged before bootstrap.' }
if ($Phase -eq 'seed' -and -not $m.ContainsKey('fixtures')) {
    throw 'Task2 bootstrap creates fixtures under root overrides; run check before extending seed.'
}
if ($Phase -eq 'seed') {
    & node (Join-Path $PSScriptRoot 'run.mjs') --run-manifest $script:runManifestPath --mode baseline `
        --stage $Stage --bundle $Bundle --viewport desktop --evidence $root --seed-only
} else {
    & node (Join-Path $PSScriptRoot 'run.mjs') --run-manifest $script:runManifestPath --mode baseline `
        --stage $Stage --bundle $Bundle --viewport desktop --evidence $root --check-seam
}
exit $LASTEXITCODE
