[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidateSet('prepare', 'seed', 'stage', 'check', 'cleanup', 'effective')][string]$Phase,
    [Parameter(Mandatory = $true)][guid]$RunId,
    [Parameter(Mandatory = $true)][string]$EvidenceRoot,
    [ValidateSet('debug', 'optimized')][string]$Bundle = 'debug',
    [ValidateSet('app', 'published')][string]$Stage = 'app',
    [uri]$Origin
)

$ErrorActionPreference = 'Stop'
if ($Phase -eq 'effective') {
    $RunManifest = Join-Path ([IO.Path]::GetFullPath($EvidenceRoot)) ($RunId.ToString('D') + '-run-manifest.json')
    if ($PSVersionTable.PSEdition -ne 'Desktop') {
        & "$env:WINDIR\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -NonInteractive -File $PSCommandPath -Phase effective -RunId $RunId -EvidenceRoot $EvidenceRoot -Bundle $Bundle -Stage $Stage
        exit $LASTEXITCODE
    }
$m = Get-Content -LiteralPath $RunManifest -Raw | ConvertFrom-Json
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$root = [IO.Path]::GetFullPath($m.evidenceRoot)
$sid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
$acl = Get-Acl -LiteralPath $RunManifest
if ($m.repositoryRoot -cne $repo -or $m.evidenceOwnerSid -cne $sid -or
    -not $root.StartsWith((Join-Path $repo '.omo\evidence\jquery-3-migration') + '\', [StringComparison]::OrdinalIgnoreCase) -or
    [IO.Path]::GetFullPath($RunManifest) -cne (Join-Path $root ($m.runId + '-run-manifest.json')) -or
    -not $acl.AreAccessRulesProtected -or @($acl.Access | Where-Object {
        $_.AccessControlType -eq 'Allow' -and $_.IdentityReference.Translate([Security.Principal.SecurityIdentifier]).Value -cne $sid
    }).Count -or $m.applicationStarted -or $m.cleanupComplete -or
    $m.stagedWebConfig -cne (Join-Path $m.stagedHost 'Web.config')) {
    throw 'Exact effective-config guard refused an unowned or running stage.'
}
$source = @'
using System;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
public sealed class CloudConfigurationGuard : MarshalByRefObject {
    public string Verify(string support, string azure, string run, string expectedServer, string origin) {
        Assembly.LoadFrom(azure);
        var assembly = Assembly.LoadFrom(support);
        var type = assembly.GetType("NemeStats.Browser.Support.SafetyPreflight", true);
        var value = type.GetMethod("ValidateEffectiveConfiguration").Invoke(null, new object[] {run});
        if ((string)value.GetType().GetProperty("Server").GetValue(value, null) != expectedServer)
            throw new InvalidOperationException("Effective server mismatch.");
        var cloud = Assembly.LoadFrom(azure).GetType("Microsoft.Azure.CloudConfigurationManager", true)
            .GetMethod("GetSetting", new Type[] {typeof(string), typeof(bool)});
        var expected = new Dictionary<string,string> {
            {"googleAppId","local-browser-test"}, {"googleClientSecret","local-browser-test"},
            {"sendGridApiKey","local-browser-test"}, {"rollbarAccessToken",""},
            {"exceptionlessApiKey",""}, {"UniversalAnalytics.TrackingId",""},
            {"Rollbar.AccessToken","local-test-only-not-a-real-token"},
            {"urlRoot",origin}, {"emailConfirmationCallbackUrl",origin + "/Account/ConfirmEmail"}
        };
        foreach (var pair in expected)
            if ((string)cloud.Invoke(null, new object[] {pair.Key, false}) != pair.Value)
                throw new InvalidOperationException("Harmless effective setting mismatch: " + pair.Key);
        return (string)value.GetType().GetProperty("Fingerprint").GetValue(value, null);
    }
}
'@
$probePath = Join-Path $root ('cloud-guard-' + [guid]::NewGuid().ToString('N') + '.dll')
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Configuration -OutputAssembly $probePath
Add-Type -Path $probePath
$setup = New-Object AppDomainSetup
$setup.ApplicationBase = Join-Path $m.stagedHost 'bin'
$setup.ConfigurationFile = $m.stagedWebConfig
$domain = [AppDomain]::CreateDomain('ExactOwnedCloudConfiguration', $null, $setup)
try {
    $probe = $domain.CreateInstanceFromAndUnwrap($probePath, 'CloudConfigurationGuard')
    $fingerprint = $probe.Verify((Join-Path $repo 'Tests\Browser.Support\bin\Release\NemeStats.Browser.Support.dll'),
        (Join-Path $repo 'packages\Microsoft.WindowsAzure.ConfigurationManager.3.2.1\lib\net40\Microsoft.WindowsAzure.Configuration.dll'),
        $m.runId, $m.database.server, $m.httpsOrigin)
    [xml]$config = Get-Content -LiteralPath $m.stagedWebConfig -Raw
    if ($config.SelectNodes('/configuration/system.webServer/modules/add[@name="ExceptionlessModule"]').Count) {
        throw 'Startup telemetry module is not disabled in this owned copy.'
    }
    @{
        runId=$m.runId;stage=$m.stage;verifiedUtc=[DateTime]::UtcNow.ToString('o')
        exactWebConfig=$m.stagedWebConfig;webConfigSha256=(Get-FileHash $m.stagedWebConfig -Algorithm SHA256).Hash.ToLowerInvariant()
        fingerprint=$fingerprint;harmlessSecrets=$true;localCallbacks=$true;beforeBootstrap=$true
        method='Exact Web.config in a new .NET Framework AppDomain; actual CloudConfigurationManager.GetSetting(key,false)'
    } | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $root 'task-10-effective-config.json') -Encoding UTF8
    Write-Output 'ExactCloudConfigurationGuard=verified'
} finally { [AppDomain]::Unload($domain) }

    exit 0
}
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$evidenceRootFull = [IO.Path]::GetFullPath($EvidenceRoot)
$run = $RunId.ToString('D')
$evidenceBase = [IO.Path]::GetFullPath((Join-Path $repoRoot '.omo\evidence\jquery-3-migration'))
$evidencePrefix = $evidenceBase.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
$httpsPort = 51000 + ([Convert]::ToInt64($RunId.ToString('N').Substring(0, 8), 16) % 10000)
$expectedOrigin = 'https://localhost:{0}' -f $httpsPort

function Get-ToolVersion([string]$Path) {
    $item = Get-Item -LiteralPath $Path
    if ($item.VersionInfo.ProductVersion) { return $item.VersionInfo.ProductVersion }
    return $item.VersionInfo.FileVersion
}

function Assert-LocalHttpsOrigin([uri]$Value) {
    if (-not $Value.IsAbsoluteUri -or $Value.Scheme -cne 'https' -or -not $Value.IsLoopback -or
        $Value.DnsSafeHost -cne 'localhost' -or $Value.UserInfo -or $Value.Query -or
        $Value.Fragment -or $Value.AbsolutePath -ne '/') {
        throw 'Origin must use the app-supported HTTPS localhost host without credentials, path, query, or fragment.'
    }
}

function Set-AppSetting([xml]$Config, [string]$Key, [string]$Value) {
    $settings = $Config.configuration.appSettings
    $existing = @($settings.add | Where-Object { $_.key -ceq $Key })
    if ($existing.Count -gt 1) { throw "Duplicate appSetting '$Key' in copied host configuration." }
    if ($existing.Count -eq 1) {
        $existing[0].SetAttribute('value', $Value)
    }
    else {
        $entry = $Config.CreateElement('add')
        $entry.SetAttribute('key', $Key)
        $entry.SetAttribute('value', $Value)
        [void]$settings.AppendChild($entry)
    }
}

function Set-RunFileAcl([string]$Path) {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $acl = New-Object Security.AccessControl.FileSecurity
    $acl.SetAccessRuleProtection($true, $false)
    $acl.SetOwner($identity.User)
    $rule = [Security.AccessControl.FileSystemAccessRule]::new(
        $identity.User,
        [Security.AccessControl.FileSystemRights]::FullControl,
        [Security.AccessControl.AccessControlType]::Allow
    )
    [void]$acl.AddAccessRule($rule)
    [IO.FileSystemAclExtensions]::SetAccessControl([IO.FileInfo]::new($Path), $acl)
}

function Invoke-NetshRead([string[]]$Arguments) {
    $output = @(& $script:netsh @Arguments 2>&1)
    $code = $LASTEXITCODE
    [pscustomobject]@{
        ExitCode = $code
        Text = ($output | Out-String)
    }
}

function Assert-FreeHttpsReservation {
    $listener = @(Get-NetTCPConnection -LocalPort $script:httpsPort -State Listen -ErrorAction SilentlyContinue)
    if ($listener.Count -gt 0) { throw "The run-owned HTTPS port is already listening: $script:httpsPort" }

    foreach ($ipPort in @(('0.0.0.0:{0}' -f $script:httpsPort), ('[::]:{0}' -f $script:httpsPort))) {
        $binding = Invoke-NetshRead @('http', 'show', 'sslcert', "ipport=$ipPort")
        if ($binding.Text -match 'Certificate Hash') {
            throw "An existing HTTP.sys TLS binding occupies the run-owned endpoint $ipPort."
        }
        if ($binding.ExitCode -ne 1 -or $binding.Text -notmatch 'cannot find the file specified') {
            throw "Could not prove the HTTP.sys TLS endpoint is unused: $ipPort"
        }
    }

    $reservation = Invoke-NetshRead @('http', 'show', 'urlacl', "url=$script:httpsUrl")
    if ($reservation.ExitCode -ne 0 -or $reservation.Text -match 'Reserved URL') {
        throw "An existing HTTP.sys URL reservation occupies the run-owned URL: $script:httpsUrl"
    }
}

function Assert-Administrator {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'TASK1_BLOCKED: certificate store, HTTP.sys binding and URL reservation require an elevated PowerShell token; no resource was changed.'
    }
}

function Save-RunManifest {
    $script:manifest | ConvertTo-Json -Depth 12 |
        Set-Content -LiteralPath $script:runManifestPath -Encoding UTF8
    Set-RunFileAcl $script:runManifestPath
}

function Get-LocalDbState([string]$InstanceName) {
    $output = @(& $script:sqlLocalDbPath info $InstanceName 2>&1)
    if ($LASTEXITCODE -ne 0) { throw "Could not inspect the explicit LocalDB instance '$InstanceName'." }
    $text = $output -join [Environment]::NewLine
    $match = [Regex]::Match($text, '(?im)^\s*State:\s*(Stopped|Running)\s*$')
    if (-not $match.Success) {
        throw "LocalDB did not report a recognized state for '$InstanceName'."
    }
    $version = [Regex]::Match($text, '(?im)^\s*Version:\s*(?<version>[^\r\n]+)')
    if (-not $version.Success) { throw "LocalDB did not report its engine version for '$InstanceName'." }
    return [pscustomobject]@{
        State = $match.Groups[1].Value
        Version = $version.Groups['version'].Value.Trim()
    }
}

function Invoke-LocalSqlScalar(
    [string]$RawConnectionString,
    [string]$InitialCatalog,
    [string]$CommandText,
    [string]$ParameterName,
    [string]$ParameterValue
) {
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($RawConnectionString)
    $builder['Initial Catalog'] = $InitialCatalog
    $connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $CommandText
        $command.CommandTimeout = 30
        if (-not [String]::IsNullOrEmpty($ParameterName)) {
            $parameter = $command.Parameters.Add($ParameterName, [System.Data.SqlDbType]::NVarChar, 128)
            $parameter.Value = $ParameterValue
        }
        return $command.ExecuteScalar()
    }
    finally {
        if ($connection.State -ne [System.Data.ConnectionState]::Closed) { $connection.Close() }
        $connection.Dispose()
    }
}

function New-OwnedApplicationHostConfig(
    [string]$TemplatePath,
    [string]$DestinationPath,
    [string]$PhysicalPath,
    [string]$SiteName,
    [int]$Port
) {
    Copy-Item -LiteralPath $TemplatePath -Destination $DestinationPath
    [xml]$config = Get-Content -LiteralPath $DestinationPath -Raw
    $sites = $config.SelectSingleNode('/configuration/system.applicationHost/sites')
    if (-not $sites) { throw 'The IIS Express template has no system.applicationHost/sites section.' }
    if ($config.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name='$SiteName']")) {
        throw 'The owned IIS site name already exists in the fresh per-run template.'
    }

    $siteIds = @($sites.SelectNodes('site') | ForEach-Object { [int]$_.GetAttribute('id') })
    $newId = 1
    if ($siteIds.Count -gt 0) { $newId = ($siteIds | Measure-Object -Maximum).Maximum + 1 }
    $site = $config.CreateElement('site')
    $site.SetAttribute('name', $SiteName)
    $site.SetAttribute('id', $newId.ToString([Globalization.CultureInfo]::InvariantCulture))
    $site.SetAttribute('serverAutoStart', 'false')

    $application = $config.CreateElement('application')
    $application.SetAttribute('path', '/')
    $application.SetAttribute('applicationPool', 'Clr4IntegratedAppPool')
    $virtualDirectory = $config.CreateElement('virtualDirectory')
    $virtualDirectory.SetAttribute('path', '/')
    $virtualDirectory.SetAttribute('physicalPath', $PhysicalPath)
    [void]$application.AppendChild($virtualDirectory)
    [void]$site.AppendChild($application)

    $bindings = $config.CreateElement('bindings')
    $binding = $config.CreateElement('binding')
    $binding.SetAttribute('protocol', 'https')
    $binding.SetAttribute('bindingInformation', ('*:{0}:localhost' -f $Port))
    $binding.SetAttribute('sslFlags', '0')
    [void]$bindings.AppendChild($binding)
    [void]$site.AppendChild($bindings)

    $siteDefaults = $sites.SelectSingleNode('siteDefaults')
    if ($siteDefaults) { [void]$sites.InsertBefore($site, $siteDefaults) }
    else { [void]$sites.AppendChild($site) }
    $config.Save($DestinationPath)

    [xml]$verified = Get-Content -LiteralPath $DestinationPath -Raw
    $writtenSite = $verified.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name='$SiteName']")
    if (-not $writtenSite -or $writtenSite.GetAttribute('serverAutoStart') -ne 'false' -or
        $writtenSite.SelectSingleNode("application/virtualDirectory[@path='/']").GetAttribute('physicalPath') -cne $PhysicalPath -or
        $writtenSite.SelectSingleNode("bindings/binding[@protocol='https']").GetAttribute('bindingInformation') -cne ('*:{0}:localhost' -f $Port)) {
        throw 'The staged IIS site binding failed its exact owned-host configuration check.'
    }
    return [pscustomobject]@{ SiteId = $newId; SiteName = $SiteName }
}

function New-OwnedHttpsCertificateAndBindings([switch]$ResumeOwnedCertificate) {
    $certPath = 'Cert:\LocalMachine\My'
    $friendly = $script:manifest.certificate.friendlyName
    $existing = @(Get-ChildItem -LiteralPath $certPath | Where-Object { $_.FriendlyName -ceq $friendly })
    if ($ResumeOwnedCertificate) {
        if (-not $script:manifest.certificate.createRequested -or -not $script:manifest.certificate.created -or
            $existing.Count -ne 1 -or $existing[0].Thumbprint -cne $script:manifest.certificate.thumbprint) {
            throw 'Certificate resume refused because exact prior run creation and thumbprint are not proven.'
        }
        $certificate = $existing[0]
    }
    else {
        if ($existing.Count -gt 0) { throw 'The run-owned certificate friendly name already exists; refusing to adopt it.' }
        $script:manifest.certificate.createRequested = $true
        Save-RunManifest
        $certificate = New-SelfSignedCertificate `
            -Type SSLServerAuthentication `
            -Subject $script:manifest.certificate.subject `
            -DnsName 'localhost' `
            -FriendlyName $friendly `
            -CertStoreLocation $certPath `
            -KeyAlgorithm RSA `
            -KeyLength 2048 `
            -HashAlgorithm SHA256 `
            -KeyUsage DigitalSignature, KeyEncipherment `
            -NotAfter ([DateTime]::Now.AddDays(30))
    }
    if (-not $certificate -or $certificate.Subject -ne $script:manifest.certificate.subject) {
        throw 'Windows did not create the expected run-owned localhost server certificate.'
    }
    $subjectAltName = @($certificate.Extensions | Where-Object { $_.Oid.Value -eq '2.5.29.17' })
    if ($subjectAltName.Count -ne 1 -or $subjectAltName[0].Format($false) -notmatch 'DNS Name=localhost' -or
        $certificate.NotAfter -le [DateTime]::Now) {
        throw 'The generated certificate does not cover a currently valid localhost TLS endpoint.'
    }
    $script:manifest.certificate.created = $true
    $script:manifest.certificate.thumbprint = $certificate.Thumbprint
    if (-not $ResumeOwnedCertificate) { $script:manifest.certificate.createdUtc = [DateTime]::UtcNow.ToString('o') }
    Save-RunManifest

    $publicCertificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new([byte[]]$certificate.RawData)
    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store(
        [System.Security.Cryptography.X509Certificates.StoreName]::Root,
        [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser
    )
    try {
        $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
        if (@($rootStore.Certificates | Where-Object { $_.Thumbprint -eq $certificate.Thumbprint }).Count -gt 0) {
            throw 'The generated thumbprint already exists in the current-user trust store.'
        }
        $rootStore.Add($publicCertificate)
        if (@($rootStore.Certificates | Where-Object { $_.Thumbprint -eq $certificate.Thumbprint }).Count -ne 1) {
            throw 'The generated certificate was not present in the current-user trust store after import.'
        }
    }
    finally { $rootStore.Close() }
    $script:manifest.certificate.trustedForCurrentUser = $true
    Save-RunManifest

    $userName = [Security.Principal.WindowsIdentity]::GetCurrent().Name
    $url = $script:manifest.urlReservation.url
    $script:manifest.urlReservation.createRequested = $true
    Save-RunManifest
    & $script:netsh http add urlacl "url=$url" "user=$userName" listen=yes delegate=no | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'HTTP.sys rejected the run-owned HTTPS URL reservation.' }
    $script:manifest.urlReservation.created = $true
    $script:manifest.urlReservation.userName = $userName
    Save-RunManifest

    foreach ($binding in $script:manifest.tlsBindings) {
        $binding.createRequested = $true
        Save-RunManifest
        & $script:netsh http add sslcert "ipport=$($binding.ipPort)" "certhash=$($certificate.Thumbprint)" `
            "appid=$($script:manifest.certificate.appId)" "certstorename=$($script:manifest.certificate.storeName)" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "HTTP.sys rejected the owned TLS binding $($binding.ipPort)." }
        $binding.created = $true
        $binding.thumbprint = $certificate.Thumbprint
        Save-RunManifest

        $observed = Invoke-NetshRead @('http', 'show', 'sslcert', "ipport=$($binding.ipPort)")
        if ($observed.ExitCode -ne 0 -or
            $observed.Text -notmatch [Regex]::Escape($certificate.Thumbprint) -or
            $observed.Text -notmatch [Regex]::Escape($script:manifest.certificate.appId) -or
            $observed.Text -notmatch 'Certificate Store Name\s*:\s*MY') {
            throw "The installed HTTP.sys binding failed exact certificate/app-id verification: $($binding.ipPort)"
        }
    }

    $urlState = Invoke-NetshRead @('http', 'show', 'urlacl', "url=$url")
    if ($urlState.ExitCode -ne 0 -or $urlState.Text -notmatch [Regex]::Escape($userName)) {
        throw 'The installed HTTP.sys URL reservation failed exact account verification.'
    }
    $script:manifest.urlReservation.verified = $true
    $script:manifest.certificate.verified = $true
    $script:manifest.httpsBindingPrepared = $true
    Save-RunManifest
}

function Invoke-Ef6Migrations {
    if ($script:manifest.localDbInstance.ownerRunId) { Assert-OwnedPrivateSql }
    $logPath = $script:manifest.database.migrationLog
    $script:manifest.database.migrationRequested = $true
    Save-RunManifest

    $arguments = @(
        [IO.Path]::GetFileName($script:manifest.database.migrationAssembly),
        "/startUpConfigurationFile=$($script:manifest.stagedWebConfig)",
        "/startUpDirectory=$($script:manifest.database.migrationStartupDirectory)",
        "/connectionString=$($script:effectiveDatabase)",
        '/connectionProviderName=System.Data.SqlClient',
        '/verbose'
    )
    $output = @(& $script:migrate @arguments 2>&1)
    $exitCode = $LASTEXITCODE
    $text = ($output | ForEach-Object { $_.ToString() }) -join [Environment]::NewLine
    if ($script:effectiveDatabase) { $text = $text.Replace($script:effectiveDatabase, '<redacted-connection-string>') }
    @(
        "RunId=$($script:manifest.runId)"
        "MigratePath=$($script:manifest.tools.migrate.path)"
        "MigrateVersion=$($script:manifest.tools.migrate.version)"
        "Assembly=$($script:manifest.database.migrationAssembly)"
        "StartupConfiguration=$($script:manifest.stagedWebConfig)"
        "StartupDirectory=$($script:manifest.database.migrationStartupDirectory)"
        "ConnectionProviderName=System.Data.SqlClient"
        "ConnectionTarget=Server=$($script:manifest.database.server);Catalog=$($script:manifest.database.catalog)"
        'FlagsUsed=/startUpConfigurationFile /startUpDirectory /connectionString /connectionProviderName /verbose'
        "MigrationExitCode=$exitCode"
        'MigrationOutputBegin'
        $text
        'MigrationOutputEnd'
    ) | Set-Content -LiteralPath $logPath -Encoding UTF8
    $script:manifest.database.migrationExitCode = $exitCode
    $script:manifest.database.migrationCompletedUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
    if ($exitCode -ne 0) { throw "EF6 migration failed with exit code $exitCode; see $logPath." }

    $migrationCount = Invoke-LocalSqlScalar $script:effectiveDatabase $script:manifest.database.catalog `
        'SELECT COUNT(*) FROM dbo.__MigrationHistory' '' ''
    $latestMigration = Invoke-LocalSqlScalar $script:effectiveDatabase $script:manifest.database.catalog `
        'SELECT MAX(MigrationId) FROM dbo.__MigrationHistory' '' ''
    $expectedMigrations = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot 'Source\BusinessLogic\Migrations') -Filter '*.cs' |
        Where-Object { $_.BaseName -match '^\d{14,17}_' -and $_.BaseName -notlike '*.Designer' } |
        Sort-Object BaseName)
    if ([int]$migrationCount -ne $expectedMigrations.Count -or
        [string]$latestMigration -cne $expectedMigrations[-1].BaseName) {
        throw 'EF6 returned success but the owned catalog does not contain the complete expected migration history.'
    }
    $script:manifest.database.migrationsApplied = $true
    $script:manifest.database.migrationCount = [int]$migrationCount
    $script:manifest.database.latestMigration = [string]$latestMigration
    Save-RunManifest
}

function Remove-RunOwnedResources([object]$State) {
    $privateOwned = [bool]$State.localDbInstance.ownerRunId
    if ($privateOwned) {
        Remove-OwnedPrivateLocalDb
        if ($null -ne $State.retiredDatabases) { Remove-RetiredOwnedCatalogs }
    }
    if ($State.database.created -and -not $State.database.createRequested) {
        throw 'Database cleanup refused because the manifest has a created catalog without a recorded create intent.'
    }
    if (-not $privateOwned -and $State.database.createRequested -and $State.database.catalogExistedBefore -eq $false) {
        $catalog = [string]$State.database.catalog
        if ($catalog -notmatch '^NemeStatsJQueryMigration_[0-9a-f]{32}$' -or
            $State.database.ownerRunId -ne $State.runId -or
            $State.database.catalogExistedBefore -ne $false) {
            throw 'Database cleanup refused because the manifest does not prove fresh run ownership.'
        }
        $databaseId = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' 'SELECT DB_ID(@catalog)' '@catalog' $catalog
        if ($null -ne $databaseId -and $databaseId -isnot [DBNull]) {
            $marker = Invoke-LocalSqlScalar $script:effectiveDatabase $catalog `
                "SELECT CONVERT(nvarchar(36), value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRunId'" '' ''
            if ([string]$marker -cne [string]$State.runId) {
                throw 'Database cleanup refused because its ownership marker does not match this run.'
            }
            [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
                "ALTER DATABASE [$catalog] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$catalog]" '' '')
        }
        $State.database.created = $false
        $State.database.droppedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    $expectedExe = [IO.Path]::GetFullPath($State.ownedIisExpress)
    $liveOwnedProcesses = @(Get-CimInstance Win32_Process | Where-Object {
        $_.ExecutablePath -and [IO.Path]::GetFullPath($_.ExecutablePath) -ceq $expectedExe
    })
    foreach ($process in $liveOwnedProcesses) {
        if (-not $State.applicationProcess -or
            [int]$State.applicationProcess.processId -ne [int]$process.ProcessId -or
            [IO.Path]::GetFullPath($State.applicationProcess.executable) -cne $expectedExe) {
            throw 'Host cleanup refused because a matching process has no matching manifest-owned process record.'
        }
        Stop-Process -Id $process.ProcessId -Force
        Wait-Process -Id $process.ProcessId -Timeout 30 -ErrorAction SilentlyContinue
        if (Get-Process -Id $process.ProcessId -ErrorAction SilentlyContinue) {
            throw "The run-owned IIS Express process did not exit: $($process.ProcessId)"
        }
        $State.applicationProcess.stoppedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    foreach ($binding in $State.tlsBindings) {
        if (-not ($binding.createRequested -or $binding.created)) { continue }
        $observed = Invoke-NetshRead @('http', 'show', 'sslcert', "ipport=$($binding.ipPort)")
        if ($observed.Text -match 'Certificate Hash') {
            if ($observed.Text -notmatch [Regex]::Escape([string]$State.certificate.thumbprint) -or
                $observed.Text -notmatch [Regex]::Escape([string]$State.certificate.appId)) {
                throw "TLS cleanup refused because the binding no longer matches the manifest: $($binding.ipPort)"
            }
            & $script:netsh http delete sslcert "ipport=$($binding.ipPort)" | Out-Null
            if ($LASTEXITCODE -ne 0) { throw "Could not remove the manifest-owned TLS binding $($binding.ipPort)." }
        }
        elseif ($observed.ExitCode -ne 1 -or $observed.Text -notmatch 'cannot find the file specified') {
            throw "TLS cleanup could not prove the binding absent or run-owned: $($binding.ipPort)"
        }
        $binding.created = $false
        $binding.removedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    if ($State.urlReservation.createRequested -or $State.urlReservation.created) {
        $reservation = Invoke-NetshRead @('http', 'show', 'urlacl', "url=$($State.urlReservation.url)")
        if ($reservation.Text -match 'Reserved URL') {
            if (-not $State.urlReservation.created -or $reservation.ExitCode -ne 0 -or
                $reservation.Text -notmatch [Regex]::Escape([string]$State.urlReservation.userName)) {
                throw 'URL reservation cleanup refused because successful run creation or the owner does not match the manifest.'
            }
            & $script:netsh http delete urlacl "url=$($State.urlReservation.url)" | Out-Null
            if ($LASTEXITCODE -ne 0) { throw 'Could not remove the manifest-owned HTTP.sys URL reservation.' }
        }
        elseif ($reservation.ExitCode -ne 0) {
            throw 'URL reservation cleanup could not prove the run-owned reservation absent.'
        }
        $State.urlReservation.created = $false
        $State.urlReservation.removedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    if ($State.certificate.createRequested -or $State.certificate.created) {
        $machineStore = New-Object System.Security.Cryptography.X509Certificates.X509Store(
            [System.Security.Cryptography.X509Certificates.StoreName]::My,
            [System.Security.Cryptography.X509Certificates.StoreLocation]::LocalMachine
        )
        $ownedThumbprint = [string]$State.certificate.thumbprint
        try {
            $machineStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
            $owned = @($machineStore.Certificates | Where-Object {
                ($ownedThumbprint -eq '' -or $_.Thumbprint -eq $ownedThumbprint) -and
                $_.Subject -eq $State.certificate.subject -and
                $_.FriendlyName -eq $State.certificate.friendlyName
            })
            if ($owned.Count -gt 1) { throw 'Certificate cleanup refused because the run-owned friendly name is ambiguous.' }
            if ($owned.Count -eq 1) { $ownedThumbprint = $owned[0].Thumbprint }
            foreach ($certificate in $owned) { $machineStore.Remove($certificate) }
        }
        finally { $machineStore.Close() }
        $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store(
            [System.Security.Cryptography.X509Certificates.StoreName]::Root,
            [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser
        )
        try {
            $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
            $trusted = @($rootStore.Certificates | Where-Object {
                $ownedThumbprint -ne '' -and
                $_.Thumbprint -eq $ownedThumbprint -and $_.Subject -eq $State.certificate.subject
            })
            foreach ($certificate in $trusted) { $rootStore.Remove($certificate) }
        }
        finally { $rootStore.Close() }
        $State.certificate.thumbprint = $ownedThumbprint
        $State.certificate.created = $false
        $State.certificate.trustedForCurrentUser = $false
        $State.certificate.removedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    $localDbStartOwned = $State.localDbInstance.startedByRun -or
        ($State.localDbInstance.startRequested -and $State.localDbInstance.stateBefore -eq 'Stopped')
    if ($localDbStartOwned -and -not $privateOwned) {
        $stateName = Get-LocalDbState ([string]$State.localDbInstance.name)
        if ($stateName.State -eq 'Running') {
            & $script:sqlLocalDbPath stop ([string]$State.localDbInstance.name) -i | Out-Null
            if ($LASTEXITCODE -ne 0) { throw 'Could not stop the LocalDB instance started by this run.' }
        }
        $State.localDbInstance.stoppedAfterMigration = $true
        $State.localDbInstance.stoppedAfterCleanup = $true
        $script:manifest = $State
        Save-RunManifest
    }

    if ($State.applicationHost.createRequested -or $State.applicationHost.created) {
        $hostFile = [string]$State.applicationHost.configPath
        if (Test-Path -LiteralPath $hostFile -PathType Leaf) {
            [xml]$hostConfig = Get-Content -LiteralPath $hostFile -Raw
            $site = $hostConfig.SelectSingleNode(
                "/configuration/system.applicationHost/sites/site[@name='$($State.applicationHost.siteName)']")
            if ($site -and (
                $site.GetAttribute('serverAutoStart') -ne 'false' -or
                $site.SelectSingleNode("application/virtualDirectory[@path='/']").GetAttribute('physicalPath') -cne $State.applicationHost.physicalPath -or
                $site.SelectSingleNode("bindings/binding[@protocol='https']").GetAttribute('bindingInformation') -cne $State.applicationHost.binding
            )) {
                throw 'ApplicationHost cleanup refused because the configuration is not the recorded disabled site.'
            }
            Remove-Item -LiteralPath $hostFile -Force
        }
        elseif ($State.applicationHost.created) {
            throw 'ApplicationHost cleanup refused because the recorded owned config file is missing.'
        }
        $State.applicationHost.created = $false
        $State.applicationHost.removedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    if (Test-Path -LiteralPath $State.stagedHost -PathType Container) {
        $ownerPath = Join-Path ([string]$State.stagedHost) '.task1-owner.json'
        if (Test-Path -LiteralPath $ownerPath -PathType Leaf) {
            $owner = Get-Content -LiteralPath $ownerPath -Raw | ConvertFrom-Json
            if ($owner.runId -ne $State.runId -or [IO.Path]::GetFullPath($owner.stagedHost) -cne
                [IO.Path]::GetFullPath($State.stagedHost)) {
                throw 'Staged host cleanup refused because its owner record does not match this run.'
            }
        }
        elseif (-not $State.stagedHostCreateRequested) {
            throw 'Staged host cleanup refused because there is no owner marker or create intent.'
        }
        Remove-Item -LiteralPath $State.stagedHost -Recurse -Force
        $State.stagedHostRemovedUtc = [DateTime]::UtcNow.ToString('o')
        $State.iisProfile.prepared = $false
        $State.iisProfile.removedUtc = $State.stagedHostRemovedUtc
        $script:manifest = $State
        Save-RunManifest
    }

    if (Test-Path -LiteralPath $script:firewallManifest -PathType Leaf) {
        $firewallOutput = @(& $script:firewallScript -Action cleanup -RunId $RunId `
            -OwnedInstallRoot $script:ownedInstallRoot -FirewallManifest $script:firewallManifest)
        if ($firewallOutput -notcontains 'FirewallContainment=cleaned') {
            throw 'Firewall cleanup did not return its owned-cleanup sentinel.'
        }
        $State.firewallVerified = $false
        $State.firewallRulesRemovedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest = $State
        Save-RunManifest
    }

    $State.cleanupComplete = $true
    $State.cleanupCompletedUtc = [DateTime]::UtcNow.ToString('o')
    $State.resourceSetup = 'cleaned'
    $State.preparationReady = $false
    $script:manifest = $State
    Save-RunManifest
}

if ($RunId -eq [guid]::Empty) { throw 'RunId must be a non-empty GUID.' }
if (-not [IO.Path]::IsPathRooted($EvidenceRoot)) { throw 'EvidenceRoot must be an absolute path.' }
if (-not $evidenceRootFull.StartsWith($evidencePrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "EvidenceRoot must be a child of the owned evidence directory: $evidenceBase"
}
if (Test-Path -LiteralPath $evidenceRootFull) {
    $existingRoot = Get-Item -LiteralPath $evidenceRootFull -Force
    if (($existingRoot.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw 'EvidenceRoot must not be a reparse point or linked directory.'
    }
}
if ($null -eq $Origin) { $Origin = [uri]$expectedOrigin }
Assert-LocalHttpsOrigin $Origin
if ($Origin.GetLeftPart([uriPartial]::Authority) -cne $expectedOrigin) {
    throw "Origin must be the deterministic run-owned endpoint $expectedOrigin."
}
if (-not (Test-Path -LiteralPath $repoRoot -PathType Container)) { throw "Repository root is missing: $repoRoot" }
if ($Phase -notin @('prepare', 'cleanup')) {
    & (Join-Path $PSScriptRoot 'seam-stage.ps1') -Phase $Phase -RunId $RunId `
        -EvidenceRoot $evidenceRootFull -Bundle $Bundle -Stage $Stage
    if ($LASTEXITCODE -ne 0) { throw "Task2 phase '$Phase' failed." }
    return
}
if ($Phase -eq 'prepare' -and $Stage -ne 'app') {
    throw 'Task 1 preparation accepts only the source app; published-host preparation belongs to task 10.'
}

$ownedInstallRoot = Join-Path $evidenceRootFull (Join-Path 'owned-host' (Join-Path $run 'IIS Express'))
$firewallManifest = Join-Path $evidenceRootFull ("firewall-" + $run + '.json')
$firewallScript = Join-Path $PSScriptRoot 'firewall-containment.ps1'
$stagedHost = Join-Path $evidenceRootFull (Join-Path 'staged-host' $run)
$runManifestPath = Join-Path $evidenceRootFull ($run + '-run-manifest.json')
$netsh = Join-Path $env:WINDIR 'System32\netsh.exe'
$script:ownedInstallRoot = $ownedInstallRoot
$script:firewallManifest = $firewallManifest
$script:firewallScript = $firewallScript
$script:stagedHost = $stagedHost
$script:runManifestPath = $runManifestPath
$script:netsh = $netsh
. (Join-Path $repoRoot 'Tests\Browser.Support\PrivateLocalDb.ps1')

if ($Phase -eq 'prepare' -and (Test-Path -LiteralPath $runManifestPath -PathType Leaf)) {
    Resume-OwnedPrivatePreparation
    return
}

if ($Phase -eq 'cleanup') {
    if (-not (Test-Path -LiteralPath $runManifestPath -PathType Leaf)) {
        throw "Cleanup requires the exact run-owned manifest: $runManifestPath"
    }
    $state = Get-Content -LiteralPath $runManifestPath -Raw | ConvertFrom-Json -AsHashtable
    if ($state.runId -ne $run -or [IO.Path]::GetFullPath($state.evidenceRoot) -cne $evidenceRootFull -or
        [IO.Path]::GetFullPath($state.stagedHost) -cne $stagedHost -or
        $state.database.ownerRunId -ne $run -or $state.httpsOrigin -cne $expectedOrigin -or
        [int]$state.httpsPort -ne $httpsPort -or
        $state.database.catalog -notmatch ('^NemeStatsJQueryMigration_' + $RunId.ToString('N') + '$')) {
        throw 'Cleanup refused because manifest identity or resource ownership does not match this invocation.'
    }
    $expectedOwnedExe = Join-Path $ownedInstallRoot 'iisexpress.exe'
    $expectedHostConfig = Join-Path $stagedHost 'applicationhost.config'
    $expectedTemplate = Join-Path $ownedInstallRoot 'config\templates\PersonalWebServer\applicationhost.config'
    $expectedUrl = $expectedOrigin + '/'
    $expectedAppId = '{' + $run + '}'
    $expectedTlsPorts = @("0.0.0.0:$httpsPort", "[::]:$httpsPort")
    $identitySid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
    if ([IO.Path]::GetFullPath($state.ownedIisExpress) -cne [IO.Path]::GetFullPath($expectedOwnedExe) -or
        [IO.Path]::GetFullPath($state.applicationHost.configPath) -cne [IO.Path]::GetFullPath($expectedHostConfig) -or
        [IO.Path]::GetFullPath($state.applicationHost.templatePath) -cne [IO.Path]::GetFullPath($expectedTemplate) -or
        $state.applicationHost.siteName -cne ('NemeStatsBrowserQA-' + $run) -or
        $state.database.server -cne ('(localdb)\' + $state.database.instance) -or
        $state.database.instance -cnotmatch ('^(MSSQLLocalDB|NemeStatsJq3_' + $RunId.ToString('N') + ')$') -or
        $state.certificate.subject -cne 'CN=localhost' -or
        $state.certificate.dnsName -cne 'localhost' -or
        $state.certificate.storeName -cne 'MY' -or
        $state.certificate.trustStore -cne 'CurrentUser\Root' -or
        $state.certificate.friendlyName -cne ('NemeStatsBrowserQA-' + $run) -or
        $state.certificate.appId -cne $expectedAppId -or
        $state.urlReservation.url -cne $expectedUrl -or
        $state.urlReservation.userSid -cne $identitySid -or
        @($state.tlsBindings).Count -ne 2 -or
        (@($state.tlsBindings | ForEach-Object { $_.ipPort }) -join '|') -cne ($expectedTlsPorts -join '|')) {
        throw 'Cleanup refused because a manifest-owned path, certificate, URL reservation, or TLS binding identifier changed.'
    }
    if ($state.firewallVerified) {
        if (-not (Test-Path -LiteralPath $firewallManifest -PathType Leaf)) {
            throw 'Cleanup refused because the run records verified firewall containment but the firewall manifest is missing.'
        }
        $firewallOutput = @(& $firewallScript -Action verify -RunId $RunId -OwnedInstallRoot $ownedInstallRoot -FirewallManifest $firewallManifest)
        if ($firewallOutput -notcontains 'FirewallContainment=verified') {
            throw 'Cleanup refused because the firewall owner verifier did not confirm the current rules.'
        }
    }
    Assert-Administrator
    if (-not (Test-Path -LiteralPath $state.tools.sqlLocalDb.path -PathType Leaf)) {
        throw 'Cleanup requires the exact LocalDB executable recorded by the run manifest.'
    }
    $script:manifest = $state
    $script:manifest.cleanupStartedUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
    $script:sqlLocalDbPath = $state.tools.sqlLocalDb.path
    $script:effectiveDatabase = "Data Source=$($state.database.server);Initial Catalog=$($state.database.catalog);Integrated Security=True;MultipleActiveResultSets=True"
    Remove-RunOwnedResources $state
    $receipt = [ordered]@{
        runId = $run
        cleanupComplete = $true
        catalogRemoved = -not $state.database.created
        sslBindingsRemoved = @($state.tlsBindings | Where-Object { -not $_.created }).Count
        urlReservationRemoved = -not $state.urlReservation.created
        certificateRemoved = -not $state.certificate.created
        stagedHostRemoved = -not (Test-Path -LiteralPath $stagedHost)
        firewallManifestRemoved = -not (Test-Path -LiteralPath $firewallManifest)
        applicationStarted = $false
        completedUtc = [DateTime]::UtcNow.ToString('o')
    }
    $receipt | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $evidenceRootFull 'task-1-cleanup.json') -Encoding UTF8
    Set-RunFileAcl (Join-Path $evidenceRootFull 'task-1-cleanup.json')
    Write-Output "CleanupReceipt=$(Join-Path $evidenceRootFull 'task-1-cleanup.json')"
    return
}

# All argument, origin, path, and ownership refusals happen before the first write.
if (Test-Path -LiteralPath $stagedHost) { throw "Refusing to overwrite existing staged host state: $stagedHost" }
if (Test-Path -LiteralPath $runManifestPath) { throw "Refusing to overwrite existing run manifest: $runManifestPath" }
if (-not (Test-Path -LiteralPath $firewallScript -PathType Leaf)) {
    throw "Firewall containment helper is missing: $firewallScript"
}
if (-not (Test-Path -LiteralPath $firewallManifest -PathType Leaf)) {
    throw "Owned firewall verification manifest is missing: $firewallManifest"
}
try {
    $firewallOutput = @(& $firewallScript -Action verify -RunId $RunId -OwnedInstallRoot $ownedInstallRoot -FirewallManifest $firewallManifest)
    if ($firewallOutput -notcontains 'FirewallContainment=verified') {
        throw 'The containment helper returned without its verified sentinel.'
    }
}
catch {
    throw "TASK1_BLOCKED: exact owned-executable outbound firewall verification failed: $($_.Exception.Message)"
}
$firewallState = Get-Content -LiteralPath $firewallManifest -Raw | ConvertFrom-Json
$expectedRuleNames = @(
    "NemeStats-jq3-$run-IPv4-low",
    "NemeStats-jq3-$run-IPv4-high",
    "NemeStats-jq3-$run-IPv6-unspecified",
    "NemeStats-jq3-$run-IPv6-nonloopback"
)
if ($firewallState.runId -ne $run -or
    [IO.Path]::GetFullPath($firewallState.executable) -cne [IO.Path]::GetFullPath((Join-Path $ownedInstallRoot 'iisexpress.exe')) -or
    (@($firewallState.ruleNames) -join '|') -cne ($expectedRuleNames -join '|')) {
    throw 'The independently verified firewall manifest does not match the expected run, executable, or four rule identities.'
}

$programFiles = ${env:ProgramFiles}
$programFilesX86 = ${env:ProgramFiles(x86)}
$vswhere = Join-Path $programFilesX86 'Microsoft Visual Studio\Installer\vswhere.exe'
$iisExpress = Join-Path $programFiles 'IIS Express\iisexpress.exe'
$migrate = Join-Path $repoRoot 'packages\EntityFramework.6.2.0\tools\migrate.exe'
$sqlLocalDb = Get-Command 'SqlLocalDB.exe' -ErrorAction SilentlyContinue
if (-not $sqlLocalDb) {
    foreach ($version in @('160', '150', '140', '130')) {
        $candidate = Join-Path $programFiles "Microsoft SQL Server\$version\Tools\Binn\SqlLocalDB.exe"
        if (Test-Path -LiteralPath $candidate -PathType Leaf) { $sqlLocalDb = Get-Item $candidate; break }
    }
}
$sqlCmd = Get-Command 'sqlcmd.exe' -ErrorAction SilentlyContinue
$appSource = Join-Path $repoRoot 'Source\UI'
$supportProject = Join-Path $repoRoot 'Tests\Browser.Support\NemeStats.Browser.Support.csproj'
foreach ($path in @($vswhere, $iisExpress, $migrate, $appSource, $supportProject)) {
    if (-not (Test-Path -LiteralPath $path)) { throw "Required preparation input was not found: $path" }
}
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
    Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($msbuild) -or -not (Test-Path -LiteralPath $msbuild -PathType Leaf)) {
    throw 'A valid MSBuild executable could not be resolved through vswhere.'
}
if (-not $sqlLocalDb) { throw 'SqlLocalDB.exe is required for the explicit fresh LocalDB target.' }
$sqlLocalDbPath = if ($sqlLocalDb.Source) { $sqlLocalDb.Source } else { $sqlLocalDb.FullName }

$netsh = Join-Path $env:WINDIR 'System32\netsh.exe'
$businessLogicAssembly = Join-Path $repoRoot 'Source\BusinessLogic\bin\Release\BusinessLogic.dll'
$applicationHostTemplate = Join-Path $ownedInstallRoot 'config\templates\PersonalWebServer\applicationhost.config'
$iisUserHomeTemplate = Join-Path $ownedInstallRoot 'config\templates\PersonalWebServer\aspnet.config'
$applicationHostPath = Join-Path $stagedHost 'applicationhost.config'
$iisUserHome = Join-Path $stagedHost 'IISUserHome'
$siteName = 'NemeStatsBrowserQA-' + $run
$siteId = 2
$certificateFriendlyName = 'NemeStatsBrowserQA-' + $run
$certificateSubject = 'CN=localhost'
$certificateStore = 'LocalMachine\My'
$trustStore = 'CurrentUser\Root'
$certificateAppId = '{' + $run + '}'
$httpsUrl = $Origin.GetLeftPart([uriPartial]::Authority) + '/'
$script:netsh = $netsh
$script:httpsPort = $httpsPort
$script:httpsUrl = $httpsUrl
$script:runManifestPath = $runManifestPath
foreach ($path in @($netsh, $businessLogicAssembly, $applicationHostTemplate, $iisUserHomeTemplate)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Required owned setup input was not found: $path" }
}
Assert-Administrator
Assert-FreeHttpsReservation

$configPath = Join-Path $appSource 'Web.config'
[xml]$sourceConfig = Get-Content -LiteralPath $configPath -Raw
if (-not $sourceConfig.configuration.appSettings) { throw 'Source Web.config has no appSettings section.' }
if ($sourceConfig.configuration.appSettings.configSource) {
    throw 'Source appSettings uses configSource; task 1 will not silently omit an external section.'
}
if ($sourceConfig.configuration.appSettings.file -and
    $sourceConfig.configuration.appSettings.file -cne 'PrivateAppSettings.config') {
    throw 'Source appSettings references an unexpected external file; refusing to copy it.'
}
$originText = $Origin.GetLeftPart([uriPartial]::Authority)
$catalog = 'NemeStatsJQueryMigration_' + $RunId.ToString('N')
$privateInstanceName = 'NemeStatsJq3_' + $RunId.ToString('N')
$privateServer = '(localdb)\' + $privateInstanceName
$connection = "Data Source=$privateServer;Initial Catalog=$catalog;Integrated Security=True;MultipleActiveResultSets=True"
$privateSettingsPath = Join-Path $stagedHost 'PrivateAppSettings.config'
$targetConnection = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($connection)
if ($targetConnection.DataSource -cne $privateServer -or $targetConnection.InitialCatalog -cne $catalog) {
    throw 'The generated database target failed its local fresh-catalog guard.'
}
$templateConfig = [xml](Get-Content -LiteralPath $applicationHostTemplate -Raw)
$templateSiteIds = @($templateConfig.SelectNodes('/configuration/system.applicationHost/sites/site') |
    ForEach-Object { [int]$_.GetAttribute('id') })
$siteId = 1
if ($templateSiteIds.Count -gt 0) { $siteId = ($templateSiteIds | Measure-Object -Maximum).Maximum + 1 }
$script:migrate = $migrate
$script:sqlLocalDbPath = $sqlLocalDbPath
$script:effectiveDatabase = $connection
$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$userSid = $identity.User.Value

# Write the ownership intent before creating the staged host, database, certificate,
# URL reservation, or HTTP.sys bindings. This manifest contains identifiers only.
$script:manifest = [ordered]@{
    schemaVersion = 1
    runId = $run
    repositoryRoot = $repoRoot
    phase = $Phase
    bundle = $Bundle
    stage = $Stage
    evidenceRoot = $evidenceRootFull
    stagedHost = $stagedHost
    stagedWebConfig = Join-Path $stagedHost 'Web.config'
    iisUserHome = $iisUserHome
    httpsOrigin = $originText
    httpsPort = $httpsPort
    evidenceOwnerSid = $userSid
    ownerIntentRecordedUtc = [DateTime]::UtcNow.ToString('o')
    resourceOwnershipRecordedBeforeCreation = $true
    stagedHostCreateRequested = $true
    migrateCliFlagsUsed = @(
        '/startUpConfigurationFile',
        '/startUpDirectory',
        '/connectionString',
        '/connectionProviderName',
        '/verbose'
    )
    tools = [ordered]@{
        vswhere = [ordered]@{ path = [IO.Path]::GetFullPath($vswhere); version = Get-ToolVersion $vswhere }
        msbuild = [ordered]@{ path = [IO.Path]::GetFullPath($msbuild); version = (& $msbuild -version -nologo | Select-Object -Last 1).Trim() }
        migrate = [ordered]@{ path = [IO.Path]::GetFullPath($migrate); version = Get-ToolVersion $migrate }
        sqlLocalDb = [ordered]@{ path = $sqlLocalDbPath; version = Get-ToolVersion $sqlLocalDbPath }
        sqlCmd = if ($sqlCmd) { [ordered]@{ path = $sqlCmd.Source; version = Get-ToolVersion $sqlCmd.Source } } else { $null }
        sourceIisExpress = [ordered]@{ path = [IO.Path]::GetFullPath($iisExpress); version = Get-ToolVersion $iisExpress }
    }
    firewallManifest = $firewallManifest
    firewallRuleNames = @($firewallState.ruleNames)
    ownedIisExpress = Join-Path $ownedInstallRoot 'iisexpress.exe'
    firewallVerified = $true
    firewallVerificationUtc = [DateTime]::UtcNow.ToString('o')
    effectiveConfigValidated = $false
    callbackIsLocal = $false
    database = [ordered]@{
        server = $privateServer
        instance = $privateInstanceName
        catalog = $catalog
        ownerRunId = $run
        catalogExistedBefore = $null
        createRequested = $false
        created = $false
        ownershipMarkerCreated = $false
        migrationAssembly = Join-Path $stagedHost 'bin\BusinessLogic.dll'
        migrationStartupDirectory = Join-Path $stagedHost 'bin'
        migrationLog = Join-Path $evidenceRootFull 'task-1-migrations.log'
        migrationRequested = $false
        migrationsApplied = $false
    }
    localDbInstance = [ordered]@{
        name = $privateInstanceName
        stateBefore = $null
        startedByRun = $false
        stoppedAfterMigration = $false
    }
    certificate = [ordered]@{
        subject = $certificateSubject
        dnsName = 'localhost'
        friendlyName = $certificateFriendlyName
        storeName = 'MY'
        trustStore = 'CurrentUser\Root'
        appId = $certificateAppId
        thumbprint = $null
        createRequested = $false
        created = $false
        trustedForCurrentUser = $false
        verified = $false
    }
    urlReservation = [ordered]@{
        url = $httpsUrl
        userSid = $userSid
        userName = $identity.Name
        createRequested = $false
        created = $false
        verified = $false
    }
    tlsBindings = @(
        [ordered]@{ ipPort = ('0.0.0.0:{0}' -f $httpsPort); createRequested = $false; created = $false; thumbprint = $null },
        [ordered]@{ ipPort = ('[::]:{0}' -f $httpsPort); createRequested = $false; created = $false; thumbprint = $null }
    )
    applicationHost = [ordered]@{
        configPath = $applicationHostPath
        templatePath = $applicationHostTemplate
        siteName = $siteName
        siteId = $siteId
        physicalPath = $stagedHost
        binding = ('*:{0}:localhost' -f $httpsPort)
        launchArguments = @(
            "/config:$applicationHostPath",
            "/site:$siteName",
            "/userhome:$iisUserHome",
            '/systray:false'
        )
        createRequested = $false
        created = $false
    }
    iisProfile = [ordered]@{
        userHome = $iisUserHome
        aspnetConfigTemplate = $iisUserHomeTemplate
        createRequested = $true
        prepared = $false
    }
    httpsBindingPrepared = $false
    certificatePrepared = $false
    databaseCatalogCreated = $false
    migrationsApplied = $false
    preparationReady = $false
    resourceSetup = 'planned'
    applicationStarted = $false
}
Save-RunManifest

try {
# The copied PrivateAppSettings.config is synthesized; never enumerate, copy, or
# open Miscellaneous/PrivateAppSettings.config from the product checkout.
New-Item -ItemType Directory -Path $stagedHost | Out-Null
$ownerRecord = Join-Path $stagedHost '.task1-owner.json'
@{ runId = $run; repositoryRoot = $repoRoot; stagedHost = $stagedHost } |
    ConvertTo-Json | Set-Content -LiteralPath $ownerRecord -Encoding UTF8
$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$restrictedAcl = New-Object Security.AccessControl.DirectorySecurity
$restrictedAcl.SetAccessRuleProtection($true, $false)
$restrictedAcl.SetOwner($identity.User)
$ownerRule = [Security.AccessControl.FileSystemAccessRule]::new(
    $identity.User,
    [Security.AccessControl.FileSystemRights]::FullControl,
    [Security.AccessControl.InheritanceFlags]'ContainerInherit, ObjectInherit',
    [Security.AccessControl.PropagationFlags]::None,
    [Security.AccessControl.AccessControlType]::Allow
)
$restrictedAcl.AddAccessRule($ownerRule)
Set-Acl -LiteralPath $stagedHost -AclObject $restrictedAcl
& robocopy.exe $appSource $stagedHost /E /COPY:DAT /R:0 /W:0 `
    /XF PrivateAppSettings.config Web.Release.config Web.Debug.config *.dll.config `
    /XD obj
if ($LASTEXITCODE -ge 8) { throw "UI staging copy failed with robocopy exit code $LASTEXITCODE." }

[xml]$stagedConfig = Get-Content -LiteralPath (Join-Path $stagedHost 'Web.config') -Raw
Set-AppSetting $stagedConfig 'urlRoot' $originText
Set-AppSetting $stagedConfig 'emailConfirmationCallbackUrl' ($originText + '/Account/ConfirmEmail')
Set-AppSetting $stagedConfig 'UniversalAnalytics.TrackingId' ''
$exceptionlessModules = @($stagedConfig.SelectNodes('/configuration/system.webServer/modules/add') |
    Where-Object { $_.GetAttribute('name') -eq 'ExceptionlessModule' })
foreach ($module in $exceptionlessModules) { [void]$module.ParentNode.RemoveChild($module) }
$stagedConfig.Save((Join-Path $stagedHost 'Web.config'))

$private = New-Object System.Xml.XmlDocument
$declaration = $private.CreateXmlDeclaration('1.0', 'utf-8', $null)
[void]$private.AppendChild($declaration)
$settings = $private.CreateElement('appSettings')
[void]$private.AppendChild($settings)
$safeSettings = [ordered]@{
    'Database.ConnectionString' = $connection
    'googleAppId' = 'local-browser-test'
    'googleClientSecret' = 'local-browser-test'
    'emailConfirmationCallbackUrl' = $originText + '/Account/ConfirmEmail'
    'urlRoot' = $originText
    'UniversalAnalytics.TrackingId' = ''
    'sendGridApiKey' = 'local-browser-test'
    'rollbarAccessToken' = ''
    'Rollbar.AccessToken' = 'local-test-only-not-a-real-token'
    'exceptionlessApiKey' = ''
}
foreach ($key in $safeSettings.Keys) {
    $entry = $private.CreateElement('add')
    $entry.SetAttribute('key', $key)
    $entry.SetAttribute('value', $safeSettings[$key])
    [void]$settings.AppendChild($entry)
}
$private.Save($privateSettingsPath)

$effectiveConfigPath = Join-Path $stagedHost '.effective-appsettings.config'
$effectiveConfig = New-Object System.Xml.XmlDocument
$effectiveRoot = $effectiveConfig.CreateElement('configuration')
[void]$effectiveConfig.AppendChild($effectiveRoot)
$effectiveSettings = $effectiveConfig.ImportNode($stagedConfig.configuration.appSettings, $true)
[void]$effectiveRoot.AppendChild($effectiveSettings)
$effectiveConfig.Save($effectiveConfigPath)
$configMap = New-Object System.Configuration.ExeConfigurationFileMap
$configMap.ExeConfigFilename = $effectiveConfigPath
$effective = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
    $configMap, [System.Configuration.ConfigurationUserLevel]::None)
$effectiveAppSettings = $effective.AppSettings.Settings
$effectiveDatabase = $effectiveAppSettings['Database.ConnectionString'].Value
$effectiveUrlRoot = $effectiveAppSettings['urlRoot'].Value
$effectiveCallback = $effectiveAppSettings['emailConfirmationCallbackUrl'].Value
$effectiveBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($effectiveDatabase)
if ($effectiveBuilder.DataSource -cne $privateServer -or
    $effectiveBuilder.InitialCatalog -cne $catalog -or
    $effectiveBuilder.AttachDBFilename -or $effectiveBuilder.UserInstance -or
    -not $effectiveBuilder.IntegratedSecurity -or
    -not ([String]::IsNullOrWhiteSpace($effectiveBuilder.UserID)) -or
    -not ([String]::IsNullOrWhiteSpace($effectiveBuilder.Password))) {
    throw 'The merged staged appSettings failed the local fresh-catalog guard.'
}
if ($effectiveUrlRoot -cne $originText -or
    $effectiveCallback -cne ($originText + '/Account/ConfirmEmail')) {
    throw 'The effective staged callback or urlRoot is not the owned local HTTPS origin.'
}

$safetyToolDirectory = Join-Path $stagedHost '.safety-tool'
New-Item -ItemType Directory -Path $safetyToolDirectory | Out-Null
& $msbuild $supportProject /t:Build /p:Configuration=Release "/p:OutputPath=$safetyToolDirectory\" /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw "Safety support build failed with MSBuild exit code $LASTEXITCODE." }
$azureConfiguration = Join-Path $repoRoot 'packages\Microsoft.WindowsAzure.ConfigurationManager.3.2.1\lib\net40\Microsoft.WindowsAzure.Configuration.dll'
$supportAssembly = Join-Path $safetyToolDirectory 'NemeStats.Browser.Support.dll'
$windowsPowerShell = Join-Path $env:WINDIR 'System32\WindowsPowerShell\v1.0\powershell.exe'
foreach ($path in @($azureConfiguration, $supportAssembly, $windowsPowerShell)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Effective-config preflight input was not found: $path" }
}
$probePath = Join-Path $safetyToolDirectory 'invoke-preflight.ps1'
$probeExe = Join-Path $safetyToolDirectory 'preflight.exe'
$probeConfig = Join-Path $safetyToolDirectory 'preflight.exe.config'
$supportConfig = $effectiveConfig.CreateElement('configuration')
$supportAppSettings = $effectiveConfig.ImportNode($stagedConfig.configuration.appSettings, $true)
$supportAppSettings.SetAttribute('file', $privateSettingsPath)
[void]$supportConfig.AppendChild($supportAppSettings)
$supportConfigDocument = New-Object System.Xml.XmlDocument
[void]$supportConfigDocument.AppendChild($supportConfigDocument.ImportNode($supportConfig, $true))
$supportConfigDocument.Save($probeConfig)
$probe = @'
$ErrorActionPreference = 'Stop'
$source = @"
using System;
using NemeStats.Browser.Support;
public static class PreflightProgram
{
    public static int Main(string[] args)
    {
        if (args.Length != 1) return 2;
        try
        {
            var result = SafetyPreflight.ValidateEffectiveConfiguration(args[0]);
            Console.WriteLine("{{\"server\":\"{0}\",\"catalog\":\"{1}\",\"fingerprint\":\"{2}\"}}",
                Escape(result.Server), Escape(result.Catalog), Escape(result.Fingerprint));
            return 0;
        }
        catch
        {
            Console.Error.WriteLine("The staged effective-configuration safety preflight failed.");
            return 3;
        }
    }
    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
"@
Add-Type -TypeDefinition $source -OutputAssembly (Join-Path $PSScriptRoot 'preflight.exe') -OutputType ConsoleApplication -ReferencedAssemblies @((Join-Path $PSScriptRoot 'NemeStats.Browser.Support.dll'), (Join-Path $PSScriptRoot 'Microsoft.WindowsAzure.Configuration.dll'))
& (Join-Path $PSScriptRoot 'preflight.exe') $args[0]
exit $LASTEXITCODE
'@
$probe | Set-Content -LiteralPath $probePath -Encoding UTF8
$safetyOutput = & $windowsPowerShell -NoProfile -NonInteractive -File $probePath $run 2>$null
if ($LASTEXITCODE -ne 0) { throw 'The staged CloudConfigurationManager safety preflight rejected effective appSettings.' }
$effectiveProof = $safetyOutput | ConvertFrom-Json
if ($effectiveProof.catalog -cne $catalog -or
    $effectiveProof.server -cne $privateServer) {
    throw 'The staged CloudConfigurationManager safety preflight returned an unexpected local target.'
}
Remove-Item -LiteralPath $effectiveConfigPath -Force
Remove-Item -LiteralPath $safetyToolDirectory -Recurse -Force

$script:effectiveDatabase = $effectiveDatabase
$script:manifest.effectiveConfigValidated = $true
$script:manifest.effectiveConfigValidatedUtc = [DateTime]::UtcNow.ToString('o')
$script:manifest.effectiveConfigFingerprint = $effectiveProof.fingerprint
$script:manifest.callbackIsLocal = $true
$script:manifest.urlRoot = $originText
Save-RunManifest
Write-Output "EffectiveConfiguration=validated; runId=$run; catalog=$catalog; applicationStarted=false"
if (-not (Test-Path -LiteralPath $script:manifest.database.migrationAssembly -PathType Leaf) -or
    (Get-FileHash -LiteralPath $script:manifest.database.migrationAssembly -Algorithm SHA256).Hash -cne
    (Get-FileHash -LiteralPath $businessLogicAssembly -Algorithm SHA256).Hash) {
    throw 'The staged migration assembly does not match the explicitly selected Release output.'
}

# Never provision compatibility data on a shared or pre-existing SQL instance.
try {
    Initialize-OwnedPrivateLocalDb
    Invoke-Ef6Migrations
    Assert-OwnedPrivateSql
}
finally {
    if ($script:manifest.localDbInstance.startedByRun -and $script:manifest.localDbInstance.created) {
        [void](Assert-PrivateInstanceIdentity)
        $stopOutput = @(& $sqlLocalDbPath stop $privateInstanceName -i 2>&1)
        $stopExit = $LASTEXITCODE
        $script:manifest.localDbInstance.stopExitCode = $stopExit
        $script:manifest.localDbInstance.stopOutput = ($stopOutput -join ' ').Trim()
        $script:manifest.localDbInstance.stoppedAfterMigration = ($stopExit -eq 0)
        Save-RunManifest
        if ($stopExit -ne 0) { throw 'The LocalDB instance started by task 1 could not be stopped cleanly.' }
    }
}

# Stage a disabled HTTPS site configuration only after the local catalog has been
# proven fresh and its EF6 migration has completed. The site is never launched here.
$iisProfileConfigDirectory = Join-Path $iisUserHome 'config'
New-Item -ItemType Directory -Path $iisProfileConfigDirectory -Force | Out-Null
Copy-Item -LiteralPath $iisUserHomeTemplate -Destination (Join-Path $iisProfileConfigDirectory 'aspnet.config')
$script:manifest.iisProfile.prepared = $true
Save-RunManifest
$script:manifest.applicationHost.createRequested = $true
Save-RunManifest
$site = New-OwnedApplicationHostConfig $applicationHostTemplate $applicationHostPath $stagedHost $siteName $httpsPort
$script:manifest.applicationHost.siteId = $site.SiteId
$script:manifest.applicationHost.created = $true
$script:manifest.applicationHost.createdUtc = [DateTime]::UtcNow.ToString('o')
Save-RunManifest

New-OwnedHttpsCertificateAndBindings
$script:manifest.httpsBindingPrepared = $true
$script:manifest.certificatePrepared = $true
$script:manifest.databaseCatalogCreated = $true
$script:manifest.migrationsApplied = $true
$script:manifest.preparationReady = $true
$script:manifest.resourceSetup = 'verified'
$script:manifest.preparedUtc = [DateTime]::UtcNow.ToString('o')
$script:manifest.applicationStarted = $false
Save-RunManifest

$safetyEvidence = [ordered]@{
    runId = $run
    effectiveConfigValidated = $true
    connectionString = [ordered]@{
        server = $effectiveProof.server
        catalog = $effectiveProof.catalog
        fingerprint = $effectiveProof.fingerprint
        credentialsRedacted = $true
    }
    httpsOrigin = $originText
    callbackIsLocal = $true
    existingCatalogAdopted = $false
    externalFirewallManifest = $firewallManifest
    catalogCreated = $true
    migrationsApplied = $true
    migrationCount = $script:manifest.database.migrationCount
    latestMigration = $script:manifest.database.latestMigration
    applicationStarted = $false
}
$safetyEvidence | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidenceRootFull 'task-1-safety.json') -Encoding UTF8
$httpsEvidence = [ordered]@{
    runId = $run
    origin = $originText
    scheme = 'https'
    loopback = $true
    port = $httpsPort
    urlReservation = $httpsUrl
    bindings = $script:manifest.tlsBindings
    certificate = $script:manifest.certificate
    applicationHostConfig = $applicationHostPath
    siteName = $siteName
    bindingConfigured = $script:manifest.httpsBindingPrepared
    certificateConfigured = $script:manifest.certificatePrepared
    applicationStarted = $false
    readinessProven = $false
    nextOwner = 'task 2 after staged root overrides'
}
$httpsEvidence | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidenceRootFull 'task-1-https.json') -Encoding UTF8
$toolingEvidence = [ordered]@{
    runId = $run
    tools = $script:manifest.tools
    firewallManifest = $firewallManifest
    ownedIisExpress = $script:manifest.ownedIisExpress
}
$toolingEvidence | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $evidenceRootFull 'task-1-tooling.json') -Encoding UTF8
$cleanupReceipt = [ordered]@{
    runId = $run
    cleanupComplete = $false
    cleanupDeferredFor = 'task 2 and later browser acceptance'
    cleanupCommand = "pwsh -NoProfile -File Tests/Browser.Tests/prepare.ps1 -Phase cleanup -RunId $run -EvidenceRoot $evidenceRootFull"
    catalog = [ordered]@{
        server = $script:manifest.database.server
        instance = $script:manifest.database.instance
        name = $script:manifest.database.catalog
        ownerRunId = $script:manifest.database.ownerRunId
        ownershipMarker = 'NemeStatsBrowserQaRunId'
        retainedForTask2 = $script:manifest.database.created
    }
    certificate = [ordered]@{
        thumbprint = $script:manifest.certificate.thumbprint
        subject = $script:manifest.certificate.subject
        friendlyName = $script:manifest.certificate.friendlyName
        store = $script:manifest.certificate.storeName
        trustStore = $script:manifest.certificate.trustStore
    }
    tlsBindings = @($script:manifest.tlsBindings | ForEach-Object { $_.ipPort })
    urlReservation = $script:manifest.urlReservation.url
    firewallManifest = $firewallManifest
    firewallRuleNames = $script:manifest.firewallRuleNames
    iisSiteName = $script:manifest.applicationHost.siteName
    iisSiteId = $script:manifest.applicationHost.siteId
    iisApplicationHostConfig = $applicationHostPath
    iisUserHome = $iisUserHome
    iisProcessStarted = $false
    iisProcessIds = @()
    localDbInstanceStartedByRun = $script:manifest.localDbInstance.startedByRun
    localDbStoppedAfterMigration = $script:manifest.localDbInstance.stoppedAfterMigration
    localDbEngineVersion = $script:manifest.localDbInstance.engineVersion
    ownedIisExpressCopyRetained = $script:manifest.ownedIisExpress
    firewallRulesRetainedForTask2 = $true
    teardownPolicy = 'Phase cleanup removes only the manifest-owned catalog, certificate, TLS bindings, URL reservation, staged host/profile, and firewall rules after dependent task2/acceptance work.'
}
$cleanupReceipt | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $evidenceRootFull 'task-1-cleanup.json') -Encoding UTF8
Write-Output "Manifest=$runManifestPath"
Write-Output "Status=task1-preparation-verified; catalog=$catalog; origin=$originText; applicationStarted=false."
}
catch {
    $failure = $_
    $script:manifest.resourceSetup = 'failed'
    $script:manifest.failureType = $failure.Exception.GetType().FullName
    $script:manifest.failureUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
    throw $failure
}
