using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("Apihosts")]
    public class Apihost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ApiHostName { get; set; } = string.Empty;

        public string ApiHostUrl { get; set; } = string.Empty;

        public string? SwaggerUsername { get; set; }

        public string? SwaggerPassword { get; set; }

        public bool IsSecure { get; set; }

        public DateTime? LastAuditDate { get; set; }

        public int? LastAuditId { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
