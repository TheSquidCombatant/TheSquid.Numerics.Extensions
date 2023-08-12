function Build
{
    # input parameters from git
    Param
    (
        [string] $revision,
        [string] $repository,
        [string] $project,
        [string] $owner,
        [string] $branch,
        [string] $commit,
        [string] $token
    )
    # inner constants
    $builds = "C:\actions-runner\builds"
    $release = "bin\Release"
    $stage = "build"
    $maxVersionValue = 65535
    # log action start
    Write-Output "Action started:
        stage=$stage
        revision=$revision
        repository=$repository
        project=$project
        owner=$owner
        branch=$branch
        commit=$commit"
    # create temporary forlder and clone here
    $tempPath = "$env:TEMP\git\" + [Guid]::NewGuid().ToString()
    if (Test-Path $tempPath) { Remove-Item $tempPath -Recurse -Force }
    New-Item -Path $tempPath -ItemType Directory
    Set-Location $tempPath
    $uri = "https://$token@github.com/$owner/$repository.git"
    git clone $uri --branch $branch --single-branch
    # switch to target commit and find lastest tag
    Set-Location "$tempPath\$repository"
    git checkout $commit
    $tag = git describe --tags | Out-String
    # define major and minor version parts
    if ($tag -match "v[0-9]+.[0-9]+")
    {
        $result = $tag | Select-String "[0-9]+" -AllMatches
        $major = $result.Matches[0].Value
        $minor = $result.Matches[1].Value
    }
    else
    {
        Write-Output "tag=$tag"
        $major = 0
        $minor = 0
    }
    # to prevent overflow in roslyn compiler
    $major = $major % $maxVersionValue
    $minor = $minor % $maxVersionValue
    $revision = $revision % $maxVersionValue
    # define revision and build version parts
    for ($build=1; $build -le $maxVersionValue; $build++)
    {
        $buildPath = "$builds\$repository\$major.$minor.$revision.$build"
        $exists = Test-Path $buildPath
        if(!$exists) { break }
    }
    # create build folder and move here
    if (Test-Path $buildPath) { Remove-Item $buildPath -Recurse -Force }
    Copy-Item -Path $tempPath -Destination $buildPath -Recurse
    Set-Location $buildPath
    Remove-Item $tempPath -Recurse -Force
    # build library project
    $version = "$major.$minor.$revision.$build"
    $output = "$buildPath\$repository\$project\$release"
    $logFile = "$output\BuildResults.log"
    $projectFile = "$buildPath\$repository\$project\$project.csproj"
    dotnet build $projectFile `
        -c Release `
        -p:OutputPath=$output `
        -p:Version=$version `
        -p:AssemblyVersion=$version `
        -p:FileVersion=$version `
        /flp:"v=normal;logfile=$logFile"
    # check build result
    $libraryFile = "$output\$project.dll"
    $success = (Get-Item $libraryFile).VersionInfo.ProductVersion -eq $version
    # output action result
    [System.Environment]::SetEnvironmentVariable("success", $success, [System.EnvironmentVariableTarget]::Process)
    [System.Environment]::SetEnvironmentVariable("version", $version, [System.EnvironmentVariableTarget]::Process)
    # log action finish
    Write-Output "Action completed:
        stage=$stage
        success=$success
        version=$version"
}