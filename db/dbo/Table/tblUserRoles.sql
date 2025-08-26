CREATE TABLE [dbo].[tblUserRoles]
(
	[flngUserKey] UNIQUEIDENTIFIER NOT NULL, 
    [flngRoleKey] UNIQUEIDENTIFIER NOT NULL,
	PRIMARY KEY ([flngUserKey], [flngRoleKey]),
	FOREIGN KEY ([flngUserKey]) REFERENCES tblUsers([flngUserKey]) ON DELETE CASCADE,
	FOREIGN KEY ([flngRoleKey]) REFERENCES tblRole([flngRoleKey]) ON DELETE CASCADE
)
