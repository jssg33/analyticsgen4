using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("AuditExceptions")]
    public class AuditException
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ApiAuditId { get; set; }

        public string? Reason { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool Active { get; set; } = true;

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
