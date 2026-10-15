# Test-only provisioning. Dot-source from prepare.ps1 after its argument/path guards.
function Get-OwnedInfrastructureManifest {
    $state = $script:manifest
    if (-not $state.infrastructureManifest) { return $state }
    $path = [IO.Path]::GetFullPath($state.infrastructureManifest)
    $expected = Join-Path $script:manifest.repositoryRoot '.omo\evidence\jquery-3-migration\wave1\task-1\6d193a50-1473-43fe-9668-f8f90baa7346-run-manifest.json'
    $sid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
    $acl = Get-Acl -LiteralPath $path
    if ($path -cne $expected -or -not $acl.AreAccessRulesProtected -or @($acl.Access | Where-Object {
        $_.AccessControlType -eq 'Allow' -and $_.IdentityReference.Translate([Security.Principal.SecurityIdentifier]).Value -cne $sid
    }).Count) { throw 'Published stage infrastructure reference is not the approved protected run.' }
    $infrastructure = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json -AsHashtable
    $entry = @($infrastructure.publishedStages | Where-Object { $_.runId -ceq $state.runId })
    if ($state.stage -cne 'published' -or $entry.Count -ne 1 -or
        $entry[0].manifestPath -cne $script:runManifestPath -or
        $entry[0].database.catalog -cne $state.database.catalog -or
        $state.localDbInstance.name -cne $infrastructure.localDbInstance.name -or
        $state.ownedIisExpress -cne $infrastructure.ownedIisExpress -or
        $state.evidenceOwnerSid -cne $infrastructure.evidenceOwnerSid) {
        throw 'Published catalog or infrastructure ownership link mismatch.'
    }
    return $infrastructure
}

function Assert-PrivateInstanceIdentity {
    $state = $script:manifest.localDbInstance
    $infrastructure = Get-OwnedInfrastructureManifest
    $expectedName = 'NemeStatsJq3_' + ([guid]$infrastructure.runId).ToString('N')
    $sid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
    if ($state.name -cne $expectedName -or $state.ownerRunId -cne $infrastructure.runId -or
        $state.ownerSid -cne $sid -or -not $state.createRequested -or -not $state.created -or
        $state.existedBefore -ne $false -or $script:manifest.database.server -cne ('(localdb)\' + $expectedName)) {
        throw 'Private LocalDB identity or creation ownership mismatch.'
    }
    $info = @(& $script:sqlLocalDbPath info $expectedName 2>&1)
    if ($LASTEXITCODE -ne 0) { throw 'The recorded private LocalDB instance is missing.' }
    $text = $info -join [Environment]::NewLine
    $owner = [Security.Principal.WindowsIdentity]::GetCurrent().Name
    if ($text -notmatch ('(?im)^Name:\s*' + [Regex]::Escape($expectedName) + '\s*$') -or
        $text -notmatch ('(?im)^Owner:\s*' + [Regex]::Escape($owner) + '\s*$') -or
        $text -notmatch '(?im)^Shared name:\s*$' -or
        $text -notmatch ('(?im)^Version:\s*' + [Regex]::Escape($state.engineVersion) + '\s*$')) {
        throw 'LocalDB is shared or its name, owner, or installed engine version differs.'
    }
    return $text
}

function Start-OwnedPrivateInstance {
    $text = Assert-PrivateInstanceIdentity
    if ($text -match '(?im)^State:\s*Stopped\s*$') {
        $script:manifest.localDbInstance.startRequested = $true
        Save-RunManifest
        & $script:sqlLocalDbPath start $script:manifest.localDbInstance.name | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'The run-owned private LocalDB instance did not start.' }
        $script:manifest.localDbInstance.startedByRun = $true
        Save-RunManifest
        $text = Assert-PrivateInstanceIdentity
    }
    if ($text -notmatch '(?im)^State:\s*Running\s*$') { throw 'Private LocalDB is not running.' }
    $pipe = [Regex]::Match($text, '(?im)^Instance pipe name:\s*(.+)\s*$')
    if (-not $pipe.Success) { throw 'Private LocalDB did not report its current pipe.' }
    $script:manifest.localDbInstance.pipe = $pipe.Groups[1].Value.Trim()
    Save-RunManifest
}

function Assert-OwnedPrivateSql {
    [void](Assert-PrivateInstanceIdentity)
    $state = $script:manifest
    $infrastructure = Get-OwnedInfrastructureManifest
    $runValue = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "SELECT CONVERT(nvarchar(36),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaInstanceOwner'" '' ''
    $sidValue = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "SELECT CONVERT(nvarchar(128),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaInstanceOwnerSid'" '' ''
    if ([string]$runValue -cne $infrastructure.runId -or [string]$sidValue -cne $state.evidenceOwnerSid) {
        throw 'Private SQL master ownership marker mismatch.'
    }
    $catalogs = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "SELECT STUFF((SELECT N'|' + name FROM sys.databases WHERE database_id > 4 ORDER BY name FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,1,'')" '' ''
    $ownedCatalogs = @($infrastructure.database, $infrastructure.compatibilityDatabase)
    foreach ($stage in @($infrastructure.publishedStages)) {
        if (-not $stage) { continue }
        $stagePath = [IO.Path]::GetFullPath($stage.manifestPath)
        $stageRoot = Join-Path $state.repositoryRoot ('.omo\evidence\jquery-3-migration\wave3\task-10\' + $stage.runId)
        if ($stagePath -cne (Join-Path $stageRoot ($stage.runId + '-run-manifest.json')) -or
            $stage.database.catalog -cne ('NemeStatsJQueryMigration_' + ([guid]$stage.runId).ToString('N')) -or
            $stage.database.ownerRunId -cne $stage.runId -or
            $stage.database.server -cne $state.database.server) {
            throw 'Additional catalog is not the exact recorded published stage.'
        }
        $stageAcl = Get-Acl -LiteralPath $stagePath
        if (-not $stageAcl.AreAccessRulesProtected -or @($stageAcl.Access | Where-Object {
            $_.AccessControlType -eq 'Allow' -and $_.IdentityReference.Translate([Security.Principal.SecurityIdentifier]).Value -cne $state.evidenceOwnerSid
        }).Count) { throw 'Published stage manifest ACL mismatch.' }
        $recorded = Get-Content -LiteralPath $stagePath -Raw | ConvertFrom-Json -AsHashtable
        if ($recorded.runId -cne $stage.runId -or $recorded.stage -cne 'published' -or
            $recorded.database.catalog -cne $stage.database.catalog -or
            $recorded.database.dataFile -cne $stage.database.dataFile -or
            $recorded.database.logFile -cne $stage.database.logFile -or
            $recorded.database.ownerRunId -cne $stage.runId) { throw 'Published stage catalog cross-record mismatch.' }
        if ($stage.database.created) { $ownedCatalogs += $stage.database }
    }
    $expectedCatalogs = @($ownedCatalogs | ForEach-Object catalog) | Sort-Object
    if ([string]$catalogs -cne ($expectedCatalogs -join '|')) { throw 'Unexpected or missing user catalog in the private instance.' }
    foreach ($catalog in $ownedCatalogs) {
        if (-not $catalog.ownerRunId -or $catalog.catalogExistedBefore -ne $false -or
            -not $catalog.createRequested -or -not $catalog.created) {
            throw 'Catalog creation ownership record mismatch.'
        }
        $marker = Invoke-LocalSqlScalar $script:effectiveDatabase $catalog.catalog `
            "SELECT CONVERT(nvarchar(36),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRunId'" '' ''
        $role = Invoke-LocalSqlScalar $script:effectiveDatabase $catalog.catalog `
            "SELECT CONVERT(nvarchar(32),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRole'" '' ''
        if ([string]$marker -cne $catalog.ownerRunId -or [string]$role -cne $catalog.role) {
            throw 'Private catalog ownership or role marker mismatch.'
        }
        $files = Invoke-LocalSqlScalar $script:effectiveDatabase $catalog.catalog `
            "SELECT STUFF((SELECT N'|' + physical_name FROM sys.database_files ORDER BY file_id FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,1,'')" '' ''
        if ([string]$files -cne ($catalog.dataFile + '|' + $catalog.logFile)) {
            throw 'Private catalog data/log file identities differ from the ownership record.'
        }
    }
    $rows = Invoke-LocalSqlScalar $script:effectiveDatabase 'NerdScorekeeper' 'SELECT COUNT(*) FROM dbo.GameDefinition' '' ''
    if ([int]$rows -ne 0) { throw 'The compatibility table must stay empty.' }
    $columns = Invoke-LocalSqlScalar $script:effectiveDatabase 'NerdScorekeeper' `
        "SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.GameDefinition') AND name IN (N'Name',N'BoardGameGeekGameDefinitionId',N'ThumbnailImageUrl')" '' ''
    if ([int]$columns -ne 3) { throw 'The compatibility table does not have its three required columns.' }
    $script:manifest.privateSqlVerifiedUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
}

function Initialize-OwnedPrivateLocalDb {
    $name = 'NemeStatsJq3_' + ([guid]$script:manifest.runId).ToString('N')
    $server = '(localdb)\' + $name
    $catalog = 'NemeStatsJQueryMigration_' + ([guid]$script:manifest.runId).ToString('N')
    $dataDirectory = Join-Path $script:manifest.evidenceRoot ('sql-data\' + $script:manifest.runId)
    $systemDirectory = Join-Path $env:LOCALAPPDATA ('Microsoft\Microsoft SQL Server Local DB\Instances\' + $name)
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    if ($script:manifest.database.server -cne $server -or $script:manifest.database.catalog -cne $catalog) {
        throw 'Private provisioning target differs from the generated endpoint/catalog.'
    }
    $instances = @(& $script:sqlLocalDbPath info 2>&1)
    if ($LASTEXITCODE -ne 0) { throw 'Could not inventory LocalDB instances before creation.' }
    if ($instances -contains $name) { throw 'Refusing a pre-existing private LocalDB instance.' }
    $pendingDirectory = $script:manifest.localDbInstance.name -ceq $name -and
        $script:manifest.localDbInstance.ownerRunId -ceq $script:manifest.runId -and
        $script:manifest.localDbInstance.ownerSid -ceq $identity.User.Value -and
        $script:manifest.localDbInstance.createRequested -and -not $script:manifest.localDbInstance.created -and
        $script:manifest.localDbInstance.dataDirectoryCreateRequested -and
        $script:manifest.localDbInstance.dataDirectory -ceq $dataDirectory
    $creationIntentUtc = if ($pendingDirectory) { $script:manifest.localDbInstance.createRequestedUtc } else { [DateTime]::UtcNow.ToString('o') }
    if (Test-Path -LiteralPath $dataDirectory) {
        $existing = Get-Item -LiteralPath $dataDirectory
        $existingAcl = Get-Acl -LiteralPath $dataDirectory
        if (-not $pendingDirectory -or $existing.CreationTimeUtc -lt [DateTime]$creationIntentUtc -or
            $existingAcl.GetOwner([Security.Principal.SecurityIdentifier]).Value -cne $identity.User.Value -or
            ($existing.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0 -or
            @(Get-ChildItem -LiteralPath $dataDirectory -Force).Count -ne 0) {
            throw 'Refusing a pre-existing SQL data directory.'
        }
    }
    if (Test-Path -LiteralPath $systemDirectory) { throw 'Refusing a pre-existing private instance system directory.' }
    $versions = @(& $script:sqlLocalDbPath versions 2>&1)
    if ($LASTEXITCODE -ne 0) { throw 'Could not inventory installed LocalDB engines.' }
    $version = [Regex]::Match(($versions -join "`n"), '\(?(17\.0\.\d+\.\d+)\)?')
    if (-not $version.Success) { throw 'Installed LocalDB engine 17.0 was not found.' }
    $script:manifest.localDbInstance = [ordered]@{
        name = $name; ownerRunId = $script:manifest.runId; ownerSid = $identity.User.Value
        engineVersion = $version.Groups[1].Value; requestedVersion = '17.0'
        existedBefore = $false; createRequested = $true; created = $false
        createRequestedUtc = $creationIntentUtc
        stateBefore = 'Stopped'; startedByRun = $false; stoppedAfterMigration = $false
        dataDirectory = $dataDirectory
        systemDirectory = $systemDirectory
        dataDirectoryCreateRequested = $true; dataDirectoryCreated = $false
    }
    $script:manifest.database.role = 'application'
    $script:manifest.database.dataFile = Join-Path $dataDirectory 'application.mdf'
    $script:manifest.database.logFile = Join-Path $dataDirectory 'application_log.ldf'
    $script:manifest.compatibilityDatabase = [ordered]@{
        server = $server; instance = $name; catalog = 'NerdScorekeeper'; role = 'compatibility'
        ownerRunId = $script:manifest.runId; catalogExistedBefore = $false
        createRequested = $false; created = $false; ownershipMarkerCreated = $false
        tableCreateRequested = $false; tableCreated = $false
        dataFile = Join-Path $dataDirectory 'compatibility.mdf'
        logFile = Join-Path $dataDirectory 'compatibility_log.ldf'
    }
    foreach ($entry in @($script:manifest.database, $script:manifest.compatibilityDatabase)) {
        foreach ($file in @($entry.dataFile, $entry.logFile)) {
            if (Test-Path -LiteralPath $file) { throw 'Refusing an existing SQL data/log file.' }
        }
    }
    Save-RunManifest
    if (-not (Test-Path -LiteralPath $dataDirectory)) { New-Item -ItemType Directory -Path $dataDirectory | Out-Null }
    $acl = Get-Acl -LiteralPath $dataDirectory
    if ($acl.GetOwner([Security.Principal.SecurityIdentifier]).Value -cne $identity.User.Value) {
        throw 'SQL directory owner differs from the creating user.'
    }
    $acl.SetAccessRuleProtection($true, $false)
    foreach ($rule in @($acl.Access)) { [void]$acl.RemoveAccessRuleSpecific($rule) }
    $acl.AddAccessRule([Security.AccessControl.FileSystemAccessRule]::new($identity.User, 'FullControl',
        'ContainerInherit, ObjectInherit', 'None', 'Allow'))
    [IO.FileSystemAclExtensions]::SetAccessControl([IO.DirectoryInfo]::new($dataDirectory), $acl)
    $script:manifest.localDbInstance.dataDirectoryCreated = $true
    Save-RunManifest
    & $script:sqlLocalDbPath create $name '17.0' | Out-Null
    $script:manifest.localDbInstance.createExitCode = $LASTEXITCODE
    if ($LASTEXITCODE -ne 0) { Save-RunManifest; throw 'Creating the fresh private LocalDB instance failed.' }
    $script:manifest.localDbInstance.created = $true
    $script:manifest.localDbInstance.createdUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
    Start-OwnedPrivateInstance
    $systemPath = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "SELECT physical_name FROM sys.database_files WHERE name=N'master'" '' ''
    if ([IO.Path]::GetDirectoryName([string]$systemPath) -cne $script:manifest.localDbInstance.systemDirectory) {
        throw 'Private instance system database directory mismatch.'
    }
    $script:manifest.localDbInstance.sqlProcessId = [int](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "SELECT CONVERT(int,SERVERPROPERTY('ProcessID'))" '' '')
    $process = Get-CimInstance Win32_Process -Filter ("ProcessId=" + $script:manifest.localDbInstance.sqlProcessId)
    if (-not $process -or (Invoke-CimMethod -InputObject $process -MethodName GetOwnerSid).Sid -cne $identity.User.Value) {
        throw 'Private SQL process does not belong to the recorded Windows owner SID.'
    }
    $script:manifest.localDbInstance.sqlExecutable = $process.ExecutablePath
    $script:manifest.localDbInstance.sqlCommandLine = $process.CommandLine
    Save-RunManifest
    [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "EXEC sys.sp_addextendedproperty @name=N'NemeStatsBrowserQaInstanceOwner', @value=@runId" '@runId' $script:manifest.runId)
    [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
        "EXEC sys.sp_addextendedproperty @name=N'NemeStatsBrowserQaInstanceOwnerSid', @value=@sid" '@sid' $identity.User.Value)
    $script:manifest.localDbInstance.ownershipMarkerCreated = $true
    Save-RunManifest
    $userCount = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' 'SELECT COUNT(*) FROM sys.databases WHERE database_id>4' '' ''
    if ([int]$userCount -ne 0) { throw 'The fresh private SQL instance already has a user catalog.' }
    foreach ($entry in @($script:manifest.database, $script:manifest.compatibilityDatabase)) {
        $id = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' 'SELECT DB_ID(@catalog)' '@catalog' $entry.catalog
        if ($null -ne $id -and $id -isnot [DBNull]) { throw 'Refusing a pre-existing private catalog.' }
        $entry.catalogExistedBefore = $false
        $entry.createRequested = $true
        $entry.createRequestedUtc = [DateTime]::UtcNow.ToString('o')
        Save-RunManifest
        $data = $entry.dataFile.Replace("'", "''")
        $log = $entry.logFile.Replace("'", "''")
        $db = $entry.catalog
        [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
            "CREATE DATABASE [$db] ON PRIMARY (NAME=N'${db}_data',FILENAME=N'$data') LOG ON (NAME=N'${db}_log',FILENAME=N'$log')" '' '')
        $entry.created = $true
        $entry.createdUtc = [DateTime]::UtcNow.ToString('o')
        Save-RunManifest
        [void](Invoke-LocalSqlScalar $script:effectiveDatabase $db `
            "EXEC sys.sp_addextendedproperty @name=N'NemeStatsBrowserQaRunId', @value=@runId" '@runId' $script:manifest.runId)
        [void](Invoke-LocalSqlScalar $script:effectiveDatabase $db `
            "EXEC sys.sp_addextendedproperty @name=N'NemeStatsBrowserQaRole', @value=@role" '@role' $entry.role)
        $entry.ownershipMarkerCreated = $true
        Save-RunManifest
    }
    $script:manifest.compatibilityDatabase.tableCreateRequested = $true
    Save-RunManifest
    [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'NerdScorekeeper' `
        'CREATE TABLE dbo.GameDefinition (Name nvarchar(255) NOT NULL, BoardGameGeekGameDefinitionId int NULL, ThumbnailImageUrl nvarchar(max) NULL)' '' '')
    $script:manifest.compatibilityDatabase.tableCreated = $true
    $script:manifest.databaseCatalogCreated = $true
    Save-RunManifest
    Assert-OwnedPrivateSql
}

function Remove-OwnedPrivateLocalDb {
    if ($script:manifest.infrastructureManifest -or @($script:manifest.publishedStages | Where-Object { $_.database.created }).Count) {
        throw 'Drop each recorded published catalog through its stage cleanup before infrastructure cleanup.'
    }
    $live = @(Get-CimInstance Win32_Process | Where-Object { $_.ExecutablePath -ceq $script:manifest.ownedIisExpress })
    foreach ($process in $live) {
        if (-not $script:manifest.applicationProcess -or
            [int]$script:manifest.applicationProcess.processId -ne [int]$process.ProcessId -or
            $script:manifest.applicationProcess.executable -cne $process.ExecutablePath) {
            throw 'Private cleanup refused an unrecorded host process.'
        }
        Stop-Process -Id $process.ProcessId -Force
        Wait-Process -Id $process.ProcessId -Timeout 30 -ErrorAction SilentlyContinue
        if (Get-Process -Id $process.ProcessId -ErrorAction SilentlyContinue) { throw 'Owned host did not exit.' }
    }
    Start-OwnedPrivateInstance
    Assert-OwnedPrivateSql
    foreach ($entry in @($script:manifest.compatibilityDatabase, $script:manifest.database)) {
        $entry.dropRequestedUtc = [DateTime]::UtcNow.ToString('o')
        Save-RunManifest
        [void](Invoke-LocalSqlScalar $script:effectiveDatabase 'master' `
            ("ALTER DATABASE [" + $entry.catalog + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + $entry.catalog + "]") '' '')
        $entry.created = $false
        $entry.droppedUtc = [DateTime]::UtcNow.ToString('o')
        Save-RunManifest
    }
    [System.Data.SqlClient.SqlConnection]::ClearAllPools()
    [void](Assert-PrivateInstanceIdentity)
    $remaining = Invoke-LocalSqlScalar $script:effectiveDatabase 'master' 'SELECT COUNT(*) FROM sys.databases WHERE database_id>4' '' ''
    if ([int]$remaining -ne 0) { throw 'Unexpected catalog prevents private instance deletion.' }
    & $script:sqlLocalDbPath stop $script:manifest.localDbInstance.name -i | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not stop the exact run-created private LocalDB instance.' }
    & $script:sqlLocalDbPath delete $script:manifest.localDbInstance.name | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not delete the exact run-created private LocalDB instance.' }
    $instances = @(& $script:sqlLocalDbPath info)
    if ($LASTEXITCODE -ne 0 -or $instances -contains $script:manifest.localDbInstance.name) {
        throw 'Private LocalDB deletion was not independently confirmed.'
    }
    foreach ($entry in @($script:manifest.compatibilityDatabase, $script:manifest.database)) {
        foreach ($file in @($entry.dataFile, $entry.logFile)) {
            if (Test-Path -LiteralPath $file) { throw 'SQL did not remove a recorded catalog file after DROP DATABASE.' }
        }
    }
    $directory = $script:manifest.localDbInstance.dataDirectory
    if (@(Get-ChildItem -LiteralPath $directory -Force).Count) { throw 'Unexpected file prevents SQL directory cleanup.' }
    Remove-Item -LiteralPath $directory
    $script:manifest.localDbInstance.created = $false
    $script:manifest.localDbInstance.deletedUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
}

function Remove-RetiredOwnedCatalogs {
    foreach ($entry in @($script:manifest.retiredDatabases)) {
        $expected = 'NemeStatsJQueryMigration_' + ([guid]$script:manifest.runId).ToString('N')
        if ($entry.server -cne '(localdb)\MSSQLLocalDB' -or $entry.instance -cne 'MSSQLLocalDB' -or
            $entry.catalog -cne $expected -or $entry.ownerRunId -cne $script:manifest.runId -or
            $entry.catalogExistedBefore -ne $false -or -not $entry.createRequested) {
            throw 'Retired catalog cleanup identity/creation record mismatch.'
        }
        if (-not $entry.created) { continue }
        $before = Get-LocalDbState 'MSSQLLocalDB'
        $raw = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=$expected;Integrated Security=True"
        try {
            $marker = Invoke-LocalSqlScalar $raw $expected `
                "SELECT CONVERT(nvarchar(36),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRunId'" '' ''
            if ([string]$marker -cne $script:manifest.runId) { throw 'Retired catalog ownership marker mismatch.' }
            $entry.dropRequestedUtc = [DateTime]::UtcNow.ToString('o')
            Save-RunManifest
            [void](Invoke-LocalSqlScalar $raw 'master' "ALTER DATABASE [$expected] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$expected]" '' '')
            $entry.created = $false
            $entry.droppedUtc = [DateTime]::UtcNow.ToString('o')
            Save-RunManifest
        }
        finally {
            if ($before.State -ceq 'Stopped') {
                & $script:sqlLocalDbPath stop 'MSSQLLocalDB' -i | Out-Null
                if ($LASTEXITCODE -ne 0) { throw 'Could not restore retired-catalog LocalDB state.' }
            }
        }
    }
}

function Assert-EffectivePrivateConfiguration {
    $repo = $script:manifest.repositoryRoot
    $hostPath = $script:manifest.stagedHost
    $toolPath = Join-Path $hostPath '.private-preflight'
    if (Test-Path -LiteralPath $toolPath) { throw 'Pre-existing private preflight directory refused.' }
    New-Item -ItemType Directory -Path $toolPath | Out-Null
    try {
        $support = Join-Path $repo 'Tests\Browser.Support\bin\Release\NemeStats.Browser.Support.dll'
        $azure = Join-Path $repo 'packages\Microsoft.WindowsAzure.ConfigurationManager.3.2.1\lib\net40\Microsoft.WindowsAzure.Configuration.dll'
        Copy-Item -LiteralPath $support, $azure -Destination $toolPath
        [xml]$web = Get-Content -LiteralPath $script:manifest.stagedWebConfig -Raw
        $config = [xml]'<configuration/>'
        $settings = $config.ImportNode($web.configuration.appSettings, $true)
        $settings.SetAttribute('file', (Join-Path $hostPath 'PrivateAppSettings.config'))
        [void]$config.DocumentElement.AppendChild($settings)
        $config.Save((Join-Path $toolPath 'preflight.exe.config'))
        $compiler = @'
param([string]$RunId, [string]$ExpectedServer)
$ErrorActionPreference = 'Stop'
$support = Join-Path $PSScriptRoot 'NemeStats.Browser.Support.dll'
$azure = Join-Path $PSScriptRoot 'Microsoft.WindowsAzure.Configuration.dll'
$source = @"
using System;
using NemeStats.Browser.Support;
public static class PrivatePreflight {
    public static int Main(string[] args) {
        try {
            var result = SafetyPreflight.ValidateEffectiveConfiguration(args[0]);
            if (result.Server != args[1]) throw new InvalidOperationException("Wrong private endpoint.");
            Console.WriteLine(result.Fingerprint);
            return 0;
        } catch { Console.Error.WriteLine("Effective private CloudConfiguration guard failed."); return 3; }
    }
}
"@
Add-Type -TypeDefinition $source -ReferencedAssemblies @($support,$azure) -OutputAssembly (Join-Path $PSScriptRoot 'preflight.exe') -OutputType ConsoleApplication
& (Join-Path $PSScriptRoot 'preflight.exe') $RunId $ExpectedServer
exit $LASTEXITCODE
'@
        $compiler | Set-Content -LiteralPath (Join-Path $toolPath 'compile.ps1') -Encoding utf8
        $result = @(& "$env:WINDIR\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -NonInteractive `
            -File (Join-Path $toolPath 'compile.ps1') $script:manifest.runId $script:manifest.database.server 2>&1)
        if ($LASTEXITCODE -ne 0) { throw ($result | Out-String) }
        $script:manifest.effectiveConfigFingerprint = ($result -join '').Trim()
        $script:manifest.effectiveConfigValidated = $true
        $script:manifest.effectiveConfigValidatedUtc = [DateTime]::UtcNow.ToString('o')
        $script:manifest.callbackIsLocal = $true
        Save-RunManifest
        Write-Output "EffectivePrivateConfiguration=validated; applicationStarted=false"
    }
    finally { Remove-Item -LiteralPath $toolPath -Recurse -Force }
}

function Resume-OwnedPrivatePreparation {
    $state = Get-Content -LiteralPath $script:runManifestPath -Raw | ConvertFrom-Json -AsHashtable
    $expectedCatalog = 'NemeStatsJQueryMigration_' + $RunId.ToString('N')
    $expectedServer = '(localdb)\NemeStatsJq3_' + $RunId.ToString('N')
    $expectedExe = Join-Path $script:ownedInstallRoot 'iisexpress.exe'
    $sid = [Security.Principal.WindowsIdentity]::GetCurrent().User.Value
    $pendingIntent = $state.database.server -ceq $expectedServer -and
        $state.privateProvisionRequestedUtc -and @($state.retiredDatabases).Count -eq 1 -and
        -not $state.database.createRequested -and -not $state.database.created -and
        -not $state.compatibilityDatabase.created -and -not $state.compatibilityDatabase.createRequested -and
        ($state.localDbInstance.name -ceq 'MSSQLLocalDB' -or
            ($state.localDbInstance.name -ceq ('NemeStatsJq3_' + $RunId.ToString('N')) -and
             -not $state.localDbInstance.created -and -not $state.localDbInstance.ownershipMarkerCreated))
    $original = if ($pendingIntent) { $state.retiredDatabases[0] } else { $state.database }
    if ($state.runId -cne $run -or $state.repositoryRoot -cne $repoRoot -or
        $state.evidenceRoot -cne $evidenceRootFull -or $state.stagedHost -cne $script:stagedHost -or
        $state.stagedWebConfig -cne (Join-Path $script:stagedHost 'Web.config') -or
        $state.ownedIisExpress -cne $expectedExe -or $state.evidenceOwnerSid -cne $sid -or
        $state.database.catalog -cne $expectedCatalog -or $state.database.ownerRunId -cne $run -or
        $state.applicationStarted -or $state.preparationReady -or ($state.compatibilityDatabase -and -not $pendingIntent) -or
        $original.server -cne '(localdb)\MSSQLLocalDB' -or $original.catalog -cne $expectedCatalog -or
        $original.ownerRunId -cne $run -or -not $original.created -or $original.catalogExistedBefore -ne $false -or
        -not $original.createRequested -or -not $state.httpsBindingPrepared -or -not $state.certificatePrepared) {
        throw 'Refusing to resume anything other than the exact unbootstrapped owned partial preparation.'
    }
    $ownerPath = Join-Path $state.stagedHost '.task1-owner.json'
    $owner = Get-Content -LiteralPath $ownerPath -Raw | ConvertFrom-Json
    if ($owner.runId -cne $run -or $owner.stagedHost -cne $state.stagedHost) { throw 'Staged host ownership mismatch.' }
    if (@(Get-CimInstance Win32_Process | Where-Object { $_.ExecutablePath -ceq $expectedExe }).Count) {
        throw 'Refusing private provisioning while an owned UI host is running.'
    }
    $firewall = @(& $script:firewallScript -Action verify -RunId $RunId `
        -OwnedInstallRoot $script:ownedInstallRoot -FirewallManifest $script:firewallManifest)
    if ($firewall -notcontains 'FirewallContainment=verified') { throw 'Current exact containment failed before private setup.' }
    $script:manifest = $state
    $script:sqlLocalDbPath = $state.tools.sqlLocalDb.path
    $script:migrate = $state.tools.migrate.path
    $before = Get-LocalDbState 'MSSQLLocalDB'
    try {
        $originalRaw = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=$expectedCatalog;Integrated Security=True"
        $marker = Invoke-LocalSqlScalar $originalRaw $expectedCatalog `
            "SELECT CONVERT(nvarchar(36),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRunId'" '' ''
        if ([string]$marker -cne $run) { throw 'Original partial catalog marker mismatch.' }
    }
    finally {
        if ($before.State -ceq 'Stopped') {
            & $script:sqlLocalDbPath stop 'MSSQLLocalDB' -i | Out-Null
            if ($LASTEXITCODE -ne 0) { throw 'Could not restore original partial-catalog LocalDB state.' }
        }
    }
    # Keep the original partial catalog and its original endpoint as a separate cleanup obligation.
    if (-not $pendingIntent) { $script:manifest.retiredDatabases = @($state.database) }
    $script:manifest.database = [ordered]@{
        server = $expectedServer; instance = 'NemeStatsJq3_' + $RunId.ToString('N')
        catalog = $expectedCatalog; ownerRunId = $run
        catalogExistedBefore = $null; createRequested = $false; created = $false; ownershipMarkerCreated = $false
        migrationAssembly = $state.retiredDatabases[0].migrationAssembly
        migrationStartupDirectory = $state.retiredDatabases[0].migrationStartupDirectory
        migrationLog = Join-Path $evidenceRootFull 'task-1-private-migrations.log'
        migrationRequested = $false; migrationsApplied = $false
    }
    $script:manifest.privateCompatibilityAuthorization = 'User Yes; root ledger owner-authorization line 16'
    $script:manifest.privateProvisionRequestedUtc = [DateTime]::UtcNow.ToString('o')
    Save-RunManifest
    $script:effectiveDatabase = "Data Source=$expectedServer;Initial Catalog=$expectedCatalog;Integrated Security=True;MultipleActiveResultSets=True"
    [xml]$private = Get-Content -LiteralPath (Join-Path $state.stagedHost 'PrivateAppSettings.config') -Raw
    $entries = @($private.appSettings.add | Where-Object { $_.key -ceq 'Database.ConnectionString' })
    if ($entries.Count -ne 1) { throw 'Synthetic staged database setting is missing or duplicated.' }
    $entries[0].SetAttribute('value', $script:effectiveDatabase)
    $private.Save((Join-Path $state.stagedHost 'PrivateAppSettings.config'))
    try {
        Assert-EffectivePrivateConfiguration
        Initialize-OwnedPrivateLocalDb
        Assert-OwnedPrivateSql
        Invoke-Ef6Migrations
        Assert-OwnedPrivateSql
        $state.migrationsApplied = $true
        $state.databaseCatalogCreated = $true
        $state.preparationReady = $true
        $state.resourceSetup = 'verified'
        $state.preparedUtc = [DateTime]::UtcNow.ToString('o')
        [void]$state.Remove('failureType')
        [void]$state.Remove('failureUtc')
        Save-RunManifest
        Write-Output 'PrivatePreparation=verified; ApplicationBootstrap=none'
    }
    catch {
        $state.resourceSetup = 'failed'
        $state.preparationReady = $false
        $state.failureType = $_.Exception.GetType().FullName
        $state.failureUtc = [DateTime]::UtcNow.ToString('o')
        Save-RunManifest
        throw
    }
    finally {
        if ($state.localDbInstance.created -and $state.localDbInstance.ownerRunId -ceq $run) {
            [void](Assert-PrivateInstanceIdentity)
            & $script:sqlLocalDbPath stop $state.localDbInstance.name -i | Out-Null
            $state.localDbInstance.stopExitCode = $LASTEXITCODE
            $state.localDbInstance.stoppedAfterMigration = ($LASTEXITCODE -eq 0)
            Save-RunManifest
            if ($LASTEXITCODE -ne 0) { throw 'Could not stop the run-owned private LocalDB instance after preparation.' }
        }
    }
}
