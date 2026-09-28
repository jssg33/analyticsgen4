CREATE TABLE SiteAccessPermissions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,
    SiteId INT NOT NULL,

    AccessLevel VARCHAR(100) NULL,

    IsEnabled BIT NOT NULL DEFAULT(1),

    GrantedDate DATETIME NOT NULL DEFAULT(GETDATE()),
    GrantedBy VARCHAR(255) NULL,

    ExpirationDate DATETIME NULL,

    ReviewedDate DATETIME NULL,
    ReviewedBy VARCHAR(255) NULL
);
