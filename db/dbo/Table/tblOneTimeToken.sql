CREATE TABLE [dbo].[tblOneTimeToken]
(
	[flngTokenKey] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [flngUserKey] UNIQUEIDENTIFIER NOT NULL, 
    [fstrPurpose] NVARCHAR(50) NOT NULL, 
    [fstrTokenHash] VARBINARY(64) NOT NULL, 
    [fdtmExpiresAt] DATETIME2 NOT NULL, 
    [fdtmConsumedAt] DATETIME2 NULL,
    FOREIGN KEY ([flngUserKey]) REFERENCES tblUsers([flngUserKey]) ON DELETE CASCADE
)
