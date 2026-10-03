-- Join table: ApiService <-> Apiaudit endpoint (many-to-many), scoped by host.
-- Run against the same DB as ApiServices / Apiaudit.

CREATE TABLE dbo.ApiServiceEndpoints
(
    Id          INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_ApiServiceEndpoints PRIMARY KEY,
    ServiceId   INT NOT NULL,       -- ApiServices.Id
    ApiAuditId  INT NOT NULL,       -- Apiaudit.Id (endpoint)
    ApiHostId   INT NULL,           -- denormalized from the service
    CreatedDate DATETIME2 NOT NULL
        CONSTRAINT DF_ApiServiceEndpoints_CreatedDate DEFAULT (SYSUTCDATETIME())
);
GO

-- One link per (service, endpoint): makes re-dropping idempotent at the DB level.
CREATE UNIQUE NONCLUSTERED INDEX UX_ApiServiceEndpoints_Service_Endpoint
    ON dbo.ApiServiceEndpoints (ServiceId, ApiAuditId);
GO

-- Fast "which services use this endpoint" lookups.
CREATE NONCLUSTERED INDEX IX_ApiServiceEndpoints_ApiAuditId
    ON dbo.ApiServiceEndpoints (ApiAuditId);
GO

-- Fast host-scoped filtering.
CREATE NONCLUSTERED INDEX IX_ApiServiceEndpoints_ApiHostId
    ON dbo.ApiServiceEndpoints (ApiHostId);
GO

-- Optional FKs (uncomment if you want DB-enforced referential integrity;
-- your other tables use loose keys, so these are off by default):
-- ALTER TABLE dbo.ApiServiceEndpoints ADD CONSTRAINT FK_ASE_Service
--     FOREIGN KEY (ServiceId)  REFERENCES dbo.ApiServices (Id) ON DELETE CASCADE;
-- ALTER TABLE dbo.ApiServiceEndpoints ADD CONSTRAINT FK_ASE_Endpoint
--     FOREIGN KEY (ApiAuditId) REFERENCES dbo.Apiaudit   (Id) ON DELETE CASCADE;
-- GO
