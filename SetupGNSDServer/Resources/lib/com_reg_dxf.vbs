Dim WshShell
Set WshShell = CreateObject("Wscript.Shell")
WshShell.run "regsvr32 GnDxfExportSrv.dll"
Set WshShell = nothing