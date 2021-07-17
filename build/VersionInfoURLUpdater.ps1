﻿$releaseUrl = $args[0]

$manifestFile = Resolve-Path $PSScriptRoot\..\src\GitBranchDiffer\source.extension.vsixmanifest

[xml]$manifestFileContent = Get-Content $manifestFile

$manifestFileContent.PackageManifest.Metadata.ReleaseNotes = $releaseUrl

$manifestFileContent.Save($manifestFile)

Write-Host "Updated ReleaseNotes URL in VSIX manifest to "$releaseUrl
