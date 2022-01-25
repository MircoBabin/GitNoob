# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true


function RunMergeWithNoConflicts
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
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'myfile-master-RunWithMergeNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local master'    
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile' -contents 'changed-myfile-master-RunWithMergeNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local master'
        
        GitNoob_GitCheckoutBranch -testdirectory $upstreamRepository.testdirectory -name "unimportant" # Other branch than "master", because the current checked out branch can't be updated
        GitNoob_GitPushBranch -testdirectory $testrepository.testdirectory -remotename "origin" -name "master"
        
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip"
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile-wip' -contents 'myfile-wip1-RunWithMergeNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile-wip'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (1) commit on local wip'

        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'myfile-wip' -contents 'myfile-wip2-RunWithMergeNoConflicts'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'myfile-wip'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'new (2) commit on local wip'

        GitNoob_AssertIsMergeActive -description 'merge not active' `
                                    -testdirectory $testrepository.testdirectory `
                                    -active $false `
                                    -currentBranch '' `
                                    -fromBranch ''

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_GitMergeIntoCurrentBranch -testdirectory $testrepository.testdirectory -fromBranch 'wip'
                                                    
        GitNoob_AssertIsMergeActive -description 'Rebase wip onto main branch "master" with no conflicts' `
                                     -testdirectory $testrepository.testdirectory `
                                     -active $false `
                                     -currentBranch '' `
                                     -fromBranch ''
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function RunMergeWithConflicts
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

        GitNoob_AssertIsMergeActive -description 'rebase not active' `
                                    -testdirectory $testrepository.testdirectory `
                                    -active $false `
                                    -currentBranch '' `
                                    -fromBranch ''

        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "master"
        GitNoob_GitMergeIntoCurrentBranch -testdirectory $testrepository.testdirectory -fromBranch 'wip'
                                                    
        GitNoob_AssertIsMergeActive -description 'Merge wip into main branch "master" with add - add conflicts' `
                                    -testdirectory $testrepository.testdirectory `
                                    -active $true `
                                    -currentBranch 'master' `
                                    -fromBranch 'wip'
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

$upstreamRepository = GitNoob_Repository_CreateTest
try {
    RunMergeWithNoConflicts -upstreamRepository $upstreamRepository
    RunMergeWithConflicts -upstreamRepository $upstreamRepository
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
