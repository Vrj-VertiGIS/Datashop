@echo off
REM #goto inactive
rem ==============================================================================
rem Zweck:  SDE-Datenbank erstellen mit
rem         DB-OWNER Login, DB Rollen, Zuordnung SDE Login zu DB User
rem GEOCOM Informatik AG
rem ==============================================================================
rem datum       wer       anpassungen
rem --------------------------------------
rem 2010-11-03, scm       erstellt und getestet
rem ==============================================================================
cd /d "%~dp0"

rem ****************************************************************************
rem ****************************************************************************
rem ****************************************************************************
rem HIER FOLGENDE ZEILEN ANPASSEN

rem Grundeinstellungen einmal pro Server
rem *************************************
set serverinstance=.\SQLEXPRESS
set sauser=sa
set sapw=5a4l0c@l
set gnsduser=gnsd_de
set gnsdpw=gnsd_de
set dbdatapath=D:\DevelopmentData\SQLData
set dblogpath=D:\DevelopmentData\SQLData

REM Datenbankname standardmaessig operat_medium
REM fuer Testdatenbanken hinten noch _TEST ergaenzen
set dbname=gnsd

REM Datenbankgroessen sorgfaeltig angeben in MB
set DBSIZE=256
set DBGROWTH=256
set DBMAXSIZE=1024

set LOGSIZE=256
set LOGGROWTH=256
set LOGMAXSIZE=1024

rem #################################################################################
rem ab hier keine Aenderungen

echo *** Datenbank, Datenbankrollen und Rollen erstellen
sqlcmd.exe -S %serverinstance% -U %sauser% -P %sapw% -i .\Create_Database_and_User.sql -v geodatabase="%dbname%" -v dbdatapath="%dbdatapath%" -v dblogpath="%dblogpath%" -v DBSIZE="%DBSIZE%" -v DBGROWTH="%DBGROWTH%" -v DBMAXSIZE="%DBMAXSIZE%" -v LOGSIZE="%LOGSIZE%" -v LOGGROWTH="%LOGGROWTH%" -v LOGMAXSIZE="%LOGMAXSIZE%" -v ownerpw="%ownerpw%"

echo *** Create schema
sqlcmd.exe -S %serverinstance% -U %gnsduser% -P %gnsdpw% -i .\Create_Schema.sql -v geodatabase="%dbname%" -v dbdatapath="%dbdatapath%" -v dblogpath="%dblogpath%" -v DBSIZE="%DBSIZE%" -v DBGROWTH="%DBGROWTH%" -v DBMAXSIZE="%DBMAXSIZE%" -v LOGSIZE="%LOGSIZE%" -v LOGGROWTH="%LOGGROWTH%" -v LOGMAXSIZE="%LOGMAXSIZE%" -v ownerpw="%ownerpw%"

echo *** Define standard values
sqlcmd.exe -S %serverinstance% -U %gnsduser% -P %gnsdpw% -i .\Insert_Default_Values.sql -v geodatabase="%dbname%" -v dbdatapath="%dbdatapath%" -v dblogpath="%dblogpath%" -v DBSIZE="%DBSIZE%" -v DBGROWTH="%DBGROWTH%" -v DBMAXSIZE="%DBMAXSIZE%" -v LOGSIZE="%LOGSIZE%" -v LOGGROWTH="%LOGGROWTH%" -v LOGMAXSIZE="%LOGMAXSIZE%" -v ownerpw="%ownerpw%"

echo *** Installation beendet.
pause
