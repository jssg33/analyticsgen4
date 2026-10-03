using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("Enterprise9")]
    public partial class Enterprise9
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ApiHostId { get; set; }

        public string ApiHostName { get; set; } = string.Empty;

        public string LogName { get; set; } = string.Empty;

        public string? LogDescription { get; set; }

        public string? EndpointUrl { get; set; }

        public string? HttpMethod { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool IsImplemented { get; set; }

        public DateTime? LastValidated { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;
        public int? ApiServiceId { get; set; }
        public string? ApiServiceName { get; set; }
    }
}