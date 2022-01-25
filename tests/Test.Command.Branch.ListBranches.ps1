# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

function TestListBranches
{
    $upstreamRepository = GitNoob_Repository_CreateTest
    try {
        
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

            # 0 = Local, 1 = LocalTrackingRemoteBranch
            $expected = @( `
                            (0 , 'wip'         ), `
                            (1 , 'master'      ) `
                         )

            GitNoob_AssertListBranches -description 'TestListBranches' `
                                       -testdirectory $testrepository.testdirectory `
                                       -expected $expected
                                       
            GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700" #creates remote tracking branch
            GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "test"        #creates remote tracking branch
            GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "wip" 
            
            # 0 = Local, 1 = LocalTrackingRemoteBranch
            $expected = @( `
                            (0 , 'wip'         ), `
                            (1 , 'master'      ), `
                            (1 , 'release2700' ), `
                            (1 , 'test'        ) `
                         )

            GitNoob_AssertListBranches -description 'TestListBranches' `
                                       -testdirectory $testrepository.testdirectory `
                                       -expected $expected
        } finally {
            GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
        }
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $upstreamRepository.testdirectory
    }
   
}


TestListBranches

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
