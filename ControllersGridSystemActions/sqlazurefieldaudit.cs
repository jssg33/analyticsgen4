using Enterprise.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnterpriseControllers;

public static class SqlAzureFieldAudit
{
    public static TableAuditControlBlock RunAudit()
    {
        var result = new TableAuditControlBlock();

        try
        {
            using var context = new EnterpriseContext();

            int inserted = 0;
            int updated = 0;
            int inspected = 0;

            var entities = context.Model.GetEntityTypes();

            foreach (var entity in entities)
            {
                var tableName = entity.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                    continue;

                var schemaName = entity.GetSchema() ?? "dbo";

                foreach (var property in entity.GetProperties())
                {
                    inspected++;

                    var fieldName = property.Name;

                    var clrType = property.ClrType.Name;

                    var dataType =
                        property.GetColumnType();

                    var maxLength =
                        property.GetMaxLength();

                    var isNullable =
                        property.IsNullable;

                    var pk =
                        entity.FindPrimaryKey();

                    bool isPrimaryKey =
                        pk != null &&
                        pk.Properties.Any(
                            x => x.Name == property.Name);

                    var existing =
                        context.DatabaseFields
                            .FirstOrDefault(x =>
                                x.TableName == tableName &&
                                x.FieldName == fieldName);

                    if (existing == null)
                    {
                        context.DatabaseFields.Add(
                            new DatabaseField
                            {
                                TableName = tableName,
                                SchemaName = schemaName,
                                FieldName = fieldName,
                                DataType = dataType,
                                ClrType = clrType,
                                IsNullable = isNullable,
                                MaxLength = maxLength,
                                IsPrimaryKey = isPrimaryKey,
                                CreatedDate = DateTime.UtcNow,
                                LastAuditDate = DateTime.UtcNow,
                                IsActive = true
                            });

                        inserted++;
                    }
                    else
                    {
                        existing.DataType = dataType;
                        existing.ClrType = clrType;
                        existing.IsNullable = isNullable;
                        existing.MaxLength = maxLength;
                        existing.IsPrimaryKey = isPrimaryKey;
                        existing.LastAuditDate = DateTime.UtcNow;

                        updated++;
                    }
                }
            }

            context.SaveChanges();

            result.Success = true;
            result.RecordCount = inspected;
            result.TablesInserted = inserted;
            result.TablesUpdated = updated;
            result.AuditDateUTC = DateTime.UtcNow;

            return result;
        }
        catch
        {
            result.Success = false;
            result.AuditDateUTC = DateTime.UtcNow;

            return result;
        }
    }
}