CREATE TABLE [dbo].[tblRefreshTokens]
(
	[flngRefreshKey] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [flngUserKey] UNIQUEIDENTIFIER NOT NULL, 
    [fstrTokenHash] VARBINARY(64) NOT NULL, 
    [fdtmIssueAt] DATETIME2 NOT NULL, 
    [fdtmExpiresAt] DATETIME2 NOT NULL, 
    [fstrDevice] NVARCHAR(200) NULL, 
    [fstrIpAddress] NVARCHAR(64) NULL, 
    [fdtmRevokedAt] DATETIME2 NULL, 
    [fstrReplacedByTokenHash] VARBINARY(64) NULL,
    FOREIGN KEY ([flngUserKey]) REFERENCES tblUsers([flngUserKey]) ON DELETE CASCADE
)
