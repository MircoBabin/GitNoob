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

function NothingWrong
{
    param (
        [Parameter(Mandatory=$true)]$laravel
    )
    
    $file = 'app\Http\Controllers\NothingWrong.php';
    
    $contents = 
        '<?php' + "`r`n" + `
        'namespace App\Http\Controllers;' + "`r`n" + `
        'class NothingWrong extends Controller' + "`r`n" + `
        '{' + "`r`n" + `
        '}' + "`r`n"
            
    $testdirectory = GitNoob_GetNonExistingTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest (clone)' `
                                -testdirectory $testdirectory `
                                -remoteUrl $laravel.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false
                                
        GitNoob_CreateFile -testdirectory $testdirectory `
                           -name $file `
                           -contents $contents
                           
        GitNoob_GitStageFile -testdirectory $testdirectory -name $file
        GitNoob_GitCommit -testdirectory $testdirectory -name 'NothingWrong'
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master

        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Nothing wrong' `
                                                      -testdirectory $testdirectory `
                                                      -projectType $laravel `
                                                      -Updated $true
        GitNoob_AssertStatus -description 'Laravel - Nothing wrong' `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $false `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'NothingWrong undo'        
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
        
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function WrongNamespace
{
    param (
        [Parameter(Mandatory=$true)]$laravel
    )
    
    $file = 'app\Http\Controllers\WrongNamespace.php';
    
    $contents = 
        '<?php' + "`r`n" + `
        'namespace App\Http\WrongControllers;' + "`r`n" + `
        'class WrongNamespace extends Controller' + "`r`n" + `
        '{' + "`r`n" + `
        '}' + "`r`n"
            
    $testdirectory = GitNoob_GetNonExistingTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest (clone)' `
                                -testdirectory $testdirectory `
                                -remoteUrl $laravel.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false

        GitNoob_CreateFile -testdirectory $testdirectory `
                           -name $file `
                           -contents $contents
                           
        GitNoob_GitStageFile -testdirectory $testdirectory -name $file
        GitNoob_GitCommit -testdirectory $testdirectory -name 'WrongNamespace'
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
                              
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Wrong namespace' `
                                                      -testdirectory $testdirectory `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongNamespace undo'        
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
        
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function WrongClassname
{
    param (
        [Parameter(Mandatory=$true)]$laravel
    )
    
    $file = 'app\Http\Controllers\WrongClassname.php';
    
    $contents = 
        '<?php' + "`r`n" + `
        'namespace App\Http\Controllers;' + "`r`n" + `
        'class ReallyWrongClassname extends Controller' + "`r`n" + `
        '{' + "`r`n" + `
        '}' + "`r`n"

    $testdirectory = GitNoob_GetNonExistingTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest (clone)' `
                                -testdirectory $testdirectory `
                                -remoteUrl $laravel.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false

        GitNoob_CreateFile -testdirectory $testdirectory `
                           -name $file `
                           -contents $contents
                           
        GitNoob_GitStageFile -testdirectory $testdirectory -name $file
        GitNoob_GitCommit -testdirectory $testdirectory -name 'WrongClassname'
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
        
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Wrong classname' `
                                                      -testdirectory $testdirectory `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongClassname undo'        
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
        
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}

function RouteWithAnonymousFunction
{
    param (
        [Parameter(Mandatory=$true)]$laravel
    )
    
    $file = 'routes\api.php';
    
    $contents = 
        '<?php' + "`r`n" + `
        'use Illuminate\Http\Request;' + "`r`n" + `
        'use Illuminate\Support\Facades\Route;' + "`r`n" + `
        'Route::middleware(''auth:api'')->get(''/user'', function (Request $request) {' + "`r`n" + `
        '    return $request->user();' + "`r`n" + `
        '});' + "`r`n"

    $testdirectory = GitNoob_GetNonExistingTestDirectory
    try {
        GitNoob_AssertGetLatest -description 'Get latest (clone)' `
                                -testdirectory $testdirectory `
                                -remoteUrl $laravel.remoteUrl `
                                -remoteBranch master `
                                -Cloned $true `
                                -Updated $false
                                
        GitNoob_CreateFile -testdirectory $testdirectory `
                           -name $file `
                           -contents $contents
                           
        GitNoob_GitStageFile -testdirectory $testdirectory -name $file
        GitNoob_GitCommit -testdirectory $testdirectory -name 'WrongClassname'
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
        
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - route with anonymous function' `
                                                      -testdirectory $testdirectory `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'RouteWithAnonymousFunction undo'        
        GitNoob_GitPushBranch -testdirectory $testdirectory `
                              -remotename origin `
                              -name master
                              
        GitNoob_DeleteTestDirectory -testdirectory $testdirectory
    }
}


$laravel = GitNoob_CreatePhpLaravel9Directory
try {
    NothingWrong -laravel $laravel
    WrongNamespace -laravel $laravel
    WrongClassname -laravel $laravel
    
    # Laravel 9 can serialize a Closure - this test will fail on Laravel 9, because the build route cache will succeed.
    # RouteWithAnonymousFunction -laravel $laravel
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $laravel.testdirectory
}


$laravel = GitNoob_CreatePhpLaravel7Directory
try {
    NothingWrong -laravel $laravel
    WrongNamespace -laravel $laravel
    WrongClassname -laravel $laravel
    RouteWithAnonymousFunction -laravel $laravel
} finally {
    GitNoob_DeleteTestDirectory -testdirectory $laravel.testdirectory
}

# $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
