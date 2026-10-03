using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("DatabaseTables")]
    public class DatabaseTable
    {
        [Key]
        public int Id { get; set; }

        public string TableName { get; set; } = string.Empty;

        public string? SchemaName { get; set; }

        public string? ModelName { get; set; }

        public int ColumnCount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastAuditDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? SchemaArea {get; set;} //GENERAL AREA THAT SCHEMA IS USED -> USER FUNCTIONS, AUDIT FUNCTIONS, ETC
    }
}