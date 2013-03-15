<#
.SYNOPSIS
Prepare executable for consumption.

.DESCRIPTION
Create ZIP package containing the executable in a subdirectory. Filename of
package should contain the assembly version number.
#>
# Copyright (C) 2013  Andrei Nicholson
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.

function TempBuildPath([string]$program, [string]$programPath)
{
    [string]$tempDir = [System.IO.Path]::GetTempPath()
    [string]$zipDir = Join-Path $tempDir $program
    New-Item -ItemType directory -Path $zipDir
    Copy-Item $programPath $zipDir
    return $zipDir
}

function CreateZip([string]$program, [string]$targetDir, [string]$programPath)
{
    [string]$programVersion = (Get-Command $programPath).FileVersionInfo.ProductVersion
    [string]$zipArchive = $program + "-" + $programVersion + ".zip"
    
    If (Test-Path (Join-Path $targetDir $zipArchive))
    {
        Write-Host -Fore Yellow "Latest build, $zipArchive, already available"
        return
    }
    
    [string]$buildDir = TempBuildPath $program $programPath
    [string]$zipArchivePath = Join-Path $targetDir $zipArchive
    [string]$scriptLocation = Get-Location
    
    Set-Location ([System.IO.Path]::GetTempPath())

    [string]$assembly = "WindowsBase,Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    [System.Reflection.Assembly]::Load($assembly)
    $ZipPackage = [System.IO.Packaging.ZipPackage]::Open($zipArchivePath, [System.IO.FileMode]"OpenOrCreate", [System.IO.FileAccess]"ReadWrite")

    $partName = New-Object System.Uri("/$program/$program.exe", [System.UriKind]"Relative")
    $part = $ZipPackage.CreatePart($partName, "application/zip", [System.IO.Packaging.CompressionOption]"Maximum")
    $bytes = [System.IO.File]::ReadAllBytes((Join-Path $buildDir "$program.exe"))
    Write-Host (Join-Path $buildDir "$program.exe")
    Write-Host $bytes.Length
    $stream = $part.GetStream()
    $stream.Write($bytes, 0, $bytes.Length)
    $stream.Close()

    $ZipPackage.Close()

    Set-Location $scriptLocation
    Move-Item $zipArchivePath $targetDir
    Remove-Item -Recurse $buildDir

    Write-Host -Fore Green "Successfully created zip "$zipArchive
}

[string]$baseDir = Resolve-Path ..\src\KeePassToRdp\bin\Release
[string]$program = "KeePassToRdp"
[string]$programExe = $program + ".exe"
[string]$programPath = [System.IO.Path]::GetFullPath((Join-Path $baseDir $programExe))

CreateZip $program $baseDir $programPath
