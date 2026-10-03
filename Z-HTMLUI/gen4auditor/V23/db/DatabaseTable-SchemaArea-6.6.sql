-- CockyAuditor build 6.6: Schema Area on the discovered EF tables (DatabaseTable).
-- You've already added this column; the script is safe to re-run on each host's database (it only adds what's missing).
IF COL_LENGTH('dbo.DatabaseTable', 'SchemaArea') IS NULL
    ALTER TABLE dbo.DatabaseTable ADD SchemaArea NVARCHAR(100) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DatabaseTable_SchemaArea' AND object_id = OBJECT_ID('dbo.DatabaseTable'))
    CREATE INDEX IX_DatabaseTable_SchemaArea ON dbo.DatabaseTable (SchemaArea);
GO
-- Model property (DatabaseTable.cs):
--     [StringLength(100)]
--     public string? SchemaArea { get; set; }
--
-- Important: the host-side discovery (SqlAzureTableAudit.RunAudit) must NOT overwrite SchemaArea when it refreshes a table
-- it already knows. Update only the measured columns (ColumnCount, ModelName, LastAuditDate, IsActive), e.g.
--     existing.ColumnCount = found.ColumnCount; existing.LastAuditDate = DateTime.UtcNow;   // leave existing.SchemaArea alone
