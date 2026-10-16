[CmdletBinding()]
param([string]$RunManifest, [string]$EvidenceRoot, [string]$QaDirectory, [ValidateSet('normal','overlay')][string]$Phase = 'normal')
if ($EvidenceRoot) {
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$root = [IO.Path]::GetFullPath($EvidenceRoot)
if ($root -cne (Join-Path $repo '.omo\evidence\jquery-3-migration\wave3\task-10')) { throw 'Unowned publish evidence root.' }
$ownership = Get-Content (Join-Path $root 'publish-ownership.json') -Raw | ConvertFrom-Json
$normal = [IO.Path]::GetFullPath($ownership.directory)
if (-not $normal.StartsWith($root + '\normal-publish-', [StringComparison]::OrdinalIgnoreCase) -or
    -not $ownership.emptyBeforePublish -or $ownership.deleteExistingFiles -or $ownership.method -cne 'FileSystem' -or
    $ownership.ownerSid -cne [Security.Principal.WindowsIdentity]::GetCurrent().User.Value) {
    throw 'Normal publish ownership mismatch.'
}
function Manifest([string]$directory) {
    @(Get-ChildItem -LiteralPath $directory -Recurse -File | Sort-Object FullName | ForEach-Object {
        @{path=$_.FullName.Substring($directory.Length+1).Replace('\','/');bytes=$_.Length;
            sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}
    })
}
function Assert([bool]$value,[string]$message) { if (-not $value) { throw $message } }
$files = Manifest $normal
if ($Phase -eq 'overlay') {
    $original = Get-Content (Join-Path $root 'normal-published-assets.json') -Raw | ConvertFrom-Json
    Assert ($files.Count -eq $original.files.Count) 'Untouched normal publish file count changed.'
    foreach($f in $files) {
        $saved = @($original.files | Where-Object path -CEQ $f.path)
        Assert ($saved.Count -eq 1 -and $saved[0].sha256 -ceq $f.sha256 -and $saved[0].bytes -eq $f.bytes) 'Untouched normal publish bytes changed.'
    }
    $qa = [IO.Path]::GetFullPath($QaDirectory)
    Assert ($qa.StartsWith($root + '\',[StringComparison]::OrdinalIgnoreCase) -and $qa -ne $normal) 'QA overlay must be a separate owned copy.'
    $qaFiles = Manifest $qa
    $left = @{}; foreach($f in $files) { $left[$f.path]=$f }
    $right = @{}; foreach($f in $qaFiles) { $right[$f.path]=$f }
    $added = @($right.Keys | Where-Object { -not $left.ContainsKey($_) } | Sort-Object)
    $deleted = @($left.Keys | Where-Object { -not $right.ContainsKey($_) } | Sort-Object)
    $changed = @($left.Keys | Where-Object { $right.ContainsKey($_) -and $left[$_].sha256 -cne $right[$_].sha256 } | Sort-Object)
    $allowAdded = @('PrivateAppSettings.config','bin/NemeStats.Browser.Support.dll') | Sort-Object
    Assert (($added -join '|') -ceq ($allowAdded -join '|')) 'Unexpected QA overlay additions.'
    Assert ($deleted.Count -eq 0) 'QA overlay deleted an application asset.'
    Assert (($changed -join '|') -ceq 'Web.config') 'QA overlay changed an application asset.'
    Assert ($right['bin/NemeStats.Browser.Support.dll'].sha256 -ceq (Get-FileHash (Join-Path $repo 'Tests\Browser.Support\bin\Release\NemeStats.Browser.Support.dll') -Algorithm SHA256).Hash.ToLowerInvariant()) 'Overlay support DLL differs from current build.'
    [xml]$config = Get-Content (Join-Path $qa 'Web.config') -Raw
    $modules = @($config.SelectNodes('/configuration/system.webServer/modules/add[@name="NemeStatsOwnedBrowserQa"]'))
    Assert ($modules.Count -eq 1 -and $modules[0].type -ceq 'NemeStats.Browser.Support.StagedQaModule, NemeStats.Browser.Support') 'Overlay module identity mismatch.'
    @{passed=$true;normalDirectory=$normal;qaDirectory=$qa;normalUnmodified=$true;files=$qaFiles;
        additions=$added;deletions=$deleted;changes=$changed;allowlist=@($allowAdded + 'Web.config');
        moduleType=$modules[0].type;verifiedUtc=[DateTime]::UtcNow.ToString('o')} |
        ConvertTo-Json -Depth 8 | Set-Content (Join-Path $root 'qa-overlay.json') -Encoding utf8
    Write-Output 'Task10OverlayIntegrity=passed'
    exit 0
}
Assert ($files.Count -gt 500) 'Normal filesystem publish is incomplete.'
$unexpected = @($files | Where-Object {
    $_.path -match '(?i)Browser[.\\/](Support|Tests)|NemeStats[.]Browser|observer[.]mjs|run-manifest|jquery-migrate|PrivateAppSettings|fixture|task-[0-9]|jquery-2[.]|jquery[.]signalR-2[.]2[.]'
})
Assert ($unexpected.Count -eq 0) 'QA/private/diagnostic/stale paths leaked into normal publish.'
$textChecks = 0
$privateRun = Get-Content (Join-Path $repo '.omo\evidence\jquery-3-migration\wave1\task-1\6d193a50-1473-43fe-9668-f8f90baa7346-run-manifest.json') -Raw | ConvertFrom-Json
$privateValues = @($privateRun.qaPipeSecret,$privateRun.qaFixturePassword)
foreach($stage in @($privateRun.publishedStages)) {
    if($stage) {
        $stageRun=Get-Content $stage.manifestPath -Raw | ConvertFrom-Json
        $privateValues += @($stageRun.qaPipeSecret,$stageRun.qaFixturePassword)
    }
}
$privateValues = @($privateValues | Where-Object { $_ })
foreach($f in $files | Where-Object { $_.path -match '\.(config|js|cshtml|html|json|xml)$' }) {
    $text = [IO.File]::ReadAllText((Join-Path $normal $f.path))
    Assert ($text -notmatch 'NemeStatsOwnedBrowserQa|BrowserQa\.|qaPipeSecret|qaFixturePassword|NemeStatsQa_|NemeStatsJQueryMigration_|qa_second_[0-9a-f]{32}|qa_[0-9a-f]{32}|jquery-migrate|JQMIGRATE|__nemeStatsObserver') "QA/diagnostic content leaked: $($f.path)"
    foreach($value in $privateValues) { Assert (-not $text.Contains($value)) 'Actual private QA credential leaked into normal output.' }
    $textChecks++
    if ($f.path -like '*.config') {
        [xml]$config = $text
        Assert ($config.SelectNodes('//add[contains(@type,"Browser.Support") or starts-with(@key,"BrowserQa.")]').Count -eq 0) 'QA config parsed in normal output.'
    }
}
Add-Type -AssemblyName System.IO.Compression.FileSystem
$assets = @(
    @('jQuery.3.7.1','Content/Scripts/jquery-3.7.1.js'),
    @('jQuery.3.7.1','Content/Scripts/jquery-3.7.1.min.js'),
    @('jQuery.3.7.1','Content/Scripts/jquery-3.7.1.min.map'),
    @('Microsoft.AspNet.SignalR.JS.2.4.3','Content/Scripts/jquery.signalR-2.4.3.js'),
    @('Microsoft.AspNet.SignalR.JS.2.4.3','Content/Scripts/jquery.signalR-2.4.3.min.js')
)
$official = @()
foreach($asset in $assets) {
    $package = Join-Path $repo ("packages\$($asset[0])\$($asset[0]).nupkg")
    $zip = [IO.Compression.ZipFile]::OpenRead($package)
    try {
        $entries = @($zip.Entries | Where-Object { $_.FullName -ieq $asset[1] })
        Assert ($entries.Count -eq 1) 'Official package entry missing or ambiguous.'
        $entry = $entries[0]
        $reader = [IO.StreamReader]::new($entry.Open())
        try { $upstream = $reader.ReadToEnd() } finally { $reader.Dispose() }
        $relative = 'Scripts/' + [IO.Path]::GetFileName($asset[1])
        $actual = [IO.File]::ReadAllText((Join-Path $normal $relative))
        Assert ($upstream.Replace("`r`n","`n") -ceq $actual.Replace("`r`n","`n")) "Official asset mismatch: $relative"
        $official += @{path=$relative;sha256=($files | Where-Object path -ceq $relative).sha256;
            officialPackage=$asset[0];packageSha256=(Get-FileHash $package -Algorithm SHA256).Hash.ToLowerInvariant();
            officialContentEqualModuloCrlf=$true}
    } finally { $zip.Dispose() }
}
$map = Get-Content (Join-Path $normal 'Scripts\jquery-3.7.1.min.map') -Raw | ConvertFrom-Json
Assert ($map.version -eq 3 -and $map.sources -contains 'jquery-3.7.1.js') 'jQuery official map sources mismatch.'
$assemblies = @()
foreach($name in @('Microsoft.AspNet.SignalR.Core','Microsoft.AspNet.SignalR.SystemWeb')) {
    $file = Join-Path $normal "bin\$name.dll"
    $identity = [Reflection.AssemblyName]::GetAssemblyName($file)
    Assert ($identity.Version.ToString() -ceq '2.4.3.0') 'Published SignalR assembly version mismatch.'
    $package = Join-Path $repo ("packages\$name.2.4.3\lib\net45\$name.dll")
    Assert ((Get-FileHash $file -Algorithm SHA256).Hash -ceq (Get-FileHash $package -Algorithm SHA256).Hash) 'Published SignalR assembly is not official package bytes.'
    $assemblies += @{path="bin/$name.dll";identity=$identity.FullName;sha256=(Get-FileHash $file -Algorithm SHA256).Hash.ToLowerInvariant()}
}
$azure=Join-Path $normal 'bin\Microsoft.WindowsAzure.Configuration.dll'
$officialAzure=Join-Path $repo 'packages\Microsoft.WindowsAzure.ConfigurationManager.3.2.1\lib\net40\Microsoft.WindowsAzure.Configuration.dll'
Assert ((Test-Path $azure) -and (Get-FileHash $azure -Algorithm SHA256).Hash -ceq (Get-FileHash $officialAzure -Algorithm SHA256).Hash) 'Normal web bin lacks the actual official database-configuration dependency.'
$assemblies += @{path='bin/Microsoft.WindowsAzure.Configuration.dll';identity=[Reflection.AssemblyName]::GetAssemblyName($azure).FullName;
    sha256=(Get-FileHash $azure -Algorithm SHA256).Hash.ToLowerInvariant()}
foreach($name in @('UI','BusinessLogic','NemeStats.Hubs')) {
    $file=Join-Path $normal "bin\$name.dll"
    $source=Join-Path $repo "Source\UI\bin\$name.dll"
    Assert ((Get-FileHash $file -Algorithm SHA256).Hash -ceq (Get-FileHash $source -Algorithm SHA256).Hash) 'Normal product assembly differs from current exact Release output.'
    $assemblies += @{path="bin/$name.dll";identity=[Reflection.AssemblyName]::GetAssemblyName($file).FullName;
        sha256=(Get-FileHash $file -Algorithm SHA256).Hash.ToLowerInvariant();matchesCurrentRelease=$true}
}
[xml]$web = Get-Content (Join-Path $normal 'Web.config') -Raw
Assert (-not $web.configuration.'system.web'.compilation.HasAttribute('debug')) 'Release transformation did not remove debug.'
@{passed=$true;normalPublishDirectory=$normal;untouched=$true;qaOverlayApplied=$false;files=$files;
    machineParsedConfigsAndText=$textChecks;actualPrivateValuesExcluded=$privateValues.Count;unexpected=$unexpected;officialAssets=$official;assemblies=$assemblies;
    jqueryMapSources=$map.sources;releaseTransform=$true;verifiedUtc=[DateTime]::UtcNow.ToString('o')} |
    ConvertTo-Json -Depth 10 | Set-Content (Join-Path $root 'normal-published-assets.json') -Encoding utf8
Write-Output 'Task10NormalShippingIntegrity=passed'
    exit 0
}
$ErrorActionPreference = 'Stop'
$m = Get-Content -LiteralPath $RunManifest -Raw | ConvertFrom-Json
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$root = [IO.Path]::GetFullPath($m.evidenceRoot)
$prefix = [IO.Path]::GetFullPath((Join-Path $repo '.omo\evidence\jquery-3-migration')) + '\'
$sid = [Security.Principal.WindowsIdentity]::GetCurrent().User
$acl = Get-Acl -LiteralPath $RunManifest
if (-not $root.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase) -or
    [IO.Path]::GetFullPath($RunManifest) -cne (Join-Path $root ($m.runId + '-run-manifest.json')) -or
    $m.evidenceOwnerSid -cne $sid.Value -or -not $acl.AreAccessRulesProtected -or
    $acl.GetOwner([Security.Principal.SecurityIdentifier]).Value -cne $sid.Value -or
    @($acl.Access | Where-Object { $_.AccessControlType -eq 'Allow' -and
        $_.IdentityReference.Translate([Security.Principal.SecurityIdentifier]).Value -cne $sid.Value }).Count -or
    $m.repositoryRoot -cne $repo -or $m.database.ownerRunId -cne $m.runId -or -not $m.preparationReady) {
    throw 'Unowned exclusion invocation.'
}
$destination = Join-Path $m.evidenceRoot ('task-2-normal-publish-' + [guid]::NewGuid().ToString('D'))
if (Test-Path -LiteralPath $destination) { throw 'Refusing occupied publication path.' }
New-Item -ItemType Directory -Path $destination | Out-Null
# Collect the real Web Publishing Pipeline items without adding support to shipping projects.
# PrivateAppSettings is absent in this checkout; it is not opened or synthesized here.
& $m.tools.msbuild.path (Join-Path $repo 'Source\UI\UI.csproj') `
    /t:PipelinePreDeployCopyAllFilesToOneFolder /p:Configuration=Release `
    "/p:_PackageTempDir=$destination" /p:DeleteExistingFiles=false `
    /p:ExcludeFilesFromDeployment=PrivateAppSettings.config /v:minimal /nologo
$buildExit = $LASTEXITCODE
if ($buildExit -ne 0) {
    @{ runId=$m.runId; passed=$false; buildExitCode=$buildExit; directory=$destination } |
        ConvertTo-Json | Set-Content -LiteralPath (Join-Path $m.evidenceRoot 'task-2-exclusion.json')
    exit $buildExit
}
$paths = @(Get-ChildItem -LiteralPath $destination -Recurse -File | ForEach-Object {
    $_.FullName.Substring($destination.Length + 1)
})
if (-not $paths.Count) { throw 'Normal publishing pipeline returned no files.' }
$unexpected = @($paths | Where-Object { $_ -match 'Browser\.Support|Browser\.Tests|observer\.mjs|run-manifest|task-2|Migrate' })
$configs = @(Get-ChildItem -LiteralPath $destination -Recurse -Filter '*.config' -File)
foreach ($file in $configs) {
    [xml]$config = Get-Content -LiteralPath $file.FullName -Raw
    if ($config.SelectNodes('//add[contains(@type,"Browser.Support") or starts-with(@key,"BrowserQa.") or contains(@name,"NemeStatsOwnedBrowserQa")]').Count) {
        $unexpected += $file.FullName.Substring($destination.Length + 1)
    }
}
$receipt = @{ runId=$m.runId; passed=($unexpected.Count -eq 0); buildExitCode=$buildExit;
    normalPublishDirectory=$destination; files=$paths; unexpected=$unexpected; qaOverlayNotApplied=$true;
    missingLocalSecretFileExcluded='PrivateAppSettings.config'; productionConfigurationRead=$false }
$receipt | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $m.evidenceRoot 'task-2-exclusion.json') -Encoding utf8
if ($unexpected.Count) { throw 'QA support leaked into untouched normal publish output.' }
Write-Output 'Task2NormalPublishExclusion=passed'
exit 0
