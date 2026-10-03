/*==============================================================
  AUDIT SQL INVENTORY
==============================================================*/

CREATE TABLE AuditSqlServers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    ServerName NVARCHAR(200) NOT NULL,
    DatabaseName NVARCHAR(200) NOT NULL,
    DatabaseType NVARCHAR(50) NOT NULL,
    DatabaseVersion NVARCHAR(100) NULL,

    Environment NVARCHAR(50) NULL,

    OwnerName NVARCHAR(200) NULL,
    OwnerEmail NVARCHAR(200) NULL,
    OwnerPhone NVARCHAR(50) NULL,

    DbaName NVARCHAR(200) NULL,
    DbaEmail NVARCHAR(200) NULL,
    DbaPhone NVARCHAR(50) NULL,

    DbContextName NVARCHAR(200) NULL,
    ConnectionStringName NVARCHAR(200) NULL,

    Description NVARCHAR(MAX) NULL,

    CreatedDate DATETIME2 NOT NULL
        DEFAULT SYSUTCDATETIME(),

    Active BIT NOT NULL
        DEFAULT(1),

    DatabaseSizeMB BIGINT NULL,

    BackupPolicy NVARCHAR(200) NULL,

    GeoReplicationEnabled BIT NOT NULL
        DEFAULT(0),

    TdeEnabled BIT NOT NULL
        DEFAULT(0),

    DefenderEnabled BIT NOT NULL
        DEFAULT(0)
);
GO

/*==============================================================
  AUDIT SQL TABLES
==============================================================*/

CREATE TABLE AuditSqlTables
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    SqlServerId INT NOT NULL,

    SchemaName NVARCHAR(100) NOT NULL,

    TableName NVARCHAR(200) NOT NULL,

    Description NVARCHAR(2000) NULL,

    ContainsPHI BIT NOT NULL
        DEFAULT(0),

    ContainsPII BIT NOT NULL
        DEFAULT(0),

    SomeRowCount BIGINT NULL,

    TableSizeMB DECIMAL(18,2) NULL,

    LastStatisticsUpdate DATETIME2 NULL,

    HasRowLevelSecurity BIT NOT NULL
        DEFAULT(0),

    HasDynamicDataMasking BIT NOT NULL
        DEFAULT(0),

    ClassificationStatus NVARCHAR(50) NULL,

    CONSTRAINT FK_AuditSqlTables_Server
        FOREIGN KEY (SqlServerId)
        REFERENCES AuditSqlServers(Id)
);
GO

/*==============================================================
  AUDIT SQL FIELDS
==============================================================*/

CREATE TABLE AuditSqlFields
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    SqlTableId INT NOT NULL,

    ColumnName NVARCHAR(200) NOT NULL,

    DataType NVARCHAR(100) NOT NULL,

    MaxLength INT NULL,

    IsPrimaryKey BIT NOT NULL
        DEFAULT(0),

    IsForeignKey BIT NOT NULL
        DEFAULT(0),

    IsNullable BIT NOT NULL
        DEFAULT(1),

    ContainsPII BIT NOT NULL
        DEFAULT(0),

    ContainsPHI BIT NOT NULL
        DEFAULT(0),

    IsEncrypted BIT NOT NULL
        DEFAULT(0),

    Description NVARCHAR(2000) NULL,

    SensitivityLabel NVARCHAR(100) NULL,

    MaskingRule NVARCHAR(500) NULL,

    CONSTRAINT FK_AuditSqlFields_Table
        FOREIGN KEY (SqlTableId)
        REFERENCES AuditSqlTables(Id)
);
GO

/*==============================================================
  AUDIT PRINCIPALS
==============================================================*/

CREATE TABLE AuditPrincipals
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    PrincipalType NVARCHAR(50) NOT NULL,

    UserName NVARCHAR(200) NOT NULL,

    DisplayName NVARCHAR(200) NULL,

    Email NVARCHAR(200) NULL,

    ObjectId NVARCHAR(100) NULL,

    Active BIT NOT NULL
        DEFAULT(1)
);
GO

/*==============================================================
  SERVER PERMISSIONS
==============================================================*/

CREATE TABLE AuditServerPermissions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    PrincipalId INT NOT NULL,

    SqlServerId INT NOT NULL,

    PermissionLevel NVARCHAR(100) NOT NULL,

    GrantedDate DATETIME2 NULL,

    RevokedDate DATETIME2 NULL,

    CONSTRAINT FK_AuditServerPermissions_Principal
        FOREIGN KEY (PrincipalId)
        REFERENCES AuditPrincipals(Id),

    CONSTRAINT FK_AuditServerPermissions_Server
        FOREIGN KEY (SqlServerId)
        REFERENCES AuditSqlServers(Id)
);
GO

/*==============================================================
  TABLE PERMISSIONS
==============================================================*/

CREATE TABLE AuditTablePermissions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    PrincipalId INT NOT NULL,

    SqlTableId INT NOT NULL,

    CanSelect BIT NOT NULL
        DEFAULT(0),

    CanInsert BIT NOT NULL
        DEFAULT(0),

    CanUpdate BIT NOT NULL
        DEFAULT(0),

    CanDelete BIT NOT NULL
        DEFAULT(0),

    CONSTRAINT FK_AuditTablePermissions_Principal
        FOREIGN KEY (PrincipalId)
        REFERENCES AuditPrincipals(Id),

    CONSTRAINT FK_AuditTablePermissions_Table
        FOREIGN KEY (SqlTableId)
        REFERENCES AuditSqlTables(Id)
);
GO

/*==============================================================
  FIELD PERMISSIONS
==============================================================*/

CREATE TABLE AuditFieldPermissions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    PrincipalId INT NOT NULL,

    SqlFieldId INT NOT NULL,

    CanRead BIT NOT NULL
        DEFAULT(0),

    CanUpdate BIT NOT NULL
        DEFAULT(0),

    CanMask BIT NOT NULL
        DEFAULT(0),

    CanDecrypt BIT NOT NULL
        DEFAULT(0),

    CONSTRAINT FK_AuditFieldPermissions_Principal
        FOREIGN KEY (PrincipalId)
        REFERENCES AuditPrincipals(Id),

    CONSTRAINT FK_AuditFieldPermissions_Field
        FOREIGN KEY (SqlFieldId)
        REFERENCES AuditSqlFields(Id)
);
GO

/*==============================================================
  PERFORMANCE INDEXES
==============================================================*/

CREATE INDEX IX_AuditSqlTables_ServerId
ON AuditSqlTables(SqlServerId);

CREATE INDEX IX_AuditSqlFields_TableId
ON AuditSqlFields(SqlTableId);

CREATE INDEX IX_AuditServerPermissions_PrincipalId
ON AuditServerPermissions(PrincipalId);

CREATE INDEX IX_AuditServerPermissions_ServerId
ON AuditServerPermissions(SqlServerId);

CREATE INDEX IX_AuditTablePermissions_PrincipalId
ON AuditTablePermissions(PrincipalId);

CREATE INDEX IX_AuditTablePermissions_TableId
ON AuditTablePermissions(SqlTableId);

CREATE INDEX IX_AuditFieldPermissions_PrincipalId
ON AuditFieldPermissions(PrincipalId);

CREATE INDEX IX_AuditFieldPermissions_FieldId
ON AuditFieldPermissions(SqlFieldId);
GO