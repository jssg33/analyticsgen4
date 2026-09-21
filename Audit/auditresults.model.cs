using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("AuditResults")]
    public class AuditResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime AuditDate { get; set; } = DateTime.UtcNow;

        public int TotalApiHosts { get; set; }

        public int ApiHostsSecure { get; set; }

        public int ApiHostsInsecure { get; set; }

        public int TotalApiEndpoints { get; set; }

        public int TotalEndpointsSecure { get; set; }

        public int TotalEndpointsInsecure { get; set; }

        public int? AuditorId { get; set; }

        public string? AuditorName { get; set; }

        public string? Notes { get; set; }
    }
}
