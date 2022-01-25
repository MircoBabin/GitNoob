# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

function RunOnNonExistingMainBranch
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
                                
        GitNoob_AssertStatus -description 'Should be on "master" branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
                             
        GitNoob_AssertEnsureMainBranchExistance -description 'Non existing branch "release0000"' `
                                                -testdirectory $testrepository.testdirectory `
                                                -mainBranch release0000 `
                                                -Exists $false
                                                  
        GitNoob_AssertStatus -description 'Should not change branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnNeverCheckedoutMainBranch
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
                                
        GitNoob_AssertStatus -description 'Should be on "master" branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
                             
        GitNoob_AssertEnsureMainBranchExistance -description 'Not checked out branch "release2700"' `
                                                  -testdirectory $testrepository.testdirectory `
                                                  -mainBranch release2700 `
                                                  -Exists $true
                                                  
        GitNoob_AssertStatus -description 'Should not change branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunOnExistingMainBranch
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
        GitNoob_AssertStatus -description 'Should checkout "release2700" branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'release2700'
                             
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_AssertStatus -description 'Should be on "master" branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
                             
                             
        GitNoob_AssertEnsureMainBranchExistance -description 'Existing branch "release2700"' `
                                                -testdirectory $testrepository.testdirectory `
                                                -mainBranch release2700 `
                                                -Exists $true
                                                  
        GitNoob_AssertStatus -description 'Should not change branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunOnNonExistingMainBranch -upstreamRepository $upstreamRepository
    RunOnNeverCheckedoutMainBranch -upstreamRepository $upstreamRepository
    RunOnExistingMainBranch -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
