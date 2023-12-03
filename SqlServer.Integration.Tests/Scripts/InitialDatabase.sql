IF NOT EXISTS(SELECT name FROM sys.server_principals WHERE name = 'integration-test')
    BEGIN
        CREATE LOGIN [integration-test] WITH PASSWORD = 'InTegrat3mE', DEFAULT_DATABASE = [master], CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF;
    END
GO

DROP DATABASE IF EXISTS [IntegrationTests]
GO

CREATE DATABASE [IntegrationTests]
GO
