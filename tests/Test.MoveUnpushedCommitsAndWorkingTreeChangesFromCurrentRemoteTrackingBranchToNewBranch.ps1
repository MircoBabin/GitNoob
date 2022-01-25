# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

# wip branch means "Work In Progress" branch

function RunWithCommittedUnpushedAndWorkingTreeChanges
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
                                
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-RunWithCommittedUnpushedAndWorkingTreeChanges'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'commit on local release2700'    

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile1' -contents 'myfile1-RunWithCommittedUnpushedAndWorkingTreeChanges'
        
        GitNoob_AssertMoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch -description 'Committed unpushed & workingtree changes' `
                                                                                                         -testdirectory $testrepository.testdirectory `
                                                                                                         -currentbranch 'release2700' `
                                                                                                         -tobranch 'wip-release2700'
                                                                                                         
        GitNoob_AssertFile -description 'myfile should not change' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile' `
                           -expectation 'myfile-RunWithCommittedUnpushedAndWorkingTreeChanges'
                           
        GitNoob_AssertFile -description 'myfile1 should not change' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile1' `
                           -expectation 'myfile1-RunWithCommittedUnpushedAndWorkingTreeChanges'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunWithCommittedUnpushedAndWorkingTreeChanges -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
