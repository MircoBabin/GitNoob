# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true


function Run
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testrepository = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
    try {
        GitNoob_AssertGetLatest -description 'Get latest on git initialized on main branch "master" empty directory' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true `
                                -AssertStatus $true
                                
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"

        GitNoob_AssertGetRemoteBranch -description 'Get remote branch' `
                                      -testdirectory $testrepository.testdirectory `
                                      -LocalBranch 'master' `
                                      -RemoteBranch 'origin/master' `
                                      -RemoteName 'origin'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    Run -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
