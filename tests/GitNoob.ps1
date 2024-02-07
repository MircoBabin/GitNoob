$GitNoob_Assets_Path = Join-Path -Path $PSScriptRoot -ChildPath '..\assets'
$GitNoob_Config_Dll = Join-Path -Path $PSScriptRoot -ChildPath '..\bin\Debug\GitNoob.Config.dll'
if (-Not (Test-Path $GitNoob_Config_Dll)) {
    Throw "GitNoob.Config.dll not found: " + $GitNoob_Config_Dll
}
[Reflection.Assembly]::LoadFile("$GitNoob_Config_Dll") | Out-Null

$GitNoob_Git_Dll = Join-Path -Path $PSScriptRoot  -ChildPath '..\bin\Debug\GitNoob.Git.dll'
if (-Not (Test-Path $GitNoob_Git_Dll)) {
    Throw "GitNoob.Git.dll not found: " + $GitNoob_Git_Dll
}
[Reflection.Assembly]::LoadFile("$GitNoob_Git_Dll") | Out-Null

$GitNoob_Utils_Dll = Join-Path -Path $PSScriptRoot  -ChildPath '..\bin\Debug\GitNoob.Utils.dll'
if (-Not (Test-Path $GitNoob_Utils_Dll)) {
    Throw "GitNoob.Utils.dll not found: " + $GitNoob_Utils_Dll
}
[Reflection.Assembly]::LoadFile("$GitNoob_Utils_Dll") | Out-Null

$GitNoob_GitResult_Dll = Join-Path -Path $PSScriptRoot  -ChildPath '..\bin\Debug\GitNoob.GitResult.dll'
if (-Not (Test-Path $GitNoob_GitResult_Dll)) {
    Throw "GitNoob.GitResult.dll not found: " + $GitNoob_GitResult_Dll
}
[Reflection.Assembly]::LoadFile("$GitNoob_GitResult_Dll") | Out-Null

$GitNoob_Gui_Program_Dll = Join-Path -Path $PSScriptRoot  -ChildPath '..\bin\Debug\GitNoob.Gui.Program.dll'
if (-Not (Test-Path $GitNoob_Gui_Program_Dll)) {
    Throw "GitNoob.Gui.Program.dll not found: " + $GitNoob_Gui_Program_Dll
}
[Reflection.Assembly]::LoadFile("$GitNoob_Gui_Program_Dll") | Out-Null

$GitNoob_ProjectTypes_Dll = Join-Path -Path $PSScriptRoot  -ChildPath '..\bin\Debug\GitNoob.ProjectTypes.dll'
[GitNoob.Config.Loader.ProjectTypeLoader]::LoadProjectTypesAssembly("$GitNoob_ProjectTypes_Dll") | Out-Null

$GitNoob_Git_Exe = (Get-Command 'git.exe').Path;
[GitNoob.Git.GitWorkingDirectory]::setGitExecutable($GitNoob_Git_Exe)

$GitNoob_Utf8NoBom_Encoding = New-Object System.Text.UTF8Encoding $False

<#
 # Duration measurement
 #
 # $global:GitNoob_StopWatched | Select-Object -Property Depth, ElapsedMilliseconds, Description | Format-Table
 #>

$global:GitNoob_StopWatchDepth = 0
$global:GitNoob_StopWatched = @()
class GitNoob_StopWatch
{
    [int] $Depth
    [string] $Description
    [long] $ElapsedMilliseconds
    [System.Diagnostics.Stopwatch] $stopwatch
    
    GitNoob_StopWatch() {
        $stack = Get-PSCallStack
        #index 0      = current function GitNoob_StopWatch::GitNoob_StopWatch
        #index 1      = caller
        $this.Description = $stack[1].FunctionName
        
        $global:GitNoob_StopWatchDepth = $global:GitNoob_StopWatchDepth + 1
        $this.Depth = $global:GitNoob_StopWatchDepth
        
        $global:GitNoob_StopWatched += $this
        
        $this.stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    }

    GitNoob_StopWatch([string] $description) {
        $stack = Get-PSCallStack
        #index 0      = current function GitNoob_StopWatch::GitNoob_StopWatch
        #index 1      = caller
        $this.Description = $stack[1].FunctionName + ': ' + $description
        
        $global:GitNoob_StopWatchDepth = $global:GitNoob_StopWatchDepth + 1
        $this.Depth = $global:GitNoob_StopWatchDepth
        
        $global:GitNoob_StopWatched += $this
        
        $this.stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    }
    
    [void]stop() {
        $this.stopwatch.Stop()
        $this.ElapsedMilliseconds = $this.stopwatch.ElapsedMilliseconds
        $global:GitNoob_StopWatchDepth = $global:GitNoob_StopWatchDepth - 1
    }
}

<#
 # Basic testrepositories
 #>

function GitNoob_Repository_MakeBare
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    $dotGitDirectory = Join-Path -Path $testdirectory.path -ChildPath '.git'
    $tmpDestination = Join-Path -Path $testdirectory.parent -ChildPath ($testdirectory.name + '.git');
    Move-Item $dotGitDirectory $tmpDestination
    Set-ItemProperty -Path $tmpDestination -Name Attributes -Value Normal    
    
    Remove-Item $testdirectory.path -Force -Recurse
    
    Move-Item $tmpDestination $testdirectory.path
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('config --bool core.bare true')
}

function GitNoob_Repository_CreateEmpty
{
    param (
        [Parameter(Mandatory=$false)]$upstreamRepository = $null
    )
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $testdirectory = GitNoob_CreateTestDirectory
        GitNoob_GitInit -testdirectory $testdirectory
        if ($upstreamRepository -ne $null)
        {
            GitNoob_GitAddRemote -testdirectory $testdirectory -name "origin" -url $upstreamRepository.remoteUrl
        }
        
        # remote via "git remote add origin c:\...\.git"
        $url = Join-Path $testdirectory.path '.git'
    
        return [pscustomobject]@{
            testdirectory = $testdirectory
            remoteUrl = $url
        }
    } finally { $stopwatch.stop() }
 }

 function GitNoob_Repository_CreateTest
 {
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $testdirectory = GitNoob_CreateTestDirectory
        GitNoob_GitInit -testdirectory $testdirectory
        
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile'
        GitNoob_CreateFile -testdirectory $testdirectory -name 'todelete' -contents 'todelete'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'todelete'
        GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit on master'
        # On master (first commit):
        # - newfile
        # - todelete

        GitNoob_GitCreateBranch -testdirectory $testdirectory -name "unimportant"

        GitNoob_GitCreateTag -testdirectory $testdirectory -name "release2700-base" -annotated $true
        
            GitNoob_GitCreateBranch -testdirectory $testdirectory -name "release2700"
            GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name "release2700"
            # On release2700 branch, same as "On master (first commit)"

            GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile2-release2700' -contents 'newfile2-release2700'
            GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile2-release2700'
            GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit on release2700'    
            # On release2700 (first commit)
            # - newfile
            # - todelete
            # - newfile2-release

            GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile modified on release2700 branch'
            GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile'
            GitNoob_GitCommit -testdirectory $testdirectory -name 'second commit on release2700'    
            # On release2700 (second commit)
            # - newfile modified 
            # - todelete
            # - newfile2-release
             
            GitNoob_CreateFile -testdirectory $testdirectory -name 'todelete' -contents 'todelete modified on release2700 branch'
            GitNoob_GitStageFile -testdirectory $testdirectory -name 'todelete'
            GitNoob_GitCommit -testdirectory $testdirectory -name 'third commit on release2700'    
            # On release2700 (third commit)
            # - newfile modified 
            # - todelete modified
            # - newfile2-release
        
        GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name "master"
        
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile' -contents 'newfile modified on master branch'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile'
        GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile1' -contents 'newfile1'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile1'
        GitNoob_GitCommit -testdirectory $testdirectory -name 'second commit on master'    
        # On master (second commit):
        # - newfile modified
        # - newfile1
        # - todelete
        
        GitNoob_GitCreateTag -testdirectory $testdirectory -name "test-base" -annotated $true
        
            GitNoob_GitCreateBranch -testdirectory $testdirectory -name "test"
            GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name "test"
            # On test branch, same as "On master (second commit)"

            GitNoob_CreateFile -testdirectory $testdirectory -name 'newfile2-test' -contents 'newfile2-test'
            GitNoob_GitStageFile -testdirectory $testdirectory -name 'newfile2-test'
            GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit on test'    
            # On test (first commit)
        
        GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name "master"
        
        GitNoob_DeleteFile -testdirectory $testdirectory -name 'todelete'
        GitNoob_GitStageFile -testdirectory $testdirectory -name 'todelete'
        GitNoob_GitCommit -testdirectory $testdirectory -name 'third commit on master'    
        # On master (third commit):
        # - newfile modified
        # - newfile1
        # (todelete is deleted)
        
        
        # remote via "git remote add origin c:\...\.git"
        $url = Join-Path $testdirectory.path '.git'
        
        return [pscustomobject]@{
            testdirectory = $testdirectory
            remoteUrl = $url
        }
    } finally { $stopwatch.stop() }
 }

 function GitNoob_Repository_UpdateTest
 {
    param (
        [Parameter(Mandatory=$true)]$repository,
        [Parameter(Mandatory=$false)]$name = 'no name'
    )
    
    $contents = (GitNoob_GetRandomName + ': ' + $name)
    
    GitNoob_GitCheckoutBranch -testdirectory $repository.testdirectory -name "release2700"

    GitNoob_CreateFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest' -contents ('[release2700]' + $contents)
    GitNoob_GitStageFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest'
    GitNoob_GitCommit -testdirectory $repository.testdirectory -name ('GitNoob_Repository_UpdateTest: ' + $name)   

    GitNoob_GitCheckoutBranch -testdirectory $repository.testdirectory -name "test"

    GitNoob_CreateFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest' -contents ('[test]' + $contents)
    GitNoob_GitStageFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest'
    GitNoob_GitCommit -testdirectory $repository.testdirectory -name ('GitNoob_Repository_UpdateTest: ' + $name)   
    
    GitNoob_GitCheckoutBranch -testdirectory $repository.testdirectory -name "master"

    GitNoob_CreateFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest' -contents ('[master]' + $contents)
    GitNoob_GitStageFile -testdirectory $repository.testdirectory -name 'GitNoob_Repository_UpdateTest'
    GitNoob_GitCommit -testdirectory $repository.testdirectory -name ('GitNoob_Repository_UpdateTest: ' + $name)   
    
    # ends with "master" branch checked out
}
 
<#
 # Directories & files
 #>

function GitNoob_GetRandomName
{
    $now = Get-Date
    $random = Get-Random
    $name = $now.Year.ToString('0000') + '-' + $now.Month.ToString('00') + '-' + $now.Day.ToString('00') + '.' + `
            $now.Hour.ToString('00') + '_' + $now.Minute.ToString('00') + '_' + $now.Second.ToString('00') + '_' + $now.Millisecond + '.' + `
            $random

    return $name    
}

function GitNoob_GetRootDirectoryForTesting
{
    $parent = [System.IO.Path]::GetTempPath()
    $parent = Join-Path $parent 'GitNoob.tests'

    if(!(test-path $parent))
    {
        New-Item -ItemType Directory -Force -Path $parent | Out-Null  
    } else {
        $parentGit = Join-Path $parent '.git'
        if(test-path $parentGit) { 
            Remove-Item $parentGit -Force -Recurse
        }
    }
    
    return $parent;
}

function GitNoob_GetNonExistingTestDirectory
{
    $parent = GitNoob_GetRootDirectoryForTesting
    
    [string] $name = '';
    $path = "";
    while($true)
    {
        $name = GitNoob_GetRandomName
        $path = Join-Path $parent $name
        if(!(test-path $path)) { break }
    }
    
    return [pscustomobject]@{
        parent = $parent
        name = $name
        path = $path
    }
}

function GitNoob_CreateTempfile
{
    param (
        [Parameter(Mandatory=$false)]$contents = $null,
        [Parameter(Mandatory=$false)]$encoding = $null
    )
    
    $parent = GitNoob_GetRootDirectoryForTesting

    [string] $name = '';
    $filename = "";
    while($true)
    {
        $name = GitNoob_GetRandomName
        $filename = Join-Path $parent $name
        if(!(test-path $filename)) { break }
    }

    if ($contents -ne $null)
    {
        if ($encoding -ne $null)
        {
            $contents | Out-File -FilePath $filename -Encoding $encoding
        }
        else
        {
            $contents | Out-File -FilePath $filename
        }
    }
    else
    {
        New-Item -ItemType File -Path $filename | Out-Null
    }

    return $filename
}

function GitNoob_CreatePhpLaravel7Directory
{
    param (
        $nonExisting = $null
    )

    if ($nonExisting -eq $null) { $nonExisting = GitNoob_GetNonExistingTestDirectory }
    
    New-Item -ItemType Directory -Force -Path $nonExisting.path
    
    $fromPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-laravel7\src\*'
    Copy-Item -Path $fromPath -Destination $nonExisting.path -recurse -Force
    
    $fromPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-composer\composer.phar';
    Copy-Item -Path $fromPath -Destination $nonExisting.path -Force
    
    $phpPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-bin-7.2.31'
    $phpIniTemplateFile = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-laravel7\php.ini'
    
    $testdirectory =  [pscustomobject]@{
        parent = $nonExisting.parent
        name = $nonExisting.name
        path = $nonExisting.path
    }
    
    GitNoob_GitInit -testdirectory $testdirectory
    GitNoob_GitStageAll -testdirectory $testdirectory
    GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit laravel7'

    # Make repository *bare* to make sure "git push origin master" works.
    #
    # remote: error: refusing to update checked out branch: refs/heads/master
    # remote: error: By default, updating the current branch in a non-bare repository 
    # remote: is denied, because it will make the index and work tree inconsistent    
    # remote: with what you pushed, and will require 'git reset --hard' to match      
    # remote: the work tree to HEAD.
    # remote:
    # remote: You can set the 'receive.denyCurrentBranch' configuration variable      
    # remote: to 'ignore' or 'warn' in the remote repository to allow pushing into    
    # remote: its current branch; however, this is not recommended unless you
    # remote: arranged to update its work tree to match what you pushed in some       
    # remote: other way.
    GitNoob_Repository_MakeBare -testdirectory $testdirectory
    
    return [pscustomobject]@{
        testdirectory = $testdirectory
        remoteUrl = $testdirectory.path
        
        projectType = 'PhpLaravel7'
        phpPath = $phpPath
        phpIniTemplateFile = $phpIniTemplateFile
    }
}

function GitNoob_CreatePhpLaravel9Directory
{
    param (
        $nonExisting = $null
    )

    if ($nonExisting -eq $null) { $nonExisting = GitNoob_GetNonExistingTestDirectory }
    
    New-Item -ItemType Directory -Force -Path $nonExisting.path
    
    $fromPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-laravel9\src\*'
    Copy-Item -Path $fromPath -Destination $nonExisting.path -recurse -Force
    
    $fromPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-composer\composer.phar';
    Copy-Item -Path $fromPath -Destination $nonExisting.path -Force
    
    $phpPath = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-bin-8.1.4-x64'
    $phpIniTemplateFile = Join-Path -Path $GitNoob_Assets_Path -ChildPath 'php-laravel9\php.ini'
    
    $testdirectory =  [pscustomobject]@{
        parent = $nonExisting.parent
        name = $nonExisting.name
        path = $nonExisting.path
    }
    
    GitNoob_GitInit -testdirectory $testdirectory
    GitNoob_GitStageAll -testdirectory $testdirectory
    GitNoob_GitCommit -testdirectory $testdirectory -name 'first commit laravel9'

    # Make repository *bare* to make sure "git push origin master" works.
    #
    # remote: error: refusing to update checked out branch: refs/heads/master
    # remote: error: By default, updating the current branch in a non-bare repository 
    # remote: is denied, because it will make the index and work tree inconsistent    
    # remote: with what you pushed, and will require 'git reset --hard' to match      
    # remote: the work tree to HEAD.
    # remote:
    # remote: You can set the 'receive.denyCurrentBranch' configuration variable      
    # remote: to 'ignore' or 'warn' in the remote repository to allow pushing into    
    # remote: its current branch; however, this is not recommended unless you
    # remote: arranged to update its work tree to match what you pushed in some       
    # remote: other way.
    GitNoob_Repository_MakeBare -testdirectory $testdirectory
    
    return [pscustomobject]@{
        testdirectory = $testdirectory
        remoteUrl = $testdirectory.path
        
        projectType = 'PhpLaravel9'
        phpPath = $phpPath
        phpIniTemplateFile = $phpIniTemplateFile
    }
}

function GitNoob_CreateTestDirectory
{
    param (
        $nonExisting = $null
    )

    if ($nonExisting -eq $null) { $nonExisting = GitNoob_GetNonExistingTestDirectory }
    
    New-Item -ItemType Directory -Force -Path $nonExisting.path
    
    return [pscustomobject]@{
        parent = $nonExisting.parent
        name = $nonExisting.name
        path = $nonExisting.path
    }
}

function GitNoob_CompareDirectories
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$upstreamTestdirectory, 
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $t1 = Get-ChildItem -Path $upstreamTestdirectory.path -Recurse | ForEach-Object { Get-FileHash -Path $_.FullName }
        $t2 = Get-ChildItem -Path $testdirectory.path -Recurse | ForEach-Object { Get-FileHash -Path $_.FullName }

        if ($t1 -eq $null) {
            if ($t2 -eq $null) {
                return $true;
            }
        }
        if ($t2 -eq $null) {
            return $false;
        }

        $diff = Compare-Object -ReferenceObject $t1 -DifferenceObject $t2 -Property hash -PassThru | Measure
        if ($diff.count -ne 0) {
            return $false;
        }
        
        return $true;
    } finally { $stopwatch.stop() }
}

function GitNoob_DeleteTestDirectory
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )

    if(test-path $testdirectory.path) { 
        Remove-Item $testdirectory.path -Force -Recurse
    }
}

function GitNoob_CreateFile
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name,
        [Parameter(Mandatory=$true)]$contents
    )
    
    $filename = Join-Path $testdirectory.path $name
    Set-Content -NoNewline -Path $filename -Value $contents
}

function GitNoob_TouchFileTimestamps
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    $filename = Join-Path $testdirectory.path $name
    $file = Get-Item $filename
    
    $time = (Get-Date).AddSeconds(-30)
    $file.CreationTime = $time
    $file.LastWriteTime = $time
    $file.LastAccessTime = $time
}

function GitNoob_DeleteFile
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    $filename = Join-Path $testdirectory.path $name
    Remove-Item -Path $filename
}

<#
 # GIT
 #>

$global:GitNoob_RunGitExe_Verbose = $false
function GitNoob_RunGitExe
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$commandline,
        [Parameter(Mandatory=$false)]$returnStdout = $false,
        [Parameter(Mandatory=$false)]$aSynchronous = $false
    )
    
    if ($returnStdout -eq $true -And $aSynchronous -eq $true)
    {
        throw "returnStdout is true implies that aSynchronous must not be true"
    }
    
    $verbose = $global:GitNoob_RunGitExe_Verbose

    $stopwatch = $null
    if ($aSynchronous -ne $true)
    {
        $stopwatch = [GitNoob_Stopwatch]::new($commandline)
    }
    else
    {
        $stopwatch = [GitNoob_Stopwatch]::new('[aSynchronous] ' + $commandline)
    }
    try {
        if ($verbose -eq $true)
        {
            if ($aSynchronous -ne $true)
            {
                Write-Host "--- GIT COMMAND: BEGIN ---"
                Write-Host ($commandline | Out-String)
            }
            else
            {
                Write-Host "--- [aSynchronous] GIT COMMAND: BEGIN ---"
            }
            
            $env:GIT_TRACE='true'
            $env:GIT_TRACE_PERFORMANCE='true'
        }
        else
        {
            if (Test-Path env:\GIT_TRACE) { Remove-Item env:\GIT_TRACE }
            if (Test-Path env:\GIT_TRACE_PERFORMANCE) { Remove-Item env:\GIT_TRACE_PERFORMANCE }
        }
        
        $stdoutFilename = GitNoob_CreateTempfile
        $stderrFilename = GitNoob_CreateTempfile
        
        $stdoutOutput = $null
            
        if ($aSynchronous -ne $true)
        {
            try
            {
                # Don't use Start-Process -Wait because that is much much much much much much much much much much (10x) slower !
                $proc = Start-Process -FilePath $global:GitNoob_Git_Exe `
                                      -ArgumentList $commandline `
                                      -WorkingDirectory $testdirectory.path `
                                      -NoNewWindow `
                                      -RedirectStandardOutput $stdoutFilename `
                                      -RedirectStandardError $stderrFilename `
                                      -PassThru
                $proc.WaitForExit()
                
                $stdoutOutput = Get-Content -Raw -Path $stdoutFilename
                                  
                if ($verbose -eq $true)
                {
                    Write-Host ($stdoutOutput | Out-String)
                    $text = Get-Content -Raw -Path $stderrFilename
                    Write-Host ($text | Out-String)
                    Write-Host "--- GIT COMMAND: END ---"
                }
                
                if ($returnStdout -eq $true) {
                    return $stdoutOutput
                }
            }
            finally
            {
                Remove-Item -Path $stdoutFilename
                Remove-Item -Path $stderrFilename
            }
        }
        else
        {
            $waitfile = GitNoob_CreateTempfile -contents 'noeditor wait file'
            $batfile = $waitfile + '.noeditor.bat'
            $activefile = $waitfile + '.noeditor.active'
            $endfile = $waitfile + '.noeditor.ended'
            
            $noeditor = '@echo off' + "`r`n" + `
                        'echo %1>"' + $activefile + '"' + "`r`n" + `
                        ':loop' + "`r`n" + `
                        '    if not exist "' + $waitfile + '" goto :end' + "`r`n" + `
                        '    PING -n 1 127.0.0.1 >NUL 2>&1 || PING -n 1 ::1 >NUL 2>&1' + "`r`n" + `
                        'goto loop' + "`r`n" + `
                        ':end' + "`r`n" + `
                        'echo %1>"' + $endfile + '"' + "`r`n"
                        
            $noeditor | Out-File -FilePath $batfile -Encoding ascii

            GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('config sequence.editor "' + $batfile.replace('\', '/') + '"')
            
            if ($verbose -eq $true)
            {
                Write-Host ($commandline | Out-String)
            }
            
            $proc = Start-Process -FilePath $global:GitNoob_Git_Exe `
                                  -ArgumentList $commandline `
                                  -WorkingDirectory $testdirectory.path `
                                  -NoNewWindow `
                                  -RedirectStandardOutput $stdoutFilename `
                                  -RedirectStandardError $stderrFilename `
                                  -PassThru
            
            if ($verbose -eq $true)
            {
                Write-Host "--- [/aSynchronous] GIT COMMAND: BEGIN ---"
            }
                                
            return [pscustomobject]@{
                proc = $proc
                stdoutFilename = $stdoutFilename
                stderrFilename = $stderrFilename
                verbose = $verbose
                commandline = $commandline
                
                noeditor_waitfile = $waitfile
                noeditor_batfile = $batfile
                noeditor_activefile = $activefile
                noeditor_endfile = $endfile
            }
        }
    } finally { $stopwatch.stop() }
}

function GitNoob_RunGitExe_aSynchronousWaitForExit
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$async
    )
    
    if (test-path $async.noeditor_waitfile) 
    {
        Remove-Item -Path $async.noeditor_waitfile
    }
    
    if (test-path $async.noeditor_activefile) 
    {
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        while ($true)
        {
            if (test-path $async.noeditor_endfile) { break }
            if ($stopwatch.ElapsedMilliseconds -ge 15000) { break } #fail after 15 seconds
            
            Start-Sleep -Milliseconds 250
        }
    }

    if (test-path $async.noeditor_activefile) 
    {
        Remove-Item -Path $async.noeditor_activefile
    }
        
    if (test-path $async.noeditor_endfile) 
    {
        Remove-Item -Path $async.noeditor_endfile
    }
    
    if (test-path $async.noeditor_batfile) 
    {
        Remove-Item -Path $async.noeditor_batfile
    }
    
    $async.proc.WaitForExit(15000) | Out-Null #fail after 15 seconds
    
    if ($async.verbose -eq $true)
    {
        Write-Host "[aSynchronous] --- GIT COMMAND: END ---"
        Write-Host ($async.commandline | Out-String)
        
        $text = Get-Content -Raw -Path $async.stdoutFilename
        Write-Host ($text | Out-String)
        $text = Get-Content -Raw -Path $async.stderrFilename
        Write-Host ($text | Out-String)
        Write-Host "[/aSynchronous] --- GIT COMMAND: END ---"
    }
                
    Remove-Item -Path $async.stdoutFilename
    Remove-Item -Path $async.stderrFilename
}
    
function GitNoob_GitInit
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline 'init --initial-branch=master'

    $expect = Join-Path $testdirectory.path '.git'    
    if(!(test-path $expect)) 
    { 
        throw 'git init failed'
    }

    GitNoob_RunGitExe -testdirectory $testdirectory -commandline 'config user.name "GitNoob Test"'
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline 'config user.email "test@gitnoob.world.nl"'
}

function GitNoob_GitRemoveWorkingTreeChanges
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('reset --hard')
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('clean --force -d --quiet')
}  

function GitNoob_GitAddRemote
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name,
        [Parameter(Mandatory=$true)]$url
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('remote add "' + $name + '" "' + $url + '"')
}

function GitNoob_GitStageFile
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('add "' + $name + '"')
}

function GitNoob_GitStageAll
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('add -A')
}

function GitNoob_GitRemoveAndStageFile
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('rm "' + $name + '"')
}

function GitNoob_GitUnstageFile
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name,
        [Parameter(Mandatory=$true)]$isuntracked
    )
    
    $filename = Join-Path $testdirectory.path $name
    $existsBefore = test-path $filename
    $sizeBefore = 0
    $hashBefore = ''
    if ($existsBefore) 
    { 
        $sizeBefore = (Get-Item $filename).length 
        $hashBefore = (Get-FileHash $filename).Hash
    }
    
    if ($isuntracked -eq $true)
    {
        GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('rm --cached "' + $name + '"')
    }
    else
    {
        GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('reset -- "' + $name + '"')
    }
    
    $existsAfter = test-path $filename
    $sizeAfter = 0;
    $hashAfter = '';
    if ($existsAfter) 
    { 
        $sizeAfter = (Get-Item $filename).length 
        $hashAfter = (Get-FileHash $filename).Hash
    }
    
    if ($existsBefore -ne $existsAfter -Or $sizeBefore -ne $sizeAfter -Or $hashBefore -ne $hashAfter)
    {
        throw 'git unstage file failed, working directory was modified'
    }
}

function GitNoob_GitCommit
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('commit --author="GitNoob tests<tests@gitnoob.nowhere>" --message="' + $name + '"')
}

function GitNoob_GitGetLastCommitId
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    $output = GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('rev-parse --verify HEAD') -returnStdout $true
    return $output.Trim()
}

function GitNoob_GitCreateBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('branch "' + $name + '"')
}

function GitNoob_GitCheckoutBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('checkout "' + $name + '"')
}

function GitNoob_GitRebaseCurrentBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$ontoBranch,
        [Parameter(Mandatory=$false)]$interactive = $false
    )
    
    if ($interactive -ne $true)
    {
        GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('rebase "' + $ontoBranch + '"')
    }
    else
    {
        $async = GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('rebase --interactive "' + $ontoBranch + '"') -aSynchronous $true

        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        while ($true)
        {
            if (test-path $async.noeditor_activefile) { break }
            if ($stopwatch.ElapsedMilliseconds -ge 30000) { break } #fail after 30 seconds
            
            Start-Sleep -Milliseconds 250
        }
        
        return $async
    }
}

function GitNoob_GitMergeIntoCurrentBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$fromBranch
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('merge "' + $fromBranch + '"')
}

function GitNoob_GitCherryPickIntoCurrentBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$fromCommitId
    )

    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('cherry-pick "' + $fromCommitId + '"')
}

function GitNoob_GitPushBranch
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$remotename,
        [Parameter(Mandatory=$true)]$name
    )

    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('push "' + $remotename + '" "' + $name + '"')
}

function GitNoob_GitCreateTag
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name,
        [Parameter(Mandatory=$true)]$annotated,
        [Parameter(Mandatory=$false)]$message = $null
    )
    
    if ($annotated -eq $true)
    {   
        if ($message -eq $null)
        {
            GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('tag --annotate --no-sign --force "' + $name + '" "--message=' + $name + '"')
        }
        else
        {
            # $message via tempfile.
            $tempfilename = GitNoob_CreateTempfile -contents $message -encoding 'ascii'
            try
            {
                GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('tag --annotate --no-sign --force "' + $name + '" "--file=' + $tempfilename + '"')
            }
            finally
            {
                Remove-Item -Path $tempfilename
            }
        }
    }
    else
    {
        GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('tag --force "' + $name + '"')
    }
}

function GitNoob_GitStash
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('stash --include-untracked')
}

function GitNoob_GitStashPop
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory
    )
    
    GitNoob_RunGitExe -testdirectory $testdirectory -commandline ('stash pop')
}

<#
 # GitNoob.Git.GitWorkingDirectory
 #>
function GitNoob_LaunchDebugger
{
    param (
        [Parameter(Mandatory=$true)]$LaunchDebugger
    )
    
    if ($LaunchDebugger -eq $true)
    {
        Write-Host "`nlaunching debugger"
        Write-Host ($testdirectory.path | Format-List | Out-String)
        
        [GitNoob.Git.GitWorkingDirectory]::LaunchDebugger()
        GitNoob_PressENTER
    }
}

function GitNoob_PressENTERIfDebuggerLaunched
{
    param (
        [Parameter(Mandatory=$true)]$LaunchDebugger
    )
    
    if ($LaunchDebugger -eq $true)
    {
        GitNoob_PressENTER
    }
}

function GitNoob_PressENTER
{
    Read-Host "Press ENTER to continue"
}

function GitNoob_AssertGetCallingFunctionName
{
    $stack = Get-PSCallStack
    #index 0      = current function GitNoob_AssertGetCallingFunctionName
    #index 1      = assert function GitNoob_Assert.... function
    #index 2      = caller
    
    return $stack[2].FunctionName 
}

function GitNoob_Git_New_GitWorkingDirectory
{
    param (
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        $remoteUrl = $null,
        $remoteBranch = $null,
        $projectType = $null,
        $phpPath = $null,
        $phpIniTemplateFilename = $null
    )

    $config = new-object GitNoob.Config.WorkingDirectory
    $config.Name = 'GitNoob test'
    $config.Path.SetToString($testdirectory.path)
    if ($remoteUrl -ne $null) {
        $config.Git.RemoteUrl = $remoteUrl
    }
    if ($remoteBranch -ne $null) {
        # todo rename parameter $remoteBranch to $mainBranch
        $config.Git.MainBranch = $remoteBranch
    }
    if ($projectType -ne $null) {
        $config.ProjectType = [GitNoob.Config.Loader.ProjectTypeLoader]::Load($projectType)
    }
    if ($phpPath -ne $null) {
        $config.Php.Path.SetToString($phpPath)
    }
    if ($phpIniTemplateFilename -ne $null) {
        $config.Php.PhpIniTemplateFilename.SetToString($phpIniTemplateFilename);
    }

    [GitNoob.Git.GitWorkingDirectory]::ClearCache()
    
    return new-object GitNoob.Git.GitWorkingDirectory($config)
}

function GitNoob_AssertStatus
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$DirectoryExists,
        [Parameter(Mandatory=$true)]$IsGitRootDirectory,
        [Parameter(Mandatory=$false)]$HasWorkingTreeChanges = $null,
        [Parameter(Mandatory=$true)]$HasStagedUncommittedFiles,
        [Parameter(Mandatory=$true)]$Rebasing,
        [Parameter(Mandatory=$true)]$Merging,
        [Parameter(Mandatory=$false)]$DetachedHead_NotOnBranch = $null,
        [Parameter(Mandatory=$false)]$CurrentBranch = $null
    )
    
    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $status = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        $status = $GitWorkingDirectory.RetrieveStatus()
        
        if ($status.IsGitRootDirectory -ne $IsGitRootDirectory)
        {
            $valid = $false
            $message = $message + 'IsGitRootDirectory = ' + $status.IsGitRootDirectory + ', should be ' + $IsGitRootDirectory + "`n"
        }
        
        if ($HasWorkingTreeChanges -ne $null -And $status.HasWorkingTreeChanges -ne $HasWorkingTreeChanges)
        {
            $valid = $false
            $message = $message + 'HasWorkingTreeChanges = ' + $status.HasWorkingTreeChanges + ', should be ' + $HasWorkingTreeChanges + "`n"
        }
        
        if ($status.HasStagedUncommittedFiles -ne $HasStagedUncommittedFiles)
        {
            $valid = $false
            $message = $message + 'HasStagedUncommittedFiles = ' + $status.HasStagedUncommittedFiles + ', should be ' + $HasStagedUncommittedFiles + "`n"
        }
        
        if ($status.Rebasing -ne $Rebasing)
        {
            $valid = $false
            $message = $message + 'Rebasing = ' + $status.Rebasing + ', should be ' + $Rebasing + "`n"
        }
        
        if ($status.Merging -ne $Merging)
        {
            $valid = $false
            $message = $message + 'Merging = ' + $status.Merging + ', should be ' + $Merging + "`n"
        }
        
        if ($DetachedHead_NotOnBranch -ne $null -And $status.DetachedHead_NotOnBranch -ne $DetachedHead_NotOnBranch)
        {
            $valid = $false
            $message = $message + 'DetachedHead_NotOnBranch = ' + $status.DetachedHead_NotOnBranch + ', should be ' + $DetachedHead_NotOnBranch + "`n"
        }
        
        if ($CurrentBranch -ne $null -And $status.CurrentBranch -ne $CurrentBranch)
        {
            $valid = $false
            $message = $message + 'CurrentBranch = ' + $status.CurrentBranch + ', should be ' + $CurrentBranch + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertGetStatus !!!"
        Write-Host $message
        Write-Host ($status | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}

function GitNoob_AssertGetLatest
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$false)]$upstreamTestdirectory = $null,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$remoteUrl = $null,
        [Parameter(Mandatory=$false)]$remoteBranch = $null,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$Cloned,
        [Parameter(Mandatory=$true)]$Updated,
        [Parameter(Mandatory=$false)]$AssertStatus = $false,
        [Parameter(Mandatory=$false)]$StashBeforeCompare = $null,
        [Parameter(Mandatory=$false)]$ErrorNonEmptyAndNotAGitRepository = $false,
        [Parameter(Mandatory=$false)]$ErrorStagedUncommittedFiles = $false,
        [Parameter(Mandatory=$false)]$ErrorUnpushedCommitsOnMainBranch = $false,
        [Parameter(Mandatory=$false)]$ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch = $false,
        [Parameter(Mandatory=$false)]$ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory -remoteUrl $remoteUrl -remoteBranch $remoteBranch
        
        #GetLatest() 
        #- updates all remote branches (fast-forward only), e.g. 'master', 'release2700', ... - Only check one "major" branch
        #- also update tags from remote - no check          
        $result = $GitWorkingDirectory.GetLatest($false) # don't check for GitCredentialsViaKeePassCommander
        
        if ($result.Cloned -eq $true)
        {
            GitNoob_RunGitExe -testdirectory $testdirectory -commandline 'config user.name "GitNoob Test"'
            GitNoob_RunGitExe -testdirectory $testdirectory -commandline 'config user.email "test@gitnoob.world.nl"'
        }

        if ($AssertStatus -eq $true)    
        {
            GitNoob_AssertStatus -description ('[' + (GitNoob_AssertGetCallingFunctionName) + '] GetLatest status: ' + $description) `
                                 -testdirectory $testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $false `
                                 -HasStagedUncommittedFiles $false `
                                 -Rebasing $false `
                                 -Merging $false
        }
                             
        if ($upstreamTestdirectory -ne $null -And ($result.Cloned -eq $true -Or $result.Updated -eq $true)) 
        {
            if ($StashBeforeCompare -ne $null)
            {
                GitNoob_GitStash -testdirectory $testdirectory
            }
            
            try
            {
                GitNoob_GitCheckoutBranch -testdirectory $upstreamTestdirectory -name $remoteBranch
                GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name $remoteBranch
                
                if ((GitNoob_CompareDirectories -upstreamTestdirectory $upstreamTestdirectory -testdirectory $testdirectory) -ne $true)
                {
                    $valid = $false
                    $message = $message + "Upstream and cloned directory are not equal`n"
                }
            }
            finally
            {
                if ($StashBeforeCompare -ne $null)
                {
                    GitNoob_GitCheckoutBranch -testdirectory $testdirectory -name $StashBeforeCompare
                    GitNoob_GitStashPop -testdirectory $testdirectory
                }
            }
        }
        
        if ($result.Cloned -ne $Cloned)
        {
            $valid = $false
            $message = $message + 'Cloned = ' + $result.Cloned + ', should be ' + $Cloned + "`n"
        }
        
        if ($result.Updated -ne $Updated)
        {
            $valid = $false
            $message = $message + 'Updated = ' + $result.Updated + ', should be ' + $Updated + "`n"
        }

        if ($ErrorNonEmptyAndNotAGitRepository -ne $null -And $result.ErrorNonEmptyAndNotAGitRepository -ne $ErrorNonEmptyAndNotAGitRepository)
        {
            $valid = $false
            $message = $message + 'ErrorNonEmptyAndNotAGitRepository = ' + $result.ErrorNonEmptyAndNotAGitRepository + ', should be ' + $ErrorNonEmptyAndNotAGitRepository + "`n"
        }
        
        if ($ErrorStagedUncommittedFiles -ne $null -And $result.ErrorStagedUncommittedFiles -ne $ErrorStagedUncommittedFiles)
        {
            $valid = $false
            $message = $message + 'ErrorStagedUncommittedFiles = ' + $result.ErrorStagedUncommittedFiles + ', should be ' + $ErrorStagedUncommittedFiles + "`n"
        }
        
        if ($ErrorUnpushedCommitsOnMainBranch -ne $null -And $result.ErrorUnpushedCommitsOnMainBranch -ne $ErrorUnpushedCommitsOnMainBranch)
        {
            $valid = $false
            $message = $message + 'ErrorUnpushedCommitsOnMainBranch = ' + $result.ErrorUnpushedCommitsOnMainBranch + ', should be ' + $ErrorUnpushedCommitsOnMainBranch + "`n"
        }
        
        if ($ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch -ne $null -And $result.ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch -ne $ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch)
        {
            $valid = $false
            $message = $message + 'ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch = ' + $result.ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch + ', should be ' + $ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch + "`n"
        }
        
        if ($ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch -ne $null -And $result.ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch -ne $ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch)
        {
            $valid = $false
            $message = $message + 'ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch = ' + $result.ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch + ', should be ' + $ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertGetLatest !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}

function GitNoob_AssertListTags
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$expected
    )
    #expected = @( ((lightweight=0 annotated=1), name, message), ... )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory -remoteUrl $remoteUrl -remoteBranch $remoteBranch
        
        $command = new-object GitNoob.Git.Command.Tag.ListTags($GitWorkingDirectory)
        $command.WaitFor()
        
        $result = $command.result.Values
        
        if ($result.Count -ne $expected.Length)
        {
            $valid = $false
            $message = $message + 'Expected ' + $expected.Length + ' tags, got ' + $result.Count + "`n"
        }
        
        $ms = New-Object System.IO.MemoryStream
        $bf = New-Object System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        $bf.Serialize($ms, $expected)
        $ms.Position = 0
        $expectedCopy = $bf.Deserialize($ms)
        $ms.Close()
        
        #prepend COUNT to each expectedTag array
        for ($i=0; $i -lt $expectedCopy.Count; $i++)
        {
            $expectedCopy[$i] = @(0) + $expectedCopy[$i]
        }
        
        foreach ($tag in $result)
        {
            foreach ($expectedTag in $expectedCopy)
            {
                if ($expectedTag[2] -eq $tag.ShortName)
                {
                    $expectedTag[0]++;
                    
                    if ($expectedTag[1] -eq 0)
                    {
                        if ($tag.Type.ToString() -ne 'LightWeight')
                        {
                            $valid = $false
                            $message = $message + 'Tag expected to be LightWeight, is ' + $tag.Type.ToString() + ': ' + $tag.Name + "`n"
                        }
                    } 
                    elseif ($expectedTag[1] -eq 1)
                    {
                        if ($tag.Type.ToString() -ne 'Annotated')
                        {
                            $valid = $false
                            $message = $message + 'Tag expected to be Annotated, is ' + $tag.Type.ToString() + ': ' + $tag.Name + "`n"
                        }
                    }
                    
                    if ($expectedTag.length -ge 4)
                    {
                        if ($tag.Message -ne $expectedTag[3])
                        {
                            $valid = $false
                            $message = $message + "Message expected to be:`n" + $expectedTag[3] + "`nis:`n" + $tag.Message + "`n"
                        }
                    }
                    break;
                }
            }
        }
        
        foreach ($expectedTag in $expectedCopy)
        {
            if ($expectedTag[0] -ne 1)
            {
                $valid = $false
                $message = $message + 'Expected tag ' + $expectedTag[0] + ' times found: ' + $expectedTag[2] + "`n"
            }
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertListTags !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
    }
}

function GitNoob_AssertListBranches
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$expected
    )
    #expected = @( ((local=0 localtrackingremotebranch=1), name), ... )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory -remoteUrl $remoteUrl -remoteBranch $remoteBranch
        
        $command = new-object GitNoob.Git.Command.Branch.ListBranches($GitWorkingDirectory, $false)
        $command.WaitFor()
        
        $result = $command.result

        if ($result.Count -ne $expected.Length)
        {
            $valid = $false
            $message = $message + 'Expected ' + $expected.Length + ' branches, got ' + $result.Count + "`n"
        }
        
        $ms = New-Object System.IO.MemoryStream
        $bf = New-Object System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        $bf.Serialize($ms, $expected)
        $ms.Position = 0
        $expectedCopy = $bf.Deserialize($ms)
        $ms.Close()
        
        #prepend COUNT to each expectedTag array
        for ($i=0; $i -lt $expectedCopy.Count; $i++)
        {
            $expectedCopy[$i] = @(0) + $expectedCopy[$i]
        }
        
        foreach ($branch in $result)
        {
            foreach ($expectedBranch in $expectedCopy)
            {
                if ($expectedBranch[2] -eq $branch.ShortName)
                {
                    $expectedBranch[0]++;
                    
                    if ($expectedBranch[1] -eq 0)
                    {
                        if ($branch.Type.ToString() -ne 'Local')
                        {
                            $valid = $false
                            $message = $message + 'Branch expected to be Local, is ' + $branch.Type.ToString() + ': ' + $branch.Name + "`n"
                        }
                    } 
                    elseif ($expectedBranch[1] -eq 1)
                    {
                        if ($branch.Type.ToString() -ne 'LocalTrackingRemoteBranch')
                        {
                            $valid = $false
                            $message = $message + 'Branch expected to be LocalTrackingRemoteBranch, is ' + $branch.Type.ToString() + ': ' + $branch.Name + "`n"
                        }
                    }                
                    break;
                }
            }
        }
        
        foreach ($expectedBranch in $expectedCopy)
        {
            if ($expectedBranch[0] -ne 1)
            {
                $valid = $false
                $message = $message + 'Expected branch ' + $expectedBranch[0] + ' times found: ' + $expectedBranch[2] + "`n"
            }
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertListBranches !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
    }
}

function GitNoob_AssertGetRemoteBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$LocalBranch,
        [Parameter(Mandatory=$true)]$RemoteBranch,
        [Parameter(Mandatory=$false)]$RemoteName = $null
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $command = new-object GitNoob.Git.Command.Branch.GetRemoteBranch($GitWorkingDirectory, $LocalBranch)
        $command.WaitFor()
        
        $result = $command.result
        $resultRemoteName = $command.remoteName
        if ($RemoteBranch -eq $false -Or $RemoteBranch -eq '')
        {
            if ([string]::IsNullOrEmpty($result) -ne $true)
            {
                $valid = $false
                $message = $message + "Expected not to track a remote tracking branch`n"
            }
        }
        else
        {
            if ($result -ne $RemoteBranch)
            {
                $valid = $false
                $message = $message + 'Expected to track remote tracking branch: ' + $RemoteBranch + "`n"
            }
        }
        
        if ($RemoteName -ne $null -And $resultRemoteName -ne $RemoteName)
        {
            $valid = $false
            $message = $message + 'remoteName = ' + $resultRemoteName + ', should be ' + $RemoteName + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertGetRemoteBranch !!!"
        Write-Host $message
        Write-Host ('localbranch      : ' + $localbranch)
        Write-Host ('tracking         : ' + $result)
    }
}        

function GitNoob_AssertIsRebaseActive
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$active,
        [Parameter(Mandatory=$true)]$currentBranch,
        [Parameter(Mandatory=$true)]$ontoBranch,
        [Parameter(Mandatory=$false)]$throwOnInvalid = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    $resultCurrentBranch = $null
    $resultOntoBranch = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $command = new-object GitNoob.Git.Command.WorkingTree.IsRebaseActive($GitWorkingDirectory)
        $command.WaitFor()
        
        $result = $command.result
        $resultCurrentBranch = $command.currentBranch
        $resultOntoBranch = $command.ontoBranch
        
        if ($result -ne $active)
        {
            $valid = $false
            $message = $message + 'Result = ' + $result.result + ', should be ' + $active + "`n"
        }
        
        if ($result -eq $true)
        {
            if ($resultCurrentBranch -eq $null -And $currentBranch -ne $null)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }
            
            if ($resultCurrentBranch -ne $null -And $resultCurrentBranch.ShortName -ne $currentBranch)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }
            
            if ($resultOntoBranch -eq $null -And $ontoBranch -ne $null)
            {
                $valid = $false
                $message = $message + 'ontoBranch = ' + $resultOntoBranch + ', should be ' + $ontoBranch + "`n"
            }
            
            if ($resultOntoBranch -ne $null -And $resultOntoBranch.ShortName -ne $ontoBranch)
            {
                $valid = $false
                $message = $message + 'ontoBranch = ' + $resultOntoBranch + ', should be ' + $ontoBranch + "`n"
            }
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        if ($throwOnInvalid -eq $true) {
            throw "!!! ERROR - GitNoob_AssertIsRebaseActive !!!"
        } else {
            Write-Host "`n!!! ERROR - GitNoob_AssertIsRebaseActive !!!"
            Write-Host $message
            Write-Host ('result        : ' + $result)
            Write-Host ('currentBranch : ')
            Write-Host ($resultCurrentBranch | Format-List | Out-String)
            Write-Host ('ontoBranch : ')
            Write-Host ($resultOntoBranch | Format-List | Out-String)
        }
    }
}        

function GitNoob_AssertIsMergeActive
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$active,
        [Parameter(Mandatory=$true)]$currentBranch, #not checked, not returned
        [Parameter(Mandatory=$true)]$fromBranch,    #not checked, not returned
        [Parameter(Mandatory=$false)]$throwOnInvalid = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    $resultCurrentBranch = $null
    $resultFromBranch = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $command = new-object GitNoob.Git.Command.WorkingTree.IsMergeActive($GitWorkingDirectory)
        $command.WaitFor()
        
        $result = $command.result
        
        if ($result -ne $active)
        {
            $valid = $false
            $message = $message + 'Result = ' + $result.result + ', should be ' + $active + "`n"
        }
        
        if ($result -eq $true)
        {
            <#
            if ($resultCurrentBranch -eq $null -And $currentBranch -ne $null)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }
            
            if ($resultCurrentBranch -ne $null -And $resultCurrentBranch.ShortName -ne $currentBranch)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }
            
            if ($resultFromBranch -eq $null -And $fromBranch -ne $null)
            {
                $valid = $false
                $message = $message + 'fromBranch = ' + $resultFromBranch + ', should be ' + $fromBranch + "`n"
            }
            
            if ($resultFromBranch -ne $null -And $resultFromBranch.ShortName -ne $fromBranch)
            {
                $valid = $false
                $message = $message + 'fromBranch = ' + $resultFromBranch + ', should be ' + $fromBranch + "`n"
            }
            #>
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        if ($throwOnInvalid -eq $true) {
            throw "!!! ERROR - GitNoob_AssertIsMergeActive !!!"
        } else {
            Write-Host "`n!!! ERROR - GitNoob_AssertIsMergeActive !!!"
            Write-Host $message
            Write-Host ('result        : ' + $result)
            Write-Host ('currentBranch : ')
            Write-Host ($resultCurrentBranch | Format-List | Out-String)
            Write-Host ('fromBranch : ')
            Write-Host ($resultFromBranch | Format-List | Out-String)
        }
    }
}

function GitNoob_AssertIsCherryPickActive
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$active,
        [Parameter(Mandatory=$true)]$currentBranch, #not checked, not returned
        [Parameter(Mandatory=$true)]$fromCommitId,  #not checked, not returned
        [Parameter(Mandatory=$false)]$throwOnInvalid = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger

    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    $resultCurrentBranch = $null
    $resultFromCommitId = $null

    $stopwatch = [GitNoob_Stopwatch]::new()
    try {

        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory

        $command = new-object GitNoob.Git.Command.WorkingTree.IsCherryPickActive($GitWorkingDirectory)
        $command.WaitFor()

        $result = $command.result

        if ($result -ne $active)
        {
            $valid = $false
            $message = $message + 'Result = ' + $result + ', should be ' + $active + "`n"
        }

        if ($result -eq $true)
        {
            <#
            if ($resultCurrentBranch -eq $null -And $currentBranch -ne $null)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }

            if ($resultCurrentBranch -ne $null -And $resultCurrentBranch.ShortName -ne $currentBranch)
            {
                $valid = $false
                $message = $message + 'currentBranch = ' + $resultCurrentBranch + ', should be ' + $currentBranch + "`n"
            }

            if ($resultFromCommitId -eq $null -And $fromCommitId -ne $null)
            {
                $valid = $false
                $message = $message + 'fromCommitId = ' + $resultFromCommitId + ', should be ' + $fromCommitId + "`n"
            }
            #>
        }
    } finally { $stopwatch.stop() }

    if ($valid -ne $true)
    {
        if ($throwOnInvalid -eq $true) {
            throw "!!! ERROR - GitNoob_AssertIsCherryPickActive !!!"
        } else {
            Write-Host "`n!!! ERROR - GitNoob_AssertIsCherryPickActive !!!"
            Write-Host $message
            Write-Host ('result        : ' + $result)
            Write-Host ('currentBranch : ')
            Write-Host ($resultCurrentBranch | Format-List | Out-String)
            Write-Host ('fromCommitId : ')
            Write-Host ($resultFromCommitId | Format-List | Out-String)
        }
    }
}        

function GitNoob_AssertMoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$currentbranch,
        [Parameter(Mandatory=$true)]$tobranch
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $result = $GitWorkingDirectory.MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch($currentbranch, $tobranch)
        
        GitNoob_AssertStatus -description ('[' + (GitNoob_AssertGetCallingFunctionName) + '] GetLatest status: ' + $description) `
                             -testdirectory $testdirectory `
                             -DirectoryExists $true `
                             -IsGitRootDirectory $true `
                             -HasWorkingTreeChanges $null `
                             -HasStagedUncommittedFiles $false `
                             -Rebasing $false `
                             -Merging $false `
                             -CurrentBranch $tobranch `
                             -DetachedHead_NotOnBranch $false
                             
        GitNoob_AssertGetRemoteBranch -description ('[' + (GitNoob_AssertGetCallingFunctionName) + '] GetRemoteBranch: ' + $description) `
                                      -testdirectory $testdirectory `
                                      -LocalBranch $tobranch `
                                      -RemoteBranch $false
                             
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertMoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
    
}

function GitNoob_AssertMoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$frombranch,
        [Parameter(Mandatory=$true)]$tobranch
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $result = $GitWorkingDirectory.MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch($frombranch, $tobranch)
        
        GitNoob_AssertGetRemoteBranch -description ('[' + (GitNoob_AssertGetCallingFunctionName) + '] GetRemoteBranch: ' + $description) `
                                      -testdirectory $testdirectory `
                                      -LocalBranch $tobranch `
                                      -RemoteBranch $false
                             
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertMoveUnpushedCommitsFromRemoteTrackingBranchToNewBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}

function GitNoob_AssertAcquireGitLockForMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$Locked,
        [Parameter(Mandatory=$false)]$ReturnGitLock = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $result = $GitWorkingDirectory.AcquireGitLockForMainBranch("GitNoob tests")
        
        if ($result.Locked -ne $Locked)
        {
            $valid = $false
            $message = $message + 'Locked = ' + $result.Locked + ', should be ' + $Locked + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertAcquireGitLockForMainBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
    
    if ($ReturnGitLock -eq $true)
    {
        if ($result -ne $null)
        {
            return $result.GitLock
        }
        else
        {
            return $null
        }
    }
}

function GitNoob_AssertGitLockRelease
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$GitLock,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$Unlocked
    )
    
    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $result = $GitLock.Release()
        
        if ($result.Unlocked -ne $Unlocked)
        {
            $valid = $false
            $message = $message + 'Unlocked = ' + $result.Unlocked + ', should be ' + $Unlocked + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertGitLockRelease !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}    

function GitNoob_AssertResetGitLockForMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $result = $GitWorkingDirectory.ResetGitLockForMainBranch()
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertResetGitLockForMainBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}

function GitNoob_AssertEnsureMainBranchExistance
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$mainBranch,
        [Parameter(Mandatory=$true)]$Exists
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory -remoteBranch $mainBranch
        
        $result = $GitWorkingDirectory.EnsureMainBranchExistance()
        
        if ($result.Exists -ne $Exists)
        {
            $valid = $false
            $message = $message + 'Rebased = ' + $result.Rebased + ', should be ' + $Rebased + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertEnsureMainBranchExistance !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}


function GitNoob_AssertRebaseCurrentBranchOntoMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$true)]$Rebased,
        [Parameter(Mandatory=$false)]$ErrorWorkingTreeChanges = $null,
        [Parameter(Mandatory=$false)]$ErrorStagedUncommittedFiles = $null,
        [Parameter(Mandatory=$false)]$ErrorUnpushedCommitsOnMainBranch = $null,
        [Parameter(Mandatory=$false)]$ErrorConflicts = $null
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null;
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory
        
        $result = $GitWorkingDirectory.RebaseCurrentBranchOntoMainBranch('GitNoob test - undelete tag message')
        
        if ($result.Rebased -ne $Rebased)
        {
            $valid = $false
            $message = $message + 'Rebased = ' + $result.Rebased + ', should be ' + $Rebased + "`n"
        }

        if ($ErrorWorkingTreeChanges -ne $null -And $result.GitDisaster_WorkingTreeChanges -ne $ErrorWorkingTreeChanges)
        {
            $valid = $false
            $message = $message + 'GitDisaster_WorkingTreeChanges = ' + $result.GitDisaster_WorkingTreeChanges + ', should be ' + $ErrorWorkingTreeChanges + "`n"
        }
        
        if ($ErrorStagedUncommittedFiles -ne $null -And $result.GitDisaster_StagedUncommittedFiles -ne $ErrorStagedUncommittedFiles)
        {
            $valid = $false
            $message = $message + 'GitDisaster_StagedUncommittedFiles = ' + $result.GitDisaster_StagedUncommittedFiles + ', should be ' + $ErrorStagedUncommittedFiles + "`n"
        }
        
        if ($ErrorUnpushedCommitsOnMainBranch -ne $null -And $result.GitDisaster_UnpushedCommitsOnMainBranch -ne $ErrorUnpushedCommitsOnMainBranch)
        {
            $valid = $false
            $message = $message + 'GitDisaster_UnpushedCommitsOnMainBranch = ' + $result.GitDisaster_UnpushedCommitsOnMainBranch + ', should be ' + $ErrorUnpushedCommitsOnMainBranch + "`n"
        }
        
        if ($ErrorConflicts -ne $null -And $result.ErrorConflicts -ne $ErrorConflicts)
        {
            $valid = $false
            $message = $message + 'ErrorConflicts = ' + $result.ErrorConflicts + ', should be ' + $ErrorConflicts + "`n"
        }
        
        if ($ErrorConflicts -eq $true)
        {
            GitNoob_AssertStatus -description ('[' + (GitNoob_AssertGetCallingFunctionName) + '] GetLatest status (rebase conflicts): ' + $description) `
                                 -testdirectory $testdirectory `
                                 -DirectoryExists $true `
                                 -IsGitRootDirectory $true `
                                 -HasWorkingTreeChanges $true `
                                 -HasStagedUncommittedFiles $true `
                                 -Rebasing $true `
                                 -Merging $false
        }
        
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertRebaseCurrentBranchOntoMainBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}

function GitNoob_AssertBuildCacheAndCommitOnMainBranch
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)]$testdirectory,
        [Parameter(Mandatory=$true)]$projectType, #e.g. GitNoob_CreatePhpLaravel7Directory
        [Parameter(Mandatory=$false)]$LaunchDebugger = $false,
        [Parameter(Mandatory=$false)]$ErrorBuildingCache = $null,
        [Parameter(Mandatory=$false)]$Updated = $null
    )

    GitNoob_LaunchDebugger -LaunchDebugger $LaunchDebugger
    
    $valid = $true
    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    $result = $null
    
    $stopwatch = [GitNoob_Stopwatch]::new()
    try {
        $GitWorkingDirectory = GitNoob_Git_New_GitWorkingDirectory -testdirectory $testdirectory `
                                                                   -projectType $projectType.projectType `
                                                                   -phpPath $projectType.phpPath `
                                                                   -phpIniTemplateFilename $projectType.phpIniTemplateFile

        $phpIni = new-object GitNoob.Utils.ConfigFileTemplate.PhpIni($null, $GitWorkingDirectory.ConfigWorkingDirectory)
        $tempPath = GitNoob_GetRootDirectoryForTesting
        $IExecutor = new-object GitNoob.Utils.BatFile(
            $null,
            'GitNoob Test', [GitNoob.Utils.BatFile+RunAstype]::runAsInvoker, [GitNoob.Utils.BatFile+WindowType]::hideWindow, 'GitNoob Test',
            $null, $GitWorkingDirectory.ConfigWorkingDirectory,
            $phpIni)

        $result = $GitWorkingDirectory.BuildCacheAndCommitOnMainBranch($IExecutor, 'Build cache - GitNoob_AssertBuildCacheAndCommitOnMainBranch')
        
        if ($ErrorBuildingCache -ne $null -And $result.ErrorBuildingCache -ne $ErrorBuildingCache)
        {
            $valid = $false
            $message = $message + 'ErrorBuildingCache = ' + $result.ErrorBuildingCache + ', should be ' + $ErrorBuildingCache + "`n"
        }
        
        if ($Updated -ne $null -And $result.Updated -ne $Updated)
        {
            $valid = $false
            $message = $message + 'Updated = ' + $result.Updated + ', should be ' + $Updated + "`n"
        }
    } finally { $stopwatch.stop() }
    
    if ($valid -ne $true)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertBuildCacheAndCommitOnMainBranch !!!"
        Write-Host $message
        Write-Host ($result | Format-List | Out-String)
        
        GitNoob_PressENTERIfDebuggerLaunched -LaunchDebugger $LaunchDebugger
    }
}        


function GitNoob_WriteHostHex()
{
    param (
        [Parameter(Mandatory=$true)]$value
    )
    
    $inputBytes = $null;
    if ($value -is [byte[]])
        {
            $inputBytes = $value
        }
    else
        {
            $inputBytes = $GitNoob_Utf8NoBom_Encoding.GetBytes([string] $value)
        }
    
    $counter = 0;
    $line = "{0}   " -f  [Convert]::ToString($counter, 16).ToUpper().PadLeft(8, '0')
    foreach($byte in $inputBytes)
    {
        ## Display each byte, in 2-digit hexidecimal, and add that to the
        ## left-hand side.
        $line += "{0:X2} " -f $byte
        
        $counter++;
        
        if (($counter % 16) -eq 0) {
            Write-Host $line
            
            $line = "{0}   " -f  [Convert]::ToString($counter, 16).ToUpper().PadLeft(8, '0')
        }
   }
   
   Write-Host $line
}

function GitNoob_AssertSame
{
    param (
        [Parameter(Mandatory=$true)]$value,
        [Parameter(Mandatory=$true)]$expectation,
        [Parameter(Mandatory=$true)]$message
    )
    
    if ($value -ne $expectation) {
        Write-Host "`n!!! ERROR - GitNoob_AssertSame !!!"
        Write-Host $message
        Write-Host ('value      : ' + $value)
        GitNoob_WriteHostHex -value $value
        Write-Host ('expectation: ' + $expectation)
        GitNoob_WriteHostHex -value $expectation
    }
}

function GitNoob_AssertFile
{
    param (
        [Parameter(Mandatory=$true)]$description,
        [Parameter(Mandatory=$true)][pscustomobject]$testdirectory,
        [Parameter(Mandatory=$true)]$name,
        [Parameter(Mandatory=$true)]$expectation
    )

    $message = '[' + (GitNoob_AssertGetCallingFunctionName) + '] Description: ' + $description + "`n"
    
    $filename = Join-Path $testdirectory.path $name
    $contents = $null
    if (Test-Path -Path $filename -PathType Leaf)
    {
        $contents = Get-Content -Raw $filename
    }

    if ($contents -ne $expectation)
    {
        Write-Host "`n!!! ERROR - GitNoob_AssertFile !!!"
        Write-Host $message
        Write-Host ('filename     : ' + $filename)
        Write-Host ('file-contents: ' + $contents)
        GitNoob_WriteHostHex -value $contents
        Write-Host ('expectation  : ' + $expectation)
        GitNoob_WriteHostHex -value $expectation
    }
}
