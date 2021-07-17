$version = $args[0]

$manifestFile = Resolve-Path $PSScriptRoot\..\src\GitBranchDiffer\source.extension.vsixmanifest

[xml]$manifestFileContent = Get-Content $manifestFile

$manifestFileContent.PackageManifest.Metadata.Identity.Version = $version

$manifestFileContent.Save($manifestFile)

Write-Host "Updated version in VSIX Manifest to "$version

