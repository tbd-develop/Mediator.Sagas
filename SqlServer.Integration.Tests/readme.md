/*

CREATE LOGIN [saga-admin-login] WITH PASSWORD = 'm3d1atR1c3', DEFAULT_DATABASE = [master], CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF
GO

CREATE USER [saga-admin-user] FROM LOGIN [saga-admin-login]
GO

GRANT CREATE DATABASE TO [saga-admin-user]
GO

GRANT ALTER ANY DATABASE TO [saga-admin-login]
GO

GRANT CREATE LOGIN TO [saga-admin-login]
WITH GRANT OPTION
GO
*/