# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true


function RunCherryPickWithNoConflicts
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

        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunCherryPickWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunCherryPickWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile-wip' -contents 'myfile-wip1-RunCherryPickWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile-wip'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        $commitId = GitNoob_GitGetLastCommitId -testdirectory $testrepository.testdirectory

        GitNoob_AssertIsCherryPickActive -description 'cherry-pick not active' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $false `
                                         -currentBranch '' `
                                         -fromCommitId ''

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_GitCherryPickIntoCurrentBranch -testdirectory $testrepository.testdirectory -fromCommitId $commitId
                                                    
        GitNoob_AssertIsCherryPickActive -description 'Cherry-pick commit onto main branch "master" with no conflicts' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $false `
                                         -currentBranch '' `
                                         -fromCommitId ''
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunCherryPickWithConflicts
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

        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunCherryPickWithConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunCherryPickWithConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'

        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip1-RunCherryPickWithConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip2-RunCherryPickWithConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'

        $commitId = GitNoob_GitGetLastCommitId -testdirectory $testrepository.testdirectory

        GitNoob_AssertIsCherryPickActive -description 'cherry-pick not active' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $false `
                                         -currentBranch '' `
                                         -fromCommitId ''

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_GitCherryPickIntoCurrentBranch -testdirectory $testrepository.testdirectory -fromCommitId $commitId

        GitNoob_AssertIsCherryPickActive -description 'Cherry-pick commit onto main branch "master" with conflicts' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $true `
                                         -currentBranch 'master' `
                                         -fromCommitId $commitId

        # commit file with conflict markers - normally the file would be edited cleanly.
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'

        GitNoob_AssertIsCherryPickActive -description 'Cherry-pick commit onto main branch "master" with conflicts [staged]' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $true `
                                         -currentBranch 'master' `
                                         -fromCommitId $commitId

        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'cherry-picked conflict'

        GitNoob_AssertIsCherryPickActive -description 'Cherry-pick commit onto main branch "master" with conflicts [committed]' `
                                         -testdirectory $testrepository.testdirectory `
                                         -active $false `
                                         -currentBranch '' `
                                         -fromCommitId ''

    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunCherryPickWithNoConflicts -upstreamRepository $upstreamRepository
    RunCherryPickWithConflicts -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
