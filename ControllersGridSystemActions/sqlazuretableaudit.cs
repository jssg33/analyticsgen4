using Microsoft.EntityFrameworkCore;
using System.Text;
using Enterprise.Models;

namespace EnterpriseControllers;

public static class SqlAzureTableAudit
{
    public static TableAuditControlBlock RunAudit()
    {
        var result = new TableAuditControlBlock();

        try
        {
            Directory.CreateDirectory("./IO");

            using var context = new EnterpriseContext();

            var sb = new StringBuilder();

            int recordCount = 0;
            int tablesInserted = 0;
            int tablesUpdated = 0;

            sb.AppendLine("================================================");
            sb.AppendLine("SQL AZURE TABLE AUDIT");
            sb.AppendLine("================================================");
            sb.AppendLine($"Generated UTC : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            //
            // ENTITY FRAMEWORK TABLE INVENTORY
            //
            sb.AppendLine("ENTITY FRAMEWORK TABLE INVENTORY");
            sb.AppendLine("---------------------------------------------");

            var entities = context.Model
                .GetEntityTypes()
                .Where(e => e.GetTableName() != null)
                .ToList();

            foreach (var entity in entities)
            {
                var tableName = entity.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                    continue;

                var schemaName = entity.GetSchema() ?? "dbo";
                var modelName = entity.ClrType.Name;
                var columnCount = entity.GetProperties().Count();

                sb.AppendLine(
                    $"{schemaName}.{tableName} | " +
                    $"Model={modelName} | " +
                    $"Columns={columnCount}");

                recordCount++;

                var existing = context.DatabaseTables
                    .FirstOrDefault(x =>
                        x.TableName == tableName &&
                        x.SchemaName == schemaName);

                if (existing == null)
                {
                    context.DatabaseTables.Add(
                        new DatabaseTable
                        {
                            TableName = tableName,
                            SchemaName = schemaName,
                            ModelName = modelName,
                            ColumnCount = columnCount,
                            CreatedDate = DateTime.UtcNow,
                            LastAuditDate = DateTime.UtcNow,
                            IsActive = true
                        });

                    tablesInserted++;
                }
                else
                {
                    existing.ModelName = modelName;
                    existing.ColumnCount = columnCount;
                    existing.LastAuditDate = DateTime.UtcNow;
                    existing.IsActive = true;

                    tablesUpdated++;
                }
            }

            context.SaveChanges();

            sb.AppendLine();
            sb.AppendLine("AUDIT SUMMARY");
            sb.AppendLine("---------------------------------------------");
            sb.AppendLine($"Entities Found : {entities.Count}");
            sb.AppendLine($"Inserted       : {tablesInserted}");
            sb.AppendLine($"Updated        : {tablesUpdated}");
            sb.AppendLine();

            sb.AppendLine("END OF REPORT");

            var fileName =
                $"sqlazuretableaudit_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";

            var filePath =
                Path.Combine("./IO", fileName);

            File.WriteAllText(filePath, sb.ToString());

            result.Success = true;
            result.AuditFileName = fileName;
            result.AuditFilePath = Path.GetFullPath(filePath);
            result.RecordCount = recordCount;
            result.ReportLength = sb.Length;
            result.AuditDateUTC = DateTime.UtcNow;
            result.TablesInserted = tablesInserted;
            result.TablesUpdated = tablesUpdated;

            result.AuditFiles = Directory
                .GetFiles("./IO")
                .Select(f => new TableAuditFileInfo
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
            result.TablesInserted = 0;
            result.TablesUpdated = 0;

            result.AuditFiles = new List<TableAuditFileInfo>
            {
                new TableAuditFileInfo
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

public class TableAuditControlBlock
{
    public bool Success { get; set; }

    public string? AuditFileName { get; set; }

    public string? AuditFilePath { get; set; }

    public int RecordCount { get; set; }

    public long ReportLength { get; set; }

    public DateTime AuditDateUTC { get; set; }

    public int TablesInserted { get; set; }

    public int TablesUpdated { get; set; }

    public List<TableAuditFileInfo> AuditFiles { get; set; } = new();
}

public class TableAuditFileInfo
{
    public string? FileName { get; set; }

    public long FileSize { get; set; }

    public DateTime LastModifiedUTC { get; set; }
}