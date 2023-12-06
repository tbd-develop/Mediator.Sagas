DROP DATABASE IF EXISTS [IntegrationTests]
GO

CREATE DATABASE [IntegrationTests]
GO

IF EXISTS(SELECT name
              FROM sys.server_principals
              WHERE name = N'integration-test')
    BEGIN
        DROP USER [integration-test]
        
        DROP LOGIN [integration-test-login]
    END
GO

CREATE LOGIN [integration-test-login] WITH PASSWORD = 'InTegrat3mE',
    DEFAULT_DATABASE = [IntegrationTests],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = OFF;

GO

USE [IntegrationTests]

CREATE USER [integration-test] FROM LOGIN [integration-test-login]

GO

GRANT DELETE, EXECUTE, INSERT, SELECT, UPDATE ON DATABASE::IntegrationTests TO [integration-test]
GO