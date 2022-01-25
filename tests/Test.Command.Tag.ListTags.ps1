# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

function TestListTags
{
    $testrepository = GitNoob_Repository_CreateEmpty
    try {
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'newfile' -contents 'newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'newfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'first commit on master'

        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "first.commit" -annotated $false
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "first.commit.annotated.with.name" -annotated $true
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "first.commit.annotated.with.message" -annotated $true -message "This is the message for:`nfirst commit annotated with message"
        
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'newfile1' -contents 'newfile1'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'newfile1'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'second commit on master'
        
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "second.commit.annotated.with.message" -annotated $true -message "This is the message for:`nsecond commit annotated with`nmessage"
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "second.commit.annotated.with.name" -annotated $true
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "second.commit" -annotated $false

        # 0 = LightWeight, 1 = Annotated
        $expected = @( `
                        (0 , 'first.commit'                        ), `
                        (1 , 'first.commit.annotated.with.name'    ), `
                        (1 , 'first.commit.annotated.with.message' ), `
                        (1 , 'second.commit.annotated.with.message'), `
                        (1 , 'second.commit.annotated.with.name'   ), `
                        (0 , 'second.commit'                       ) `
                     )
                     
        GitNoob_AssertListTags -description 'TestListTags' `
                               -testdirectory $testrepository.testdirectory `
                               -expected $expected
                              
        
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

function TestListTagsMessage
{
    $testrepository = GitNoob_Repository_CreateEmpty
    try {
        GitNoob_CreateFile -testdirectory $testrepository.testdirectory -name 'newfile' -contents 'newfile'
        GitNoob_GitStageFile -testdirectory $testrepository.testdirectory -name 'newfile'
        GitNoob_GitCommit -testdirectory $testrepository.testdirectory -name 'first commit on master'

        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "first.commit.annotated.with.message" -annotated $true -message "This is a one line message"
        GitNoob_GitCreateTag -testdirectory $testrepository.testdirectory -name "first.commit.annotated.with.message1" -annotated $true -message "This is a three line message`nsecond line`nthird line"
        

        # 0 = LightWeight, 1 = Annotated
        $expected = @( `
                        (1 , 'first.commit.annotated.with.message' , "This is a one line message" ), `
                        (1 , 'first.commit.annotated.with.message1' , "This is a three line message`r`nsecond line`r`nthird line" ) `
                     )
                     
        GitNoob_AssertListTags -description 'TestListTagsMessage' `
                               -testdirectory $testrepository.testdirectory `
                               -expected $expected
                              
        
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
}

TestListTags
TestListTagsMessage

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
