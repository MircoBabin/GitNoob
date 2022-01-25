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

function RunOnMainBranch
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
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Not locked' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -Locked $true
                                                   
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Can not lock twice' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -Locked $false
                                                   
        GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock' `
                                                  -testdirectory $testrepository.testdirectory
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Can lock again' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -Locked $true
                                                   
        $testrepository1 = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
        try {
            GitNoob_AssertGetLatest -description 'Get latest on git initialized on main branch "master" empty directory' `
                                    -upstreamTestdirectory $upstreamRepository.testdirectory `
                                    -testdirectory $testrepository1.testdirectory `
                                    -remoteUrl $upstreamRepository.remoteUrl `
                                    -remoteBranch master `
                                    -Cloned $false `
                                    -Updated $true `
                                    -AssertStatus $true
                                    
            GitNoob_GitCheckoutBranch -testdirectory $testrepository1.testdirectory -name "master"
            
            GitNoob_AssertAcquireGitLockForMainBranch -description 'Can not lock - locked by repository' `
                                                      -testdirectory $testrepository1.testdirectory `
                                                      -Locked $false
                                                       
            GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock by repository' `
                                                    -testdirectory $testrepository.testdirectory `
                                                      
                                                       
            GitNoob_AssertAcquireGitLockForMainBranch -description 'Repository1 can lock' `
                                                      -testdirectory $testrepository1.testdirectory `
                                                      -Locked $true
                                                       
            GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock by repository1' `
                                                    -testdirectory $testrepository1.testdirectory `
        } finally {
            GitNoob_DeleteTestDirectory -testdirectory $testrepository1.testdirectory
        }
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Can lock again after repository1 is done' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -Locked $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnMainBranchUnlockedReset
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
                                
        GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock' `
                                                -testdirectory $testrepository.testdirectory
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Not locked' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -Locked $true
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnWip
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
        GitNoob_GitCreateBranch -testdirectory $testrepository.testdirectory -name "wip-local"
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip-local"
        
        GitNoob_AssertStatus -description 'On branch "wip-local"' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-local'
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Not locked' `
                                                   -testdirectory $testrepository.testdirectory `
                                                   -Locked $true
                                                   
        GitNoob_AssertStatus -description '(1) On branch "wip-local"' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-local'
                                                   
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Can not lock twice' `
                                                   -testdirectory $testrepository.testdirectory `
                                                   -Locked $false
                                                   
        GitNoob_AssertStatus -description '(2) On branch "wip-local"' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-local'
                                                   
        GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock' `
                                                  -testdirectory $testrepository.testdirectory
        
        GitNoob_AssertStatus -description '(3) On branch "wip-local"' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-local'
                             
        $gitlock = GitNoob_AssertAcquireGitLockForMainBranch -description 'Can lock again' `
                                                               -testdirectory $testrepository.testdirectory `
                                                               -Locked $true `
                                                               -ReturnGitLock $true
                                                   
        GitNoob_AssertStatus -description '(4) On branch "wip-local"' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'wip-local'
                                                   
        $testrepository1 = GitNoob_Repository_CreateEmpty -upstreamRepository $upstreamRepository
        try {
            GitNoob_AssertGetLatest -description 'Get latest on git initialized on main branch "master" empty directory' `
                                    -upstreamTestdirectory $upstreamRepository.testdirectory `
                                    -testdirectory $testrepository1.testdirectory `
                                    -remoteUrl $upstreamRepository.remoteUrl `
                                    -remoteBranch master `
                                    -Cloned $false `
                                    -Updated $true `
                                    -AssertStatus $true
                                    
            GitNoob_GitCheckoutBranch -testdirectory $testrepository1.testdirectory -name "master"
            GitNoob_GitCreateBranch -testdirectory $testrepository1.testdirectory -name "wip-local"
            GitNoob_GitCheckoutBranch -testdirectory $testrepository1.testdirectory -name "wip-local"
            
            GitNoob_AssertStatus -description 'repository1 (1) On branch "wip-local"' `
                                 -testdirectory $testrepository1.testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false `
                                 -CurrentBranch 'wip-local'
            
            GitNoob_AssertAcquireGitLockForMainBranch -description 'Can not lock - locked by repository' `
                                                       -testdirectory $testrepository1.testdirectory `
                                                       -Locked $false
                                                       
            GitNoob_AssertStatus -description 'repository1 (2) On branch "wip-local"' `
                                 -testdirectory $testrepository1.testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false `
                                 -CurrentBranch 'wip-local'

            GitNoob_AssertGitLockRelease -description 'release lock' `
                                         -GitLock $gitlock `
                                         -Unlocked $true
                                                       
            GitNoob_AssertStatus -description 'repository1 (3) On branch "wip-local"' `
                                 -testdirectory $testrepository1.testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false `
                                 -CurrentBranch 'wip-local'
                                 
            GitNoob_AssertAcquireGitLockForMainBranch -description 'Repository1 can lock' `
                                                       -testdirectory $testrepository1.testdirectory `
                                                       -Locked $true
                                                       
            GitNoob_AssertStatus -description 'repository1 (4) On branch "wip-local"' `
                                 -testdirectory $testrepository1.testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false `
                                 -CurrentBranch 'wip-local'
                                 
            GitNoob_AssertResetGitLockForMainBranch -description 'Forced unlock by repository1' `
                                                    -testdirectory $testrepository1.testdirectory `
                                                      
            GitNoob_AssertStatus -description 'repository1 (5) On branch "wip-local"' `
                                 -testdirectory $testrepository1.testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false `
                                 -CurrentBranch 'wip-local'
                                 
            # 0 = Local, 1 = LocalTrackingRemoteBranch
            $expected = @( `
                            (0 , 'wip-local'   ), `
                            (1 , 'master'      ) `
                         )
            GitNoob_AssertListBranches -description 'No temp branches should exist' `
                                       -testdirectory $testrepository1.testdirectory `
                                       -expected $expected
                                 
        } finally {
            GitNoob_DeleteTestDirectory -testdirectory $testrepository1.testdirectory
        }
        
        GitNoob_AssertAcquireGitLockForMainBranch -description 'Can lock again after repository1 is done' `
                                                   -testdirectory $testrepository.testdirectory `
                                                   -Locked $true
                                                   
        # 0 = Local, 1 = LocalTrackingRemoteBranch
        $expected = @( `
                        (0 , 'wip-local'   ), `
                        (1 , 'master'      ) `
                     )
        GitNoob_AssertListBranches -description 'No temp branches should exist' `
                                   -testdirectory $testrepository.testdirectory `
                                   -expected $expected
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunOnMainBranch -upstreamRepository $upstreamRepository
    RunOnMainBranchUnlockedReset -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}
    
$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunOnWip -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
