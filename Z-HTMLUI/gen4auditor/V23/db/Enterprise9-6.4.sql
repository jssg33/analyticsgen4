-- Enterprise(9) per-host log setup (build 6.4).
-- One row per log a host exposes. Run against the same DB as ApiHost / Apiaudit.
-- DbSet in EnterpriseContext must be named Enterprise9s:
--   public DbSet<Enterprise9> Enterprise9s { get; set; }

CREATE TABLE dbo.Enterprise9
(
    Id             INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Enterprise9 PRIMARY KEY,
    ApiHostId      INT NULL,                 -- ApiHost.Id (nullable: an unassigned row won't break the list route)
    ApiHostName    VARCHAR(255) NULL,        -- denormalized for display / filtering
    LogName        VARCHAR(100) NOT NULL,    -- canonical set name, e.g. 'Apilog'
    LogDescription VARCHAR(255) NULL,        -- display label, e.g. 'API Log'
    EndpointUrl    VARCHAR(500) NULL,        -- path ('/api/Apilog') or full URL (region-specific base)
    HttpMethod     VARCHAR(10)  NULL,        -- usually GET
    IsRequired     BIT NOT NULL CONSTRAINT DF_Enterprise9_IsRequired    DEFAULT (0),
    IsImplemented  BIT NOT NULL CONSTRAINT DF_Enterprise9_IsImplemented DEFAULT (0),
    LastValidated  DATETIME2 NULL,
    Notes          VARCHAR(1000) NULL,
    IsActive       BIT NOT NULL CONSTRAINT DF_Enterprise9_IsActive      DEFAULT (1),
    CreatedDate    DATETIME2 NOT NULL CONSTRAINT DF_Enterprise9_CreatedDate DEFAULT (SYSUTCDATETIME()),
    ModifiedDate   DATETIME2 NULL
);
GO

-- Fast per-host lookups (GET /api/enterprise9/host/{apiHostId}).
CREATE NONCLUSTERED INDEX IX_Enterprise9_ApiHostId
    ON dbo.Enterprise9 (ApiHostId);
GO

-- One config per (host, log). Drop this if you intentionally want duplicates.
CREATE UNIQUE NONCLUSTERED INDEX UX_Enterprise9_Host_Log
    ON dbo.Enterprise9 (ApiHostId, LogName)
    WHERE ApiHostId IS NOT NULL;
GO
