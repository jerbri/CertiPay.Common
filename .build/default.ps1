$PSake.use_exit_on_error = $true
Properties {
	## The version format is Major.Minor.Build
	## The dll will be stamped with the informational version including the changeset
	$MajorVersion = "0"
	$MinorVersion = "0"
	$Version = "$MajorVersion.$MinorVersion.$BuildNumber"
	$InformationalVersion = "$Version.$CommitHash"
}

## This comes from the build server iteration
if(!$BuildNumber) { $BuildNumber = "1" }

## This comes from the Hg commit hash used to build
if(!$CommitHash) { $CommitHash = "local-build" }

## The build configuration, i.e. Debug/Release
if(!$Configuration) { $Configuration = "Debug" }

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$SolutionRoot = (Split-Path -parent $Here)

Import-Module "$Here\Common" -DisableNameChecking

$SolutionFile = "$SolutionRoot\CertiPay.sln"

$NuGet = Join-Path $SolutionRoot ".nuget\nuget.exe"

$MSBuild ="${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe"

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task default -depends Build

Task Build -depends Restore-Packages, Update-AssemblyInfoFiles {
	exec { . $MSBuild $SolutionFile /t:Build /v:normal /p:Configuration=$Configuration }
}

Task Package {
	## TODO
}

Task Clean {
	Remove-Item -Path "$SolutionRoot\packages\*" -Exclude repositories.config -Recurse -Force 
	Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { Remove-Item $_.fullname -Force -Recurse }
	exec { . $MSBuild $SolutionFile /t:Clean /v:quiet }
}

Task Restore-Packages -depends Install-BuildTools {
	exec { . $NuGet restore $SolutionFile }
}

Task Install-NUnitRunner {
	if(!(Test-Path $NUnit)){
		. $NuGet install NUnit.Runners -version $NUnitVersion -OutputDirectory "$SolutionRoot\packages"
	}
}

Task Install-MSBuild {
    if(!(Test-Path "${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe")) 
	{ 
		cinst microsoft-build-tools
	}
}

Task Install-BuildTools -depends Install-MSBuild

# Borrowed from Luis Rocha's Blog (http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html)
Task Update-AssemblyInfoFiles {
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileCommitPattern = 'AssemblyInformationalVersion\("(.*?)"\)'

    $assemblyVersion = 'AssemblyVersion("' + $Version + '")';
    $fileVersion = 'AssemblyFileVersion("' + $Version + '")';
    $commitVersion = 'AssemblyInformationalVersion("' + $InformationalVersion + '")';

    Get-ChildItem -path $SolutionRoot -r -filter AssemblyInfo.cs | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        $filename + ' -> ' + $Version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion } |
            % {$_ -replace $fileCommitPattern, $commitVersion }
        } | Set-Content $filename
    }
}