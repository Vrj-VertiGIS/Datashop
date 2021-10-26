-- =============================================================================
-- Zweck:  SDE-Datenbank erstellen und Datenbankrollen erzeugen
-- GEOCOM Informatik AG
-- =============================================================================
-- datum       wer       anpassungen
-- --------------------------------------
-- 2010-11-03, scm       erstellt und getestet
-- =============================================================================
-- Variablen
-- -----------------------------------------------------------------------------
-- $(operat)
-- $(medium)
-- $(ownerpw)


-- =============================================================================


-------------------------------------------------------------------------------
-- Create SDE User Login
-------------------------------------------------------------------------------
PRINT 'Create GNSD Login'
GO

IF NOT EXISTS (SELECT name FROM master.sys.syslogins WHERE name='$(gnsduser)')
BEGIN
  CREATE LOGIN [$(gnsduser)] WITH PASSWORD='$(gnsdpw)', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[Deutsch], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END
ELSE
BEGIN
  PRINT 'Login GNSD schon vorhanden'
END
GO


-------------------------------------------------------------------------------
-- Create Database
-------------------------------------------------------------------------------

PRINT 'Create Database'
GO
IF EXISTS (SELECT name FROM master.sys.databases WHERE name = '$(geodatabase)')
BEGIN
  PRINT 'Database schon vorhanden'
  
END
ELSE
BEGIN
CREATE DATABASE [$(geodatabase)] ON  PRIMARY 
( NAME = N'$(geodatabase)_dat', FILENAME = N'$(dbdatapath)\$(geodatabase).mdf' , SIZE = $(DBSIZE)MB , FILEGROWTH = $(DBGROWTH)MB ,MAXSIZE = $(DBMAXSIZE)MB )
 LOG ON 
 ( NAME = N'$(geodatabase)_log', FILENAME = N'$(dblogpath)\$(geodatabase)_log.ldf' , SIZE = $(LOGSIZE)MB , FILEGROWTH = $(LOGGROWTH)MB, MAXSIZE = $(LOGMAXSIZE)MB)

ALTER DATABASE [$(geodatabase)] SET COMPATIBILITY_LEVEL = 100

ALTER DATABASE [$(geodatabase)] SET ANSI_NULL_DEFAULT OFF 

ALTER DATABASE [$(geodatabase)] SET ANSI_NULLS OFF 

ALTER DATABASE [$(geodatabase)] SET ANSI_PADDING OFF 

ALTER DATABASE [$(geodatabase)] SET ANSI_WARNINGS OFF 

ALTER DATABASE [$(geodatabase)] SET ARITHABORT OFF 

ALTER DATABASE [$(geodatabase)] SET AUTO_CLOSE OFF 

ALTER DATABASE [$(geodatabase)] SET AUTO_CREATE_STATISTICS ON 

ALTER DATABASE [$(geodatabase)] SET AUTO_SHRINK OFF 

ALTER DATABASE [$(geodatabase)] SET AUTO_UPDATE_STATISTICS ON 

ALTER DATABASE [$(geodatabase)] SET CURSOR_CLOSE_ON_COMMIT OFF 

ALTER DATABASE [$(geodatabase)] SET CURSOR_DEFAULT  GLOBAL 

ALTER DATABASE [$(geodatabase)] SET CONCAT_NULL_YIELDS_NULL OFF 

ALTER DATABASE [$(geodatabase)] SET NUMERIC_ROUNDABORT OFF 

ALTER DATABASE [$(geodatabase)] SET QUOTED_IDENTIFIER OFF 

ALTER DATABASE [$(geodatabase)] SET RECURSIVE_TRIGGERS OFF 

ALTER DATABASE [$(geodatabase)] SET  DISABLE_BROKER 

ALTER DATABASE [$(geodatabase)] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 

ALTER DATABASE [$(geodatabase)] SET DATE_CORRELATION_OPTIMIZATION OFF 

ALTER DATABASE [$(geodatabase)] SET PARAMETERIZATION SIMPLE 

ALTER DATABASE [$(geodatabase)] SET  READ_WRITE 

ALTER DATABASE [$(geodatabase)] SET RECOVERY FULL 

ALTER DATABASE [$(geodatabase)] SET  MULTI_USER 

ALTER DATABASE [$(geodatabase)] SET PAGE_VERIFY CHECKSUM  
END
GO




USE [$(geodatabase)]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [$(geodatabase)] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

-------------------------------------------------------------------------------
-- Create GNSD User in Database
-------------------------------------------------------------------------------

-- User zu Login erstellen
PRINT 'Create GNSD User in Database'
GO

CREATE USER [$(gnsduser)] FOR LOGIN [$(gnsduser)]
GO
-- Schema in der Datenbank mit Schemaowner anlegen
CREATE SCHEMA [$(gnsduser)] AUTHORIZATION [$(gnsduser)]
GO
ALTER USER [$(gnsduser)] WITH DEFAULT_SCHEMA=[$(gnsduser)]
GO

GRANT CREATE FUNCTION TO [$(gnsduser)]
GO
GRANT CREATE PROCEDURE TO [$(gnsduser)]
GO
GRANT CREATE VIEW TO [$(gnsduser)]
GO
GRANT CREATE TABLE TO [$(gnsduser)]
GO
