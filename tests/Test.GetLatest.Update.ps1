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

function RunOnGitInitialized
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

        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700" # creates remote tracking branch
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"

        GitNoob_Repository_UpdateTest -repository $upstreamRepository -name 'RunOnGitInitialized'
                                
        GitNoob_AssertGetLatest -description 'Get latest on fetched repository on branch wip' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true `
                                -AssertStatus $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitInitializedWorkingTreeChangesOnMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testrepository = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
    try {
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'newfile' -contents 'newfile'
        
        GitNoob_AssertGetLatest -description 'Get latest with workingtree changes (on "master")' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitInitializedStagedUncommittedChangesOnMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testrepository = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
    try {
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'newfile' -contents 'newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'newfile'
        
        GitNoob_AssertGetLatest -description 'Get latest with staged uncommitted changes (on "master")' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorStagedUncommittedFiles $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitInitializedCommittedChangesOnMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$upstreamRepository
    )
    
    $testrepository = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
    try {
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'first commit on local master'    
        
        GitNoob_AssertGetLatest -description 'Get latest with committed changes in "master" (on "master") [1]' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorUnpushedCommitsOnMainBranch $true `
                                -ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch $false # checked out before GetLatest (so not automatically tracking remote)
                                
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile1' -contents 'myfile1'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile1'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'second commit on local master'    
        
        GitNoob_AssertGetLatest -description 'Get latest with committed changes in "master" (on "master") [2]' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorUnpushedCommitsOnMainBranch $true `
                                -ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch $false # checked out before GetLatest (so not automatically tracking remote)
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitFetchedWorkingTreeChangesOnWip
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
                                
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
                                
        GitNoob_Repository_UpdateTest -repository $upstreamRepository -name 'RunOnGitFetchedWorkingTreeChangesOnWip'
                                
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'my-newfile' -contents 'my-newfile'
        
        GitNoob_AssertGetLatest -description 'Get latest with workingtree changes (on wip)' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true `
                                -StashBeforeCompare wip
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitFetchedStagedUncommittedChangesOnWip
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
                                
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'my-newfile' -contents 'my-newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'my-newfile'
        
        GitNoob_AssertGetLatest -description 'Get latest with staged uncommitted changes (on wip)' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -AssertStatus $false `
                                -ErrorStagedUncommittedFiles $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitFetchedCommittedChangesOnWip
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
                                
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'my-newfile' -contents 'my-newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'my-newfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'first commit on wip'

        GitNoob_Repository_UpdateTest -repository $upstreamRepository -name 'RunOnGitFetchedCommittedChangesOnWip'
        
        GitNoob_AssertGetLatest -description 'Get latest with committed changes in wip (on wip)' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnGitFetchedCommittedChangesOnRelease2700
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
                                
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700" # creates remote tracking branch
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'my-newfile' -contents 'my-newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'my-newfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'not so smart commit on local release2700'
        
        $commitid = GitNoob_GitGetLastCommitId -testdirectory $testrepository.testdirectory

        GitNoob_Repository_UpdateTest -repository $upstreamRepository -name 'RunOnGitFetchedCommittedChangesOnRelease2700'
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700"
        
        GitNoob_AssertGetLatest -description 'Get latest with committed changes in release2700 (on release2700)' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $false `
                                -ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch $true

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        
        GitNoob_AssertGetLatest -description 'Get latest with committed changes in release 2700 (on wip)' `
                                -upstreamTestdirectory $upstreamRepository.testdirectory `
                                -testdirectory $testrepository.testdirectory `
                                -remoteUrl $upstreamRepository.remoteUrl `
                                -remoteBranch master `
                                -Cloned $false `
                                -Updated $true

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700"
        $commitid1 = GitNoob_GitGetLastCommitId -testdirectory $testrepository.testdirectory
        GitNoob_AssertSame -value $commitid1 -expectation $commitid -message "release2700 remote tracking branch should not change, because fast-forward is not possible due to local commit"
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunOnGitInitialized -upstreamRepository $upstreamRepository
    RunOnGitInitializedWorkingTreeChangesOnMainBranch -upstreamRepository $upstreamRepository
    RunOnGitInitializedStagedUncommittedChangesOnMainBranch -upstreamRepository $upstreamRepository
    RunOnGitInitializedCommittedChangesOnMainBranch -upstreamRepository $upstreamRepository

    RunOnGitFetchedWorkingTreeChangesOnWip -upstreamRepository $upstreamRepository
    RunOnGitFetchedStagedUncommittedChangesOnWip -upstreamRepository $upstreamRepository
    RunOnGitFetchedCommittedChangesOnWip -upstreamRepository $upstreamRepository
    
    RunOnGitFetchedCommittedChangesOnRelease2700 -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
