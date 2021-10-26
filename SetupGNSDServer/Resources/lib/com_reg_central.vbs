Dim WshShell
Set WshShell = CreateObject("Wscript.Shell")
WshShell.run "regsvr32 GeonisCentralObjects.dll"
Set WshShell = nothing