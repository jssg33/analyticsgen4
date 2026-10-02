using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{   
    public class AuditSqlTable
    {
        public int Id { get; set; }

        public int SqlServerId { get; set; }

        public string SchemaName { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool ContainsPHI { get; set; }

        public bool ContainsPII { get; set; }

        public long? SomeRowCount { get; set; }

        public decimal? TableSizeMB { get; set; }

        public DateTime? LastStatisticsUpdate { get; set; }

        public bool HasRowLevelSecurity { get; set; }

        public bool HasDynamicDataMasking { get; set; }

        public string? ClassificationStatus { get; set; }
    }
}