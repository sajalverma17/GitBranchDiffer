
$tagName = git describe --tag --abbrev=0

# Tag pattern is v*.*.*
$versionString = $tagname.Substring(1, $tagname.length - 1)

Write-Host "Version number from latest Git tag: "$versionString

return $versionString
