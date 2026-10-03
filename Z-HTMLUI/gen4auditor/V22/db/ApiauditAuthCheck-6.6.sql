-- CockyAuditor build 6.6: results of Run Audit's token / auth check, stamped on each endpoint (Apiaudit).
--   AuthEnforced    1 = refuses calls without a valid token / login (strict compliant), 0 = doesn't, NULL = not verified
--   CorsRestricted  1 = CORS names specific origins (or blocks the auditor), 0 = wildcard (*), NULL = not checked
--   IpRestricted    1 = private address or Allowed Ranges recorded, 0 = public with no ranges, NULL = not checked
--   Practical compliance = AuthEnforced = 1 OR (CorsRestricted = 1 AND IpRestricted = 1)
IF COL_LENGTH('dbo.Apiaudit', 'AuthEnforced') IS NULL     ALTER TABLE dbo.Apiaudit ADD AuthEnforced BIT NULL;
IF COL_LENGTH('dbo.Apiaudit', 'CorsRestricted') IS NULL   ALTER TABLE dbo.Apiaudit ADD CorsRestricted BIT NULL;
IF COL_LENGTH('dbo.Apiaudit', 'IpRestricted') IS NULL     ALTER TABLE dbo.Apiaudit ADD IpRestricted BIT NULL;
IF COL_LENGTH('dbo.Apiaudit', 'AuthCheckResult') IS NULL  ALTER TABLE dbo.Apiaudit ADD AuthCheckResult NVARCHAR(400) NULL;
IF COL_LENGTH('dbo.Apiaudit', 'AuthCheckDate') IS NULL    ALTER TABLE dbo.Apiaudit ADD AuthCheckDate DATETIME2 NULL;
GO
-- Model properties (Apiaudit.cs), so the existing PUT /api/Apiaudit/{id} saves them:
--     public bool? AuthEnforced { get; set; }
--     public bool? CorsRestricted { get; set; }
--     public bool? IpRestricted { get; set; }
--     [StringLength(400)] public string? AuthCheckResult { get; set; }
--     public DateTime? AuthCheckDate { get; set; }
--
-- Strict vs practical, per host:
-- SELECT a.ApiHostId,
--        COUNT(*) AS Endpoints,
--        SUM(CASE WHEN a.AuthEnforced = 1 THEN 1 ELSE 0 END) AS StrictCompliant,
--        SUM(CASE WHEN a.AuthEnforced = 1 OR (a.CorsRestricted = 1 AND a.IpRestricted = 1) THEN 1 ELSE 0 END) AS PracticalCompliant
-- FROM dbo.Apiaudit a WHERE a.IsActive = 1 GROUP BY a.ApiHostId;
