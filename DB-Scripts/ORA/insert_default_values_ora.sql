SPOOL .\insert_default_values_ora.log

-- Define admin with password test1234
INSERT INTO &&1..gnsd_biz_users
           (Password,PasswordSalt,ROLES,Status)
     VALUES
           ('936AF4308F9216458A195FB566E1AFFCE2D8060C', 'KIjeVf9yiQHYYhHtRJsL3Q==', 'BUSINESS,ADMIN', 1)
/

INSERT INTO &&1..gnsd_users
           (Adress, Firstname, Lastname, Email, Street, Streetnr, Citycode, City, Company, Tel, Fax, BIZUSER)
     VALUES
           ('','','Administrator','admin_at_mycompany.ch','','','','','','','',1)
/


-- Define standard values for reasons table 
DELETE FROM &&1..GNSD_REASONS
/
INSERT INTO &&1..GNSD_REASONS (DESCRIPTION) VALUES 
	('unbekannt')
/

INSERT INTO &&1..GNSD_REASONS (DESCRIPTION) VALUES   
  ('Bauausführung')
/
INSERT INTO &&1..GNSD_REASONS (DESCRIPTION) VALUES   
  ('Grabarbeit')
/
INSERT INTO &&1..GNSD_REASONS (DESCRIPTION) VALUES   
  ('Planung')
/
INSERT INTO &&1..GNSD_REASONS (DESCRIPTION) VALUES   
  ('Störung')
/

--  Define standard values for placementoptions table 
DELETE FROM &&1..GNSD_PLACEMENTOPTIONS
/
INSERT INTO &&1..GNSD_PLACEMENTOPTIONS (TEXT) VALUES   
  ('E-Mail');

INSERT INTO &&1..GNSD_PLACEMENTOPTIONS (TEXT) VALUES   
  ('Brief');

INSERT INTO &&1..GNSD_PLACEMENTOPTIONS (TEXT) VALUES   
  ('Fax');


COMMIT
/

SPOOL OFF
QUIT

