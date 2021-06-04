$infoUrl = $args[0]

$manifestFile = Resolve-Path $PSScriptRoot\GitBranchDiffer\source.extension.vsixmanifest

[xml]$manifestFileContent = Get-Content $manifestFile

$manifestFileContent.PackageManifest.Metadata.MoreInfo = $infoUrl

$manifestFileContent.Save($manifestFile)

Write-Host "Updated MoreInfo URL in VSIX manifest to "$infoUrl

