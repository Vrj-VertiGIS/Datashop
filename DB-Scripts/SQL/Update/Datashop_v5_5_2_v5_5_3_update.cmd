@echo off

rem ****************************************************************************
rem HIER FOLGENDE ZEILEN ANPASSEN

rem Grundeinstellungen einmal pro Server
rem *************************************
set serverinstance=datashop_server
set gnsduser=datashop_user
set gnsdpw=datashop_pw
set dbname=datashop_db


rem #################################################################################
rem ab hier keine Aenderungen
echo *** Update Datashop Repository to 5.5.3
sqlcmd.exe -S %serverinstance% -U %gnsduser% -P %gnsdpw% -i .\Datashop_v5_5_2_v5_5_3_update.sql -v geodatabase="%dbname%"

echo *** Update beendet.
pause
