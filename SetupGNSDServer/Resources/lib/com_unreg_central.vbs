Dim WshShell
Set WshShell = CreateObject("Wscript.Shell")
WshShell.run "regsvr32 GeonisCentralObjects.dll /unregister"
Set WshShell = nothing