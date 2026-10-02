using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    public class AuditSqlField
    {
        public int Id { get; set; }

        public int SqlTableId { get; set; }

        public string ColumnName { get; set; } = string.Empty;

        public string DataType { get; set; } = string.Empty;

        public int? MaxLength { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }

        public bool IsNullable { get; set; }

        public bool ContainsPII { get; set; }

        public bool ContainsPHI { get; set; }

        public bool IsEncrypted { get; set; }

        public string? Description { get; set; }

        public string? SensitivityLabel { get; set; }

        public string? MaskingRule { get; set; }
    }
}