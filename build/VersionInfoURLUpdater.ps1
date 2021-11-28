$releaseUrl = $args[0]


function UpdateLinkInReleaseNotes() {
	param([string]$manifestFilePath)

$manifestFile = Resolve-Path $manifestFilePath

[xml]$manifestFileContent = Get-Content $manifestFile

$manifestFileContent.PackageManifest.Metadata.ReleaseNotes = $releaseUrl

$manifestFileContent.Save($manifestFile)

Write-Host "Updated ReleaseNotes URL in $manifestFile to $releaseUrl"
}

try {
	UpdateLinkInReleaseNotes($PSScriptRoot + '\..\src\GitBranchDiffer\source.extension.vsixmanifest')
	UpdateLinkInReleaseNotes($PSScriptRoot + '\..\src\GitBranchDiffer2019\source.extension.vsixmanifest')
}
catch {
	Write-Host $_.ScriptStackTrace
    ExitWithExitCode 1 
}


