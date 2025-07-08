IF db_id('IntegrationTests') IS NOT NULL
BEGIN
	ALTER DATABASE [IntegrationTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

	DROP DATABASE IF EXISTS [IntegrationTests]
END
GO

CREATE DATABASE [IntegrationTests]
GO

IF NOT EXISTS(SELECT name
              FROM sys.server_principals
              WHERE name = N'integration-test-login')
BEGIN
    CREATE LOGIN [integration-test-login] WITH PASSWORD = 'InTegrat3mE',
        DEFAULT_DATABASE = [IntegrationTests],
        CHECK_EXPIRATION = OFF,
        CHECK_POLICY = OFF;
END
GO

USE [IntegrationTests]
CREATE USER [integration-test-user] FOR LOGIN [integration-test-login] WITH DEFAULT_SCHEMA=[dbo]
GO
       
USE [IntegrationTests]
ALTER ROLE [db_ddladmin] ADD MEMBER [integration-test-user]
GO
      
USE [IntegrationTests]      
ALTER ROLE [db_datareader] ADD MEMBER [integration-test-user]
GO
      
USE [IntegrationTests]      
ALTER ROLE [db_datawriter] ADD MEMBER [integration-test-user]
GO
       
       
-- GRANT CREATE TABLE TO [integration-test]
-- GO
-- 
-- GRANT ALTER, EXECUTE, SELECT, INSERT, DELETE, UPDATE ON DATABASE::IntegrationTests TO [integration-test-user]
-- GO