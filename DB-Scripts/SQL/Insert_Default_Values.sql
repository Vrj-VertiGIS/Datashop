USE [$(geodatabase)]

/****** Define admin with password test1234 ******/

INSERT INTO [$(gnsduser)].[gnsd_biz_users]
           ([Password]
           ,[PasswordSalt]
           ,[Roles]
           ,[Status])
     VALUES
           ('936AF4308F9216458A195FB566E1AFFCE2D8060C', 'KIjeVf9yiQHYYhHtRJsL3Q==', 'BUSINESS,ADMIN', 1)
GO

INSERT INTO [$(gnsduser)].[gnsd_users]
           ([Adress]
           ,[Firstname]
           ,[Lastname]
           ,[Email]
           ,[Street]
           ,[Streetnr]
           ,[Citycode]
           ,[City]
           ,[Company]
           ,[Tel]
           ,[Fax]
           ,[BIZUSER])
     VALUES
           ('','','Administrator','admin_at_mycompany.ch','','','','','','','',1)
GO


/****** Define standard values for reasons table ******/
DELETE FROM [$(gnsduser)].GNSD_REASONS;

INSERT INTO [$(gnsduser)].GNSD_REASONS ( DESCRIPTION) VALUES 
  ( 'unbekannt');
GO
INSERT INTO [$(gnsduser)].GNSD_REASONS ( DESCRIPTION) VALUES   
  ( 'Bauausführung');
GO
INSERT INTO [$(gnsduser)].GNSD_REASONS ( DESCRIPTION) VALUES   
  ( 'Grabarbeit');
GO
INSERT INTO [$(gnsduser)].GNSD_REASONS ( DESCRIPTION) VALUES   
  ( 'Planung');
GO
INSERT INTO [$(gnsduser)].GNSD_REASONS ( DESCRIPTION) VALUES   
  ( 'Störung');
GO

DELETE FROM [$(gnsduser)].[gnsd_placementoptions];

INSERT INTO [$(gnsduser)].[gnsd_placementoptions] (Text) VALUES   
  ('E-Mail');
GO
INSERT INTO [$(gnsduser)].[gnsd_placementoptions] (Text) VALUES   
  ('Brief');
GO
INSERT INTO [$(gnsduser)].[gnsd_placementoptions] (Text) VALUES   
  ('Fax');
GO