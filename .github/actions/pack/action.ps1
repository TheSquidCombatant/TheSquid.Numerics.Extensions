# input parameters from git
Param
(
    [string] $version,
    [string] $repository,
    [string] $project,
    [string] $owner,
    [string] $branch,
    [string] $commit,
    [string] $about,
    [string] $token
)
# inner constants
$builds = "C:\actions-runner\builds"
$release = "bin\Release"
$stage = "pack"
$aboutMaxLength = 110
$aboutForbiddenSymbols = ',', ';'
# log action start
Write-Output "Action started:
    stage=$stage
    version=$version
    repository=$repository
    project=$project
    owner=$owner
    branch=$branch
    commit=$commit
    about=$about"
# try get information about repository
if (!$about)
{
    $repositoryPath = "$builds\$repository\$version\$repository"
    Set-Location $repositoryPath
    Invoke-Expression "`"$token`" | gh auth login --with-token"
    $outputGithub = gh repo view | Out-String
    if (!$lastExitCode)
    {
        $result = $outputGithub -match "description:(.*)\r\n"
        if ($result) { $about=$matches[1].Trim() }
        foreach ($symbol in $aboutForbiddenSymbols) { $about = $about.Replace($symbol , "") }
        if ($about.Length -gt $aboutMaxLength) { $about = $about.Substring(0, $aboutMaxLength) }
        Write-Output "New about=$about"
    }
    else
    {
        Write-Output $outputGithub
    }
}
# create nuget package
$projectFolder = "$builds\$repository\$version\$repository\$project"
$input = $output = "$projectFolder\$release"
$nugetPackage = "$output\$project.$version.nupkg"
$nugetSymbolsPackage = "$output\$project.$version.symbols.nupkg"
$url = "https://github.com/$owner/$repository"
$projectFile = "$projectFolder\$project.csproj"
if (Test-Path $nugetPackage) { Remove-Item $nugetPackage }
if (Test-Path $nugetSymbolsPackage) { Remove-Item $nugetSymbolsPackage }
dotnet pack $projectFile `
    --no-build `
    --no-restore `
    --include-source `
    -c Release `
    -o $output `
    -p:OutputPath=$input `
    -p:Version=$version `
    -p:Authors=$owner `
    -p:Owners=$owner `
    -p:Title="$about" `
    -p:Description="$about" `
    -p:PackageProjectUrl=$url `
    -p:RepositoryUrl=$url `
    -p:RepositoryType="git" `
    -p:RepositoryBranch=$branch `
    -p:RepositoryCommit=$commit
# check create result
$success = (Test-Path $nugetPackage) -and (Test-Path $nugetSymbolsPackage)
# output action result
[System.Environment]::SetEnvironmentVariable("success", $success, [System.EnvironmentVariableTarget]::Process)
[System.Environment]::SetEnvironmentVariable("version", $version, [System.EnvironmentVariableTarget]::Process)
# log action finish
Write-Output "Action completed:
    stage=$stage
    success=$success
    version=$version"