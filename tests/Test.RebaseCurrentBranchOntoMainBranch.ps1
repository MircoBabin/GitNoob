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

function RunWithStagedUncommittedFilesAndWorkingTreeChangesAndUnpushedCommitsOnMainBranch
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithStagedUncommittedFilesAndWorkingTreeChangesAndUnpushedCommitsOnMainBranch'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'unpushed commit on local master'    
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile1' -contents 'myfile1-wip-RunWithStagedUncommittedFilesAndWorkingTreeChangesAndUnpushedCommitsOnMainBranch'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile1'
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile2' -contents 'myfile2-wip-RunWithStagedUncommittedFilesAndWorkingTreeChangesAndUnpushedCommitsOnMainBranch'
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with staged uncommitted files, workingtree changes and unpushed commits on main branch' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $false `
                                                        -ErrorWorkingTreeChanges $true `
                                                        -ErrorStagedUncommittedFiles $true `
                                                        -ErrorUnpushedCommitsOnMainBranch $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithNoCommits
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithNoCommits'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithNoCommits'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'

        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto master with no commits' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $true
                                                    
        GitNoob_AssertFile -description 'myfile should be same as commit(2)' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile' `
                           -expectation 'changed-myfile-master-RunWithNoCommits'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithNoConflicts
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile1' -contents 'myfile1-wip-RunWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile1'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile2' -contents 'myfile2-wip-RunWithNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile2'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with no conflicts' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $true
                                                    
        GitNoob_AssertFile -description 'myfile should be same as commit(2)' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile' `
                           -expectation 'changed-myfile-master-RunWithNoConflicts'
                                                    
        GitNoob_AssertFile -description 'myfile1 should be same as commit(1)' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile1' `
                           -expectation 'myfile1-wip-RunWithNoConflicts'
                           
        GitNoob_AssertFile -description 'myfile2 should be same as commit(2)' `
                           -testdirectory $testrepository.testdirectory `
                           -name 'myfile2' `
                           -expectation 'myfile2-wip-RunWithNoConflicts'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithModifyModifyConflicts
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithModifyModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithModifyModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip1-RunWithModifyModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip2-RunWithModifyModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with modify - modify conflicts' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $false `
                                                        -ErrorConflicts $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithAddAddConflicts
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithAddAddConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithAddAddConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip1-RunWithAddAddConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip2-RunWithAddAddConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with add - add conflicts' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $false `
                                                        -ErrorConflicts $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithDeleteModifyConflicts
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

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithDeleteModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_DeleteFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'delete (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip1-RunWithDeleteModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-wip2-RunWithDeleteModifyConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'
        
        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with delete - modify conflicts' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $false `
                                                        -ErrorConflicts $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithModifyDeleteConflicts
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

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithModifyDeleteConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithModifyDeleteConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_DeleteFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'delete (1) commit on local wip'

        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with modify - delete conflicts' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $false `
                                                        -ErrorConflicts $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunWithDeleteDeleteNoConflicts
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

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithDeleteDeleteNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_DeleteFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'delete (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_DeleteFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'delete (1) commit on local wip'

        GitNoob_AssertRebaseCurrentBranchOntoMainBranch -description 'Rebase wip onto main branch "master" with delete - delete' `
                                                        -testdirectory $testrepository.testdirectory `
                                                        -Rebased $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunWithStagedUncommittedFilesAndWorkingTreeChangesAndUnpushedCommitsOnMainBranch -upstreamRepository $upstreamRepository
    RunWithNoCommits -upstreamRepository $upstreamRepository
    RunWithNoConflicts -upstreamRepository $upstreamRepository
    
    RunWithModifyModifyConflicts -upstreamRepository $upstreamRepository
    RunWithAddAddConflicts -upstreamRepository $upstreamRepository
    RunWithDeleteModifyConflicts -upstreamRepository $upstreamRepository
    RunWithModifyDeleteConflicts -upstreamRepository $upstreamRepository
    
    RunWithDeleteDeleteNoConflicts -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
