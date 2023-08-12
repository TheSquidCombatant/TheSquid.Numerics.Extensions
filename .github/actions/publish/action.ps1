# input parameters from git
Param
(
    [string] $version,
    [string] $repository,
    [string] $project,
    [string] $packageGalleryNugetURL,
    [string] $packageGalleryNugetKey,
    [string] $packageGalleryGithubURL,
    [string] $packageGalleryGithubKey
)
# inner constants
$builds = "C:\actions-runner\builds"
$release = "bin\Release"
$stage = "publish"
# log action start
Write-Output "Action started:
    stage=$stage,
    version=$version,
    repository=$repository,
    project=$project,
    packageGalleryNugetURL=$packageGalleryNugetURL,
    packageGalleryGithubURL=$packageGalleryGithubURL"
# publish nuget package
$nugetSymbolsPackage = "$builds\$repository\$version\$repository\$project\$release\$project.$version.symbols.nupkg"
dotnet nuget push $nugetSymbolsPackage --api-key $packageGalleryNugetKey --source $packageGalleryNugetURL
$successOnNuget = ($lastExitCode -eq 0)
$successOnGithub = $false
if ($successOnNuget)
{
    dotnet nuget push $nugetSymbolsPackage --api-key $packageGalleryGithubKey --source $packageGalleryGithubURL
    $successOnGithub = ($lastExitCode -eq 0)
}
# check publish result
$success = $successOnNuget -and $successOnGithub
# output action result
[System.Environment]::SetEnvironmentVariable("success", $success, [System.EnvironmentVariableTarget]::Process)
[System.Environment]::SetEnvironmentVariable("version", $version, [System.EnvironmentVariableTarget]::Process)
# log action finish
Write-Output "Action completed:
    stage=$stage
    success=$success
    version=$version"