@echo on
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
set tnsname=datasho
set dba_user=dbauser
set dba_pwd=dbpw
set gnsduser=datashop_user
set gnsdpw=datashop_pwd
set user_tablespace=users
set user_quota=unlimited

rem #################################################################################
rem ab hier keine Aenderungen

echo *** Create schema
sqlplus -s %dba_user%/%dba_pwd%@%tnsname% AS Sysdba @.\create_schema_ora.sql "%gnsduser%" "%gnsdpw%" "%user_tablespace%" "%user_quota%"

echo *** Define standard values
sqlplus -s %dba_user%/%dba_pwd%@%tnsname% AS Sysdba @.\insert_default_values_ora.sql "%gnsduser%" 

echo *** Installation beendet.
pause
