using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Enterprise.Models;

namespace EnterpriseControllers;

public static class SqlAzureSecurityAudit
{
    public static AuditControlBlock RunAudit()
    {
        var result = new AuditControlBlock();

        try
        {
            Directory.CreateDirectory("./IO");

            using var context = new EnterpriseContext();

            var sb = new StringBuilder();
            int recordCount = 0;

            sb.AppendLine("================================================");
            sb.AppendLine("SQL AZURE SECURITY AUDIT");
            sb.AppendLine("================================================");
            sb.AppendLine($"Generated UTC : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            var conn = context.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();

            //
            // DATABASE INFORMATION
            //
            sb.AppendLine("DATABASE INFORMATION");
            sb.AppendLine("---------------------------------------------");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT
                        DB_NAME() AS DatabaseName,
                        @@VERSION AS SqlVersion
                ";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine($"Database : {reader["DatabaseName"]}");
                    sb.AppendLine($"Version  : {reader["SqlVersion"]}");
                    recordCount++;
                }
            }

            sb.AppendLine();

            //
            // DATABASE USERS
            //
            sb.AppendLine("DATABASE USERS");
            sb.AppendLine("---------------------------------------------");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT
                        name,
                        type_desc,
                        create_date
                    FROM sys.database_principals
                    WHERE principal_id > 4
                    ORDER BY name";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(
                        $"{reader["name"]} | " +
                        $"{reader["type_desc"]} | " +
                        $"{reader["create_date"]}");

                    recordCount++;
                }
            }

            sb.AppendLine();

            //
            // ROLE MEMBERSHIPS
            //
            sb.AppendLine("ROLE MEMBERSHIPS");
            sb.AppendLine("---------------------------------------------");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT
                        roles.name AS RoleName,
                        members.name AS MemberName
                    FROM sys.database_role_members drm
                    INNER JOIN sys.database_principals roles
                        ON drm.role_principal_id = roles.principal_id
                    INNER JOIN sys.database_principals members
                        ON drm.member_principal_id = members.principal_id
                    ORDER BY roles.name, members.name";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(
                        $"{reader["RoleName"]} -> {reader["MemberName"]}");

                    recordCount++;
                }
            }

            sb.AppendLine();

            //
            // EXPLICIT PERMISSIONS
            //
            sb.AppendLine("EXPLICIT PERMISSIONS");
            sb.AppendLine("---------------------------------------------");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT
                        dp.name,
                        perm.permission_name,
                        perm.state_desc,
                        OBJECT_NAME(perm.major_id) AS ObjectName
                    FROM sys.database_permissions perm
                    INNER JOIN sys.database_principals dp
                        ON perm.grantee_principal_id = dp.principal_id
                    ORDER BY dp.name";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(
                        $"{reader["name"]} | " +
                        $"{reader["permission_name"]} | " +
                        $"{reader["state_desc"]} | " +
                        $"{reader["ObjectName"]}");

                    recordCount++;
                }
            }

            sb.AppendLine();

            //
            // TABLE INVENTORY
            //
            sb.AppendLine("TABLE INVENTORY");
            sb.AppendLine("---------------------------------------------");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT
                        TABLE_SCHEMA,
                        TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    ORDER BY TABLE_SCHEMA,TABLE_NAME";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(
                        $"{reader["TABLE_SCHEMA"]}.{reader["TABLE_NAME"]}");

                    recordCount++;
                }
            }

            sb.AppendLine();

            //
            // ENTITY FRAMEWORK MODEL
            //
            sb.AppendLine("ENTITY FRAMEWORK MODEL");
            sb.AppendLine("---------------------------------------------");

            foreach (var entity in context.Model.GetEntityTypes())
            {
                sb.AppendLine(entity.ClrType.Name);
                recordCount++;

                foreach (var property in entity.GetProperties())
                {
                    sb.AppendLine(
                        $"    {property.Name} ({property.ClrType.Name})");

                    recordCount++;
                }

                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("END OF REPORT");

            var fileName =
                $"sqlazuresecurityaudit_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";

            var filePath = Path.Combine("./IO", fileName);

            File.WriteAllText(filePath, sb.ToString());

            result.Success = true;
            result.AuditFileName = fileName;
            result.AuditFilePath = Path.GetFullPath(filePath);
            result.RecordCount = recordCount;
            result.ReportLength = sb.Length;
            result.AuditDateUTC = DateTime.UtcNow;

            result.AuditFiles = Directory
                .GetFiles("./IO")
                .Select(f => new AuditFileInfo
                {
                    FileName = Path.GetFileName(f),
                    FileSize = new FileInfo(f).Length,
                    LastModifiedUTC = File.GetLastWriteTimeUtc(f)
                })
                .OrderByDescending(x => x.LastModifiedUTC)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.AuditFileName = null;
            result.AuditFilePath = null;
            result.RecordCount = 0;
            result.ReportLength = 0;
            result.AuditDateUTC = DateTime.UtcNow;

            result.AuditFiles = new List<AuditFileInfo>
            {
                new AuditFileInfo
                {
                    FileName = ex.Message,
                    FileSize = 0,
                    LastModifiedUTC = DateTime.UtcNow
                }
            };

            return result;
        }
    }
}

public class AuditControlBlock
{
    public bool Success { get; set; }

    public string? AuditFileName { get; set; }

    public string? AuditFilePath { get; set; }

    public int RecordCount { get; set; }

    public long ReportLength { get; set; }

    public DateTime AuditDateUTC { get; set; }

    public List<AuditFileInfo> AuditFiles { get; set; } = new();
}

public class AuditFileInfo
{
    public string? FileName { get; set; }

    public long FileSize { get; set; }

    public DateTime LastModifiedUTC { get; set; }
}