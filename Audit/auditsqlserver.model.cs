using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    public class AuditSqlServer
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string ServerName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string DatabaseName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DatabaseType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DatabaseVersion { get; set; }

        [MaxLength(50)]
        public string? Environment { get; set; }

        [MaxLength(200)]
        public string? OwnerName { get; set; }

        [MaxLength(200)]
        public string? OwnerEmail { get; set; }

        [MaxLength(50)]
        public string? OwnerPhone { get; set; }

        [MaxLength(200)]
        public string? DbaName { get; set; }

        [MaxLength(200)]
        public string? DbaEmail { get; set; }

        [MaxLength(50)]
        public string? DbaPhone { get; set; }

        [MaxLength(200)]
        public string? DbContextName { get; set; }

        [MaxLength(200)]
        public string? ConnectionStringName { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Active { get; set; }

        public long? DatabaseSizeMB { get; set; }

        public string? BackupPolicy { get; set; }

        public bool GeoReplicationEnabled { get; set; }

        public bool TdeEnabled { get; set; }

        public bool DefenderEnabled { get; set; }
    }
}