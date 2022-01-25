# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

function RunOnNonExistingDirectory
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testdirectory = GitNoob_GetNonExistingTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest on non existing directory' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false `
                                -AssertStatus $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function RunOnExistingEmptyDirectory
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testdirectory = GitNoob_CreateTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest on existing empty directory' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false `
                                -AssertStatus $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function RunOnExistingNongitFilledDirectory
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testdirectory = GitNoob_CreateTestDirectory
    try {
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile'
        
        GitNoob_AssertGetLatest -description 'Get latest on existing nongit filled directory' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorNonEmptyAndNotAGitRepository $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunOnNonExistingDirectory -upstreamRepository $upstreamRepository
    RunOnExistingEmptyDirectory -upstreamRepository $upstreamRepository
    RunOnExistingNongitFilledDirectory -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
