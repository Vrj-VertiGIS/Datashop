Import-Module $env:geo_build\Utils\Common\2.0.0\Utils.Common.psm1 -DisableNameChecking -ErrorAction Stop

Push-Location $PSScriptRoot
svn update --ignore-externals
$svnInfo = [xml](svn info --xml)
Pop-Location

$assInfoPath = "$PSScriptRoot\build\CommonAssemblyVersion.cs"
$asmInfo = Get-Content -Raw -Path $assInfoPath
$asmInfo -match '(?<=AssemblyVersion\(\")(?<asmVer>.*)(?=\.0\")' | Out-Null
$DatashopVersion = $Matches.asmVer 
$asmInfo -match '(?<=AssemblyInformationalVersion\(\")(?<asmInfoVer>.*)(?=\")' | Out-Null
$DatashopProductVersion = $Matches.asmInfoVer + "" #add some suffix
$GeonisLibVersion = "5.1.0"
$ArcGISVersions = @("10.6.0", "10.7.0", "10.8.0")

Remove-Item $PSScriptRoot\_setups -Recurse -Force
# replace file version
$fileVersionBuildInfo = $svnInfo.info.entry.revision
Replace-InAssemblyByPlace -File $assInfopath -Place 'fileversion' -Replacement ('${major}.${minor}.${patch}.' + $fileVersionBuildInfo)

foreach ($ArcGISVersion in $ArcGISVersions)
{
    $itemsToDelete = Get-ChildItem -Path $PSScriptRoot -Recurse -File | Where FullName -Match '\\bin|\\obj|\\Release|\\Debug' 
	Write-Host "Deleted $($itemsToDelete.Count) files" -ForegroundColor Green
	$itemsToDelete | Remove-Item -Force -Recurse
	
    $nantCmd = 
@"
NAnt.exe -buildfile:GNSDatashop.build ``
    -D:arcgis_version=$ArcGISVersion ``
    -D:geonis_lib_version=$GeonisLibVersion ``
    -D:datashop_version=$DatashopVersion ``
    -D:datashop_product_version='$DatashopProductVersion' ``
    -D:buildManually=true ``
    VS_Setups
if(-not `$?) {throw "Nant failed"}
"@
    Write-Host "Starting a build"
    Write-Host "$nantCmd" -ForegroundColor Green
    Push-Location $PSScriptRoot
    Invoke-Expression -Command $nantCmd -ErrorAction Stop
    Pop-Location
}

# revert common assembly info
svn revert -R $assInfoPath


$svnRev = $svnInfo.info.entry.revision
$currentSvnUrl = $svnInfo.info.entry.url
$rootSvnUrl = $svnInfo.info.entry.repository.root
$tagSvnUrl =  "$rootSvnUrl/tags/$DatashopProductVersion/rc_$svnRev"

$confirmation = Read-Host "Do you want to create a tag`nfrom $currentSvnUrl`nto   $tagSvnUrl`n(yes/no)"
if ($confirmation -eq 'y' -or $confirmation -eq 'yes') 
{
    $tagSuffix = Read-Host "Enter tag suffix (e.g. '_ewb') or just press enter"
    $svnTagCmd =
@"
svn copy --parents ``
-m "Tagging a release candidate '$DatashopProductVersion.$svnRev$tagSuffix'." ``
$currentSvnUrl ``
$tagSvnUrl$tagSuffix
"@

    Write-Host "Creating a tag" -ForegroundColor Green
    Write-Host "$svnTagCmd" -ForegroundColor Green
    Invoke-Expression -Command $svnTagCmd -Verbose
}

pause