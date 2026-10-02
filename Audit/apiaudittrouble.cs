using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    public class WorkerTroubleTicket
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? TicketNumber { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(200)]
        public string? ApiName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Endpoint { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Environment { get; set; } = "Production";

        [MaxLength(50)]
        public string? Severity { get; set; } = "Critical";

        [MaxLength(50)]
        public string? Status { get; set; } = "Open";

        [MaxLength(200)]
        public string? ReportedBy { get; set; } = string.Empty;

        public DateTime ReportedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedOn { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? ResolutionNotes { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? AssignedTo { get; set; } = string.Empty;

        public bool IsHardDown { get; set; }

        public int? IncidentCount { get; set; } = 1;
        public int? ImpactedUsers { get; set; }
 
        [MaxLength(100)]
        public string? BusinessUnit { get; set; }
 
        [MaxLength(2000)]
        public string? RootCause { get; set; }
    
        [MaxLength(1000)]
        public string? EvidenceUrl { get; set; }
 
        [MaxLength(4000)]
        public string? AuditorNotes { get; set; }
 
        [MaxLength(200)]
        public string? ApplicationOwner { get; set; }
    }
}