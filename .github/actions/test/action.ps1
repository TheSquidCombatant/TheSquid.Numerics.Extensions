function Test
{
    # input parameters from git
    Param
    (
        [string] $version,
        [string] $repository,
        [string] $project,
        [string] $methods
    )
    # inner constants
    $builds = "C:\actions-runner\builds"
    $release = "bin\Debug"
    $stage = "test"
    # log action start
    Write-Output "Action started:
        stage=$stage
        version=$version
        repository=$repository
        project=$project
        methods=$methods"
    # run selected tests
    $path = "$builds\$repository\$version\$repository\$project"
    $output = "$path\$repository\$project\$release"
    $logFile = "$output\TestResults.trx"
    dotnet test "$path\$project.csproj" `
        -c Debug `
        -p:OutputPath=$output `
        --logger "trx;logfilename=$logFile" `
        --filter $methods
    # check test result
    $success = ($lastExitCode -eq 0)
    # output action result
    [System.Environment]::SetEnvironmentVariable("success", $success, [System.EnvironmentVariableTarget]::Process)
    [System.Environment]::SetEnvironmentVariable("version", $version, [System.EnvironmentVariableTarget]::Process)
    # log action finish
    Write-Output "Action completed:
        stage=$stage
        success=$success
        version=$version"
}