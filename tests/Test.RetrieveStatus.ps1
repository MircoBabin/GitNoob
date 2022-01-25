# include GitNoob.ps1
$GitNoob_ps1 = Join-Path -Path $PSScriptRoot  -ChildPath 'GitNoob.ps1'
. $GitNoob_ps1

# Add parm to launch Visual Studio Debugger.
#
#                            -LaunchDebugger $true `
#

# Set for viewing GIT output: 
#       $global:GitNoob_RunGitExe_Verbose = $true

function RunOnEmptyRepository
{
    $testdirectory = GitNoob_GetNonExistingTestDirectory
    
    GitNoob_AssertStatus -description 'Non existing directory' `
                         -testdirectory $testdirectory `
                         -DirectoryExists $false `
                         -IsGitRootDirectory $false `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $false `
                         -Rebasing $false `
                         -Merging $false

    $testdirectory = GitNoob_CreateTestDirectory -nonExisting $testdirectory
    
    GitNoob_AssertStatus -description '[Empty] Empty directory, git not initialized' `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $false `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $false `
                         -Rebasing $false `
                         -Merging $false
                 
    try {
        GitNoob_GitInit -testdirectory $testdirectory
        
        GitNoob_AssertStatus -description '[Empty] Empty directory, git initialized, no commits' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch 'master'
                             
        <#
         # Untracked
         #>
         
        TestUntracked -description '[Empty - no commits]' `
                      -testdirectory $testdirectory `
                      -name 'newfile'
        
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function RunOnFilledRepository
{
    $testdirectory = GitNoob_CreateTestDirectory
    
    try {
        GitNoob_GitInit -testdirectory $testdirectory
        
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile'
        GitNoob_CreateFile -testdirectory $testdirectory -name 'todelete' -contents 'todelete'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'todelete'
        GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit'    
        
        GitNoob_AssertStatus -description '[Filled] first commit' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        <#
         # Modified
         #>         
                             
        GitNoob_TouchFileTimestamps -testdirectory $testdirectory -name 'newfile'
        
        GitNoob_AssertStatus -description '[Filled] new file timestamps touched' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile modified'
                             
        GitNoob_AssertStatus -description '[Filled] new file modified' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile'
        
        GitNoob_AssertStatus -description '[Filled] new file modified staged' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_TouchFileTimestamps -testdirectory $testdirectory -name 'newfile'
        
        GitNoob_AssertStatus -description '[Filled] new file modified staged touched' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile modified staged modified'
                             
        GitNoob_AssertStatus -description '[Filled] new file modified staged modified' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_GitUnstageFile -testdirectory $testdirectory -name 'newfile' -isuntracked $false
                             
        GitNoob_AssertStatus -description '[Filled] new file modified unstaged' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile'
                             
        GitNoob_AssertStatus -description '[Filled] new file unmodified' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
        
        <#
         # Delete
         #>
        
        GitNoob_DeleteFile -testdirectory $testdirectory -name 'todelete'
                             
        GitNoob_AssertStatus -description '[Filled] todelete deleted' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'todelete'
        
        GitNoob_AssertStatus -description '[Filled] todelete deleted staged (add)' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_GitUnstageFile -testdirectory $testdirectory -name 'todelete' -isuntracked $false
                             
        GitNoob_AssertStatus -description '[Filled] todelete deleted unstaged (add)' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false 
                             
        GitNoob_CreateFile -testdirectory $testdirectory -name 'todelete' -contents 'todelete'
                             
        GitNoob_AssertStatus -description '[Filled] todelete undeleted unstaged (add)' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false 
                            
        GitNoob_GitRemoveAndStageFile -testdirectory $testdirectory -name 'todelete'
        
        GitNoob_AssertStatus -description '[Filled] todelete deleted staged (rm)' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_CreateFile -testdirectory $testdirectory -name 'todelete' -contents 'todelete'
        
        GitNoob_AssertStatus -description '[Filled] todelete deleted staged (rm) undeleted' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $true `
                             -HasStagedUncommittedFiles $true `
                             -Rebasing $false `
                             -Merging $false
                             
        GitNoob_GitUnstageFile -testdirectory $testdirectory -name 'todelete' -isuntracked $false
                             
        GitNoob_AssertStatus -description '[Filled] todelete undeleted unstaged (rm)' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
                             
        <#
         # Untracked
         #>
         
        TestUntracked -description '[Filled]' `
                      -testdirectory $testdirectory `
                      -name 'newfile1'
                             
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function TestUntracked
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_AssertStatus -description ($description + ' New file - start with no changes + no staged files') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $false `
                         -Rebasing $false `
                         -Merging $false
    
    GitNoob_CreateFile -testdirectory $testdirectory -name $name -contents 'newfile'
    
    GitNoob_AssertStatus -description ($description + ' New file - created') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $true `
                         -HasStagedUncommittedFiles $false `
                         -Rebasing $false `
                         -Merging $false
                 
    GitNoob_GitStageFile -testdirectory $testdirectory -name $name
    
    GitNoob_AssertStatus -description ($description + ' New file - staged') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false
    
    GitNoob_TouchFileTimestamps -testdirectory $testdirectory -name $name
    
    GitNoob_AssertStatus -description ($description + ' New file - staged - timestamps touched') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false
                 
    GitNoob_CreateFile -testdirectory $testdirectory -name $name -contents 'newfile modified'
    
    GitNoob_AssertStatus -description ($description + ' New file - staged - modified') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $true `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false
                 
    GitNoob_DeleteFile -testdirectory $testdirectory -name $name
    
    GitNoob_AssertStatus -description ($description + ' New file - staged - deleted') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $true `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false

    GitNoob_CreateFile -testdirectory $testdirectory -name $name -contents 'newfile'
    
    GitNoob_AssertStatus -description ($description + ' New file - staged - recreated') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false
                 
    GitNoob_GitUnstageFile -testdirectory $testdirectory -name $name -isuntracked $true
                 
    GitNoob_AssertStatus -description ($description + ' New file - unstaged') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $true `
                         -HasStagedUncommittedFiles $false `
                         -Rebasing $false `
                         -Merging $false
                 
    GitNoob_GitStageFile -testdirectory $testdirectory -name $name
    
    GitNoob_AssertStatus -description ($description + ' New file - staged again') `
                         -testdirectory $testdirectory `
                         -DirectoryExists $true `
                         -IsGitRootDirectory $true `
                         -HasWorkingTreeChanges $false `
                         -HasStagedUncommittedFiles $true `
                         -Rebasing $false `
                         -Merging $false
}


function RunOnDifferentBranches
{
    $testrepository = GitNoob_Repository_CreateTest
    try {
        GitNoob_AssertStatus -description 'Test repository - "master" branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'master'
                             
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700"
        GitNoob_AssertStatus -description 'Test repository - release2700 branch' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $false `
                             -CurrentBranch 'release2700'
                             
        GitNoob_GitCheckoutBranch -testdirectory $testrepository.testdirectory -name "release2700-base"
        GitNoob_AssertStatus -description 'Test repository - release2700-base detached head' `
                             -testdirectory $testrepository.testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -DetachedHead_NotOnBranch $true
                             
        
    } finally {
        GitNoob_DeleteTestDirectory -testdirectory $testrepository.testdirectory
    }
    
}

RunOnEmptyRepository
RunOnFilledRepository
RunOnDifferentBranches

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
