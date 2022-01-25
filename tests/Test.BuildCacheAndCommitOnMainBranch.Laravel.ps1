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

    GitNoob_CreateFile -testdirectory $laravel.testdirectory `
                       -name $file `
                       -contents $contents
    try {
        GitNoob_GitStageFile -testdirectory $laravel.testdirectory -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'NothingWrong'
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Nothing wrong' `
                                                      -projectType $laravel `
                                                      -Updated $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'NothingWrong undo'
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

    GitNoob_CreateFile -testdirectory $laravel.testdirectory `
                       -name $file `
                       -contents $contents
    try {
        GitNoob_GitStageFile -testdirectory $laravel.testdirectory -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongNamespace'
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Wrong namespace' `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongNamespace undo'
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

    GitNoob_CreateFile -testdirectory $laravel.testdirectory `
                       -name $file `
                       -contents $contents
    try {
        GitNoob_GitStageFile -testdirectory $laravel.testdirectory -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongClassname'
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - Wrong classname' `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongClassname undo'
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

    GitNoob_CreateFile -testdirectory $laravel.testdirectory `
                       -name $file `
                       -contents $contents
    try {
        GitNoob_GitStageFile -testdirectory $laravel.testdirectory -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'WrongClassname'
        GitNoob_AssertBuildCacheAndCommitOnMainBranch -description 'Laravel - route with anonymous function' `
                                                      -projectType $laravel `
                                                      -ErrorBuildingCache $true
    } finally {
        GitNoob_GitRemoveAndStageFile -testdirectory $laravel.testdirectory `
                                      -name $file
        GitNoob_GitCommit -testdirectory $laravel.testdirectory -name 'RouteWithAnonymousFunction undo'
    }
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
