Import-Module $env:geo_build\3.0.0\Utils\Utils.MSBuild.psm1 -DisableNameChecking -ErrorAction Stop

$ArcGISVersions = @( "10_1_0", "10_2_0", "10_3_0", "10_4_0", "10_5_0", "10_6_0", "10_7_0", "10_8_0" )

foreach ($ArcGISVersion in $ArcGISVersions)
{
    Write-Host "Building using version ArcGIS $ArcGISVersion"
    Push-Location "$PSScriptRoot\build"
    . ".\set_ARCGIS_$ArcGISVersion.bat"
    Pop-Location

    G-MSBuild "$PSScriptRoot\GEOCOM.GNSDatashop.Export.DXF.sln" `
        -target:Rebuild `
        -property:Configuration=Release `
        -property:Platform=x86 `
        -property:RegisterForComInterop=false `
        -maxcpucount `
        -verbosity:normal
}