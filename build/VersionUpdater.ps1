$version = $args[0]

function Update-Version {
	param ([string]$manifestFilePath)

$manifestFile = Resolve-Path $manifestFilePath

[xml]$manifestFileContent = Get-Content $manifestFile

$manifestFileContent.PackageManifest.Metadata.Identity.Version = $version

$manifestFileContent.Save($manifestFile)

Write-Host "Updated version in $manifestFile to $version"
}

try {
    Update-Version($PSScriptRoot + '\..\src\GitBranchDiffer\source.extension.vsixmanifest')
    Update-Version($PSScriptRoot + '\..\src\GitBranchDiffer2019\source.extension.vsixmanifest')
}
catch{
    Write-Host $_.ScriptStackTrace
    ExitWithExitCode 1   
}