SPOOL ./create_schema_ora.log

-- SYSTEM PRIVILEGES TO CREATE TABLE IN OWN SCHEMA
GRANT CREATE TABLE TO &&1;


-- Add one new column to the GNSD_BIZ_USERS table
ALTER TABLE &&1..GNSD_BIZ_USERS
    ADD (FAILEDLOGINCOUNT INT, 
		BLOCKEDUNTIL DATE NULL)

/
COMMIT
/


SPOOL OFF
QUIT