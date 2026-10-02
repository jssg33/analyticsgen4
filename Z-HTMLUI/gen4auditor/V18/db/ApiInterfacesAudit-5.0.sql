-- CockyAuditor build 5.0: interfaces belong to a host (API) when discovered, and to an endpoint only once associated.
-- Safe to run more than once. Table/column names assume the EF defaults: ApiInterfacesAudit, Apihosts, Apiaudit.
-- Matching C# model (Enterprise.Models.ApiInterfacesAudit):
--     public int? ApiHostId { get; set; }     // host the interface was discovered on
--     public int? ApiAuditId { get; set; }    // endpoint it serves; null until associated on the Interfaces page

-- 1. ApiHostId (skip if you've already added it)
IF COL_LENGTH('dbo.ApiInterfacesAudit', 'ApiHostId') IS NULL
    ALTER TABLE dbo.ApiInterfacesAudit ADD ApiHostId INT NULL;
GO

-- 2. ApiAuditId must allow NULL. SQL Server won't alter a column a foreign key uses, so drop the FK, alter, put it back.
DECLARE @fk sysname, @sql nvarchar(max);
SELECT @fk = fk.name
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
WHERE fk.parent_object_id = OBJECT_ID('dbo.ApiInterfacesAudit') AND c.name = 'ApiAuditId';

IF @fk IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE dbo.ApiInterfacesAudit DROP CONSTRAINT ' + QUOTENAME(@fk);
    EXEC (@sql);
END

ALTER TABLE dbo.ApiInterfacesAudit ALTER COLUMN ApiAuditId INT NULL;

IF @fk IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE dbo.ApiInterfacesAudit WITH CHECK ADD CONSTRAINT ' + QUOTENAME(@fk) +
               N' FOREIGN KEY (ApiAuditId) REFERENCES dbo.Apiaudit (Id)';
    EXEC (@sql);
END
GO

-- 3. Foreign key + lookup index for the host
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ApiInterfacesAudit_Apihosts')
    ALTER TABLE dbo.ApiInterfacesAudit WITH CHECK ADD CONSTRAINT FK_ApiInterfacesAudit_Apihosts
        FOREIGN KEY (ApiHostId) REFERENCES dbo.Apihosts (Id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ApiInterfacesAudit_Host' AND object_id = OBJECT_ID('dbo.ApiInterfacesAudit'))
    CREATE INDEX IX_ApiInterfacesAudit_Host ON dbo.ApiInterfacesAudit (ApiHostId, InterfaceName, MethodName);
GO

-- Optional: the Interfaces page has a "Move to their host" button for rows saved by the old paste import.
-- This does the same in SQL (host from the endpoint, endpoint cleared):
-- UPDATE i SET i.ApiHostId = a.ApiHostId, i.ApiAuditId = NULL
-- FROM dbo.ApiInterfacesAudit i JOIN dbo.Apiaudit a ON a.Id = i.ApiAuditId
-- WHERE i.ApiHostId IS NULL;
