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

function RunWithCommittedUnpushedChanges
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
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithCommittedUnpushedChanges'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'commit on local master'    

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-RunWithCommittedUnpushedChanges'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'commit on local release2700'    

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile1' -contents 'myfile1-RunWithCommittedUnpushedChanges'
        
        
        GitNoob_AssertMoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch -description 'Committed unpushed changes on "master"' `
                                                                             -testdirectory $testrepository.testdirectory `
                                                                             -frombranch 'master' `
                                                                             -tobranch 'wip-master'
                                                                             
        GitNoob_AssertStatus -description 'Still on branch release2700' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'release2700'
                                                                                                         
        GitNoob_AssertFile -description 'myfile should not change' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile' `
                           -expectation 'myfile-RunWithCommittedUnpushedChanges'
                           
        GitNoob_AssertFile -description 'myfile1 should not change' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile1' `
                           -expectation 'myfile1-RunWithCommittedUnpushedChanges'

        # undo working tree changes                           
        GitNoob_DeleteFile -testdirectory $testrepository.testdirectory -name 'myfile1'
        
        # main branch
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_AssertGetLatest -description 'Get latest on main branch "master" should succeed' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true `
                                -AssertStatus $true
                                
        # wip-master branch
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip-master"
        GitNoob_AssertStatus -description 'wip-master branch is created' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-master'
                             
        GitNoob_AssertFile -description 'myfile should not change on wip-master' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile' `
                           -expectation 'myfile-master-RunWithCommittedUnpushedChanges'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunWithCommittedUnpushedChanges -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
