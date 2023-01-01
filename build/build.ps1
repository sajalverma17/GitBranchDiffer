$buildConfig = $args[0]

function build {
	param ([string]$config)

If($config -eq "Debug" -or $config -eq "Release") {

    Write-Host "Restoring and building with build config $config"

    msbuild ..\src\GitBranchDiffer.sln /t:Restore /p:Configuration=$config /v:m

    msbuild ..\src\GitBranchDiffer.sln /p:Configuration=$config /p:Platform="Any CPU" /v:m

    return
}

    Write-Host "Invalid build configuration specified"

    return
}

try {
    build($buildConfig)
}
catch{
    Write-Host $_.ScriptStackTrace
    Exit 1
}
