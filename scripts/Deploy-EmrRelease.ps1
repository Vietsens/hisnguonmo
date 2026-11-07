#Requires -Version 5.0
<#
.SYNOPSIS
Deploys an extracted EMR release to an existing HIS runtime.
.DESCRIPTION
Obtain the release from https://14.224.177.62:4433/ChuongTrinh/EMRRelease
and pass its local, extracted folder as ReleaseRoot. Close HIS before deployment.
Only release DLL/EXE files and Plugin DLL files are synchronized to Integrate\EMR.
Existing connector configuration, data and templates are preserved. Host Json 6
and EMR_MAIN.dll are preserved; the EMR model uses a separate Json 11 codeBase.
Overwritten files are backed up under HisRuntimeRoot\EmrDeploymentBackups.
.EXAMPLE
.\scripts\Deploy-EmrRelease.ps1 -ReleaseRoot D:\EMRRelease -HisRuntimeRoot D:\HIS -WhatIf
.EXAMPLE
.\scripts\Deploy-EmrRelease.ps1 -ReleaseRoot D:\EMRRelease -HisRuntimeRoot D:\HIS `
    -FormMedicalRecordPath D:\Build\HIS.Desktop.Plugins.Library.FormMedicalRecord.dll
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)][string]$ReleaseRoot,
    [Parameter(Mandatory = $true)][string]$HisRuntimeRoot,
    [string]$FormMedicalRecordPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-ExistingPath([string]$Path, [bool]$Directory) {
    $item = Get-Item -LiteralPath $Path
    if ($item.PSIsContainer -ne $Directory) { throw "Unexpected path type: $Path" }
    if ($item.PSProvider.Name -ne 'FileSystem') { throw "A filesystem path is required: $Path" }
    return $item.FullName.TrimEnd([IO.Path]::DirectorySeparatorChar)
}

function Get-AssemblyVersion([string]$Path, [string]$ExpectedName) {
    $identity = [Reflection.AssemblyName]::GetAssemblyName($Path)
    if ($identity.Name -ne $ExpectedName) { throw "Unexpected assembly identity: $Path" }
    return $identity.Version
}

function Test-SameFile([string]$First, [string]$Second) {
    if (-not [IO.File]::Exists($Second)) { return $false }
    if ((Get-Item -LiteralPath $First).Length -ne (Get-Item -LiteralPath $Second).Length) { return $false }
    return (Get-FileHash -LiteralPath $First -Algorithm SHA256).Hash -eq
        (Get-FileHash -LiteralPath $Second -Algorithm SHA256).Hash
}

$releasePath = Get-ExistingPath $ReleaseRoot $true
$runtimePath = Get-ExistingPath $HisRuntimeRoot $true
$separator = [IO.Path]::DirectorySeparatorChar
if ($releasePath.Equals($runtimePath, [StringComparison]::OrdinalIgnoreCase) -or
    $releasePath.StartsWith($runtimePath + $separator, [StringComparison]::OrdinalIgnoreCase) -or
    $runtimePath.StartsWith($releasePath + $separator, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'ReleaseRoot and HisRuntimeRoot must be separate folders, with neither inside the other.'
}
$hisExe = Get-ExistingPath (Join-Path $runtimePath 'HIS.exe') $false
$configPath = Get-ExistingPath (Join-Path $runtimePath 'HIS.exe.config') $false
foreach ($process in @(Get-Process -Name HIS -ErrorAction SilentlyContinue)) {
    if ($process.Path -and $process.Path.Equals($hisExe, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Close HIS from this runtime before deployment.'
    }
}
foreach ($process in @(Get-Process -Name ConnectToEMR -ErrorAction SilentlyContinue)) {
    if ($process.Path -and $process.Path.StartsWith($runtimePath + $separator, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Close ConnectToEMR from this runtime before deployment.'
    }
}

# Complete source/configuration preflight before creating directories or copying files.
$mainPath = Get-ExistingPath (Join-Path $releasePath 'EMR_MAIN.dll') $false
$libraryPath = Get-ExistingPath (Join-Path $releasePath 'EMR_MAIN.Library.dll') $false
$bsonPath = Get-ExistingPath (Join-Path $releasePath 'MongoDB.Bson.dll') $false
$jsonPath = Get-ExistingPath (Join-Path $releasePath 'Newtonsoft.Json.dll') $false
$null = Get-ExistingPath (Join-Path $releasePath 'ConnectToEMR.exe') $false
$mainVersion = Get-AssemblyVersion $mainPath 'EMR_MAIN'
$libraryVersion = Get-AssemblyVersion $libraryPath 'EMR_MAIN.Library'
if ($libraryVersion -lt [version]'26.10.22.1509' -or $libraryVersion -ne $mainVersion) {
    throw 'EMR_MAIN and EMR_MAIN.Library must have matching versions >= 26.10.22.1509.'
}
if ((Get-AssemblyVersion $jsonPath 'Newtonsoft.Json') -ne [version]'11.0.0.0') {
    throw 'The EMR release must contain Newtonsoft.Json assembly version 11.0.0.0.'
}
if ((Get-AssemblyVersion $bsonPath 'MongoDB.Bson') -ne [version]'2.5.0.0') {
    throw 'The EMR release must contain MongoDB.Bson assembly version 2.5.0.0.'
}
if ($FormMedicalRecordPath) {
    $FormMedicalRecordPath = Get-ExistingPath $FormMedicalRecordPath $false
    $null = Get-AssemblyVersion $FormMedicalRecordPath 'HIS.Desktop.Plugins.Library.FormMedicalRecord'
    if ([IO.File]::Exists((Join-Path $runtimePath 'Plugins\Module\HIS.Desktop.Plugins.Library.FormMedicalRecord.dll'))) {
        throw 'A duplicate FormMedicalRecord DLL exists in Plugins\Module. Review the runtime layout before deployment to ReferencedAssemblies.'
    }
}

$config = New-Object System.Xml.XmlDocument
$config.PreserveWhitespace = $true
$config.Load($configPath)
if ($config.DocumentElement.Name -ne 'configuration') { throw 'Invalid HIS.exe.config root element.' }
$originalConfigXml = $config.OuterXml
$runtimeNode = $config.SelectSingleNode('/configuration/runtime')
if (-not $runtimeNode) {
    $runtimeNode = $config.CreateElement('runtime')
    $null = $config.DocumentElement.AppendChild($runtimeNode)
}
$bindingNamespace = 'urn:schemas-microsoft-com:asm.v1'
$namespaces = New-Object System.Xml.XmlNamespaceManager($config.NameTable)
$namespaces.AddNamespace('asm', $bindingNamespace)
$assemblyBinding = $runtimeNode.SelectSingleNode('asm:assemblyBinding', $namespaces)
if (-not $assemblyBinding) {
    $assemblyBinding = $config.CreateElement('assemblyBinding', $bindingNamespace)
    $null = $runtimeNode.AppendChild($assemblyBinding)
}
$jsonBindings = @($runtimeNode.SelectNodes(
    "asm:assemblyBinding/asm:dependentAssembly[asm:assemblyIdentity/@name='Newtonsoft.Json']", $namespaces))
if ($jsonBindings.Count -gt 1) { throw 'Multiple Newtonsoft.Json bindings require manual review before deployment.' }
if ($jsonBindings.Count -eq 1) {
    $jsonBinding = $jsonBindings[0]
    $identity = $jsonBinding.SelectSingleNode('asm:assemblyIdentity', $namespaces)
    if ($identity.GetAttribute('publicKeyToken') -ne '30ad4fe6b2a6aeed' -or
        $identity.GetAttribute('culture') -notin @('', 'neutral')) {
        throw 'Unexpected Newtonsoft.Json identity in HIS.exe.config.'
    }
    foreach ($redirect in @($jsonBinding.SelectNodes('asm:bindingRedirect', $namespaces))) {
        $range = $redirect.GetAttribute('oldVersion').Split('-')
        $low = [version]$range[0]
        $high = [version]$range[$range.Length - 1]
        if ($low -le [version]'11.0.0.0' -and $high -ge [version]'11.0.0.0' -and
            [version]$redirect.GetAttribute('newVersion') -ne [version]'11.0.0.0') {
            throw 'An existing Newtonsoft.Json redirect overrides Json 11. Review that binding before deployment.'
        }
        if ($low -le [version]'6.0.0.0' -and $high -ge [version]'6.0.0.0' -and
            [version]$redirect.GetAttribute('newVersion') -ne [version]'6.0.0.0') {
            throw 'An existing Newtonsoft.Json redirect overrides host Json 6. Review that binding before deployment.'
        }
    }
} else {
    $jsonBinding = $config.CreateElement('dependentAssembly', $bindingNamespace)
    $identity = $config.CreateElement('assemblyIdentity', $bindingNamespace)
    $identity.SetAttribute('name', 'Newtonsoft.Json')
    $identity.SetAttribute('publicKeyToken', '30ad4fe6b2a6aeed')
    $identity.SetAttribute('culture', 'neutral')
    $null = $jsonBinding.AppendChild($identity)
    $null = $assemblyBinding.AppendChild($jsonBinding)
}
$codeBases = @($jsonBinding.SelectNodes("asm:codeBase[@version='11.0.0.0']", $namespaces))
if ($codeBases.Count -gt 1) { throw 'Duplicate Json 11 codeBase entries require manual review.' }
if ($codeBases.Count -eq 1) { $codeBase = $codeBases[0] } else {
    $codeBase = $config.CreateElement('codeBase', $bindingNamespace)
    $codeBase.SetAttribute('version', '11.0.0.0')
    $null = $jsonBinding.AppendChild($codeBase)
}
$codeBase.SetAttribute('href', 'ReferencedAssemblies/EmrJson11/Newtonsoft.Json.dll')
$configChanged = $originalConfigXml -ne $config.OuterXml

$plan = New-Object 'System.Collections.Generic.List[object]'
function Add-Copy([string]$Source, [string]$RelativeDestination) {
    $destination = [IO.Path]::GetFullPath((Join-Path $runtimePath $RelativeDestination))
    if (-not $destination.StartsWith($runtimePath + $separator, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Destination escapes HisRuntimeRoot: $destination"
    }
    if ([IO.Directory]::Exists($destination)) { throw "Destination is a directory: $destination" }
    $plan.Add([pscustomobject]@{ Source = $Source; Destination = $destination; RelativePath = $RelativeDestination })
}
Add-Copy $libraryPath 'ReferencedAssemblies\EMR_MAIN.Library.dll'
Add-Copy $bsonPath 'ReferencedAssemblies\MongoDB.Bson.dll'
Add-Copy $jsonPath 'ReferencedAssemblies\EmrJson11\Newtonsoft.Json.dll'
if ($FormMedicalRecordPath) {
    Add-Copy $FormMedicalRecordPath 'ReferencedAssemblies\HIS.Desktop.Plugins.Library.FormMedicalRecord.dll'
}
foreach ($file in @(Get-ChildItem -LiteralPath $releasePath -File | Where-Object { $_.Extension -in @('.dll', '.exe') })) {
    Add-Copy $file.FullName (Join-Path 'Integrate\EMR' $file.Name)
}
$pluginRoot = Join-Path $releasePath 'Plugin'
if ([IO.Directory]::Exists($pluginRoot)) {
    foreach ($file in @(Get-ChildItem -LiteralPath $pluginRoot -File -Recurse | Where-Object { $_.Extension -eq '.dll' })) {
        $relativeFile = $file.FullName.Substring($pluginRoot.Length + 1)
        Add-Copy $file.FullName (Join-Path 'Integrate\EMR\Plugin' $relativeFile)
    }
}

$backupRoot = Join-Path $runtimePath ('EmrDeploymentBackups\' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [guid]::NewGuid().ToString('N').Substring(0, 8))
$changedFiles = New-Object 'System.Collections.Generic.List[object]'
function Backup-Target([string]$Destination, [string]$RelativePath) {
    $existed = [IO.File]::Exists($Destination)
    if ($existed) {
        $backupPath = Join-Path $backupRoot $RelativePath
        $null = [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($backupPath))
        [IO.File]::Copy($Destination, $backupPath, $false)
    }
    $changedFiles.Add([pscustomobject]@{ RelativePath = $RelativePath; PreviouslyExisted = $existed })
    $null = [IO.Directory]::CreateDirectory($backupRoot)
    $changedFiles | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath (Join-Path $backupRoot 'manifest.json') -Encoding UTF8
}

$unchanged = 0
try {
    foreach ($entry in $plan) {
        if (Test-SameFile $entry.Source $entry.Destination) { $unchanged++; continue }
        if ($PSCmdlet.ShouldProcess($entry.Destination, "Copy $($entry.Source)")) {
            Backup-Target $entry.Destination $entry.RelativePath
            $null = [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($entry.Destination))
            [IO.File]::Copy($entry.Source, $entry.Destination, $true)
        }
    }
    if ($configChanged -and $PSCmdlet.ShouldProcess($configPath, 'Set the exact Json 11 codeBase and preserve other bindings')) {
        Backup-Target $configPath 'HIS.exe.config'
        $config.Save($configPath)
    }
} catch {
    if ([IO.Directory]::Exists($backupRoot)) { Write-Warning "Deployment stopped. Recovery files: $backupRoot" }
    throw
}

[pscustomobject]@{
    ReleaseRoot = $releasePath
    HisRuntimeRoot = $runtimePath
    EmrVersion = $libraryVersion.ToString()
    ChangedFiles = $changedFiles.Count
    UnchangedBinaries = $unchanged
    BackupRoot = $(if ([IO.Directory]::Exists($backupRoot)) { $backupRoot } else { $null })
}
