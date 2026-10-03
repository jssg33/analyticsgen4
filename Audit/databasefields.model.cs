using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models;

[Table("DatabaseFields")]
public class DatabaseField
{
    [Key]
    public int Id { get; set; }

    public int? DatabaseTableId { get; set; }

    public string TableName { get; set; } = string.Empty;

    public string? SchemaName { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string? DataType { get; set; }

    public string? ClrType { get; set; }

    public bool IsNullable { get; set; }

    public int? MaxLength { get; set; }

    public bool IsPrimaryKey { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastAuditDate { get; set; }

    public bool IsActive { get; set; } = true;
}