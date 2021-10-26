@echo off
REM #goto inactive
rem ==============================================================================
rem Zweck:  Datashop-DB Update auf Version 5.0.3
rem
rem GEOCOM Informatik AG
rem ==============================================================================
rem datum       wer       anpassungen
rem --------------------------------------
rem 2012-05-07, tbu       erstellt und getestet
rem ==============================================================================
cd /d "%~dp0"

rem ****************************************************************************
rem ****************************************************************************
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


echo *** Update Datashop Repository to 5.0.3
sqlcmd.exe -S %serverinstance% -U %gnsduser% -P %gnsdpw% -i .\Datashop_v5_0_2_v5_0_3_update.sql -v geodatabase="%dbname%"

echo *** Update beendet.
pause
