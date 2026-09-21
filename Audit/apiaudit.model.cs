using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("Apiaudit")]
    public class Apiaudit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AuditorId { get; set; }

        public string? Description { get; set; }
        public string? Family { get; set; }
        public string? Url { get; set; }
        public string? ApiRoot { get; set; }

        public string? Type { get; set; } // SqlOnly, NetworkEmbedded, ConsoleApp

        public bool ServicesAttached { get; set; }
        public string? ServicesJson { get; set; }

        public string? IPv4Address { get; set; }
        public string? IPv6Address { get; set; }

        public string? HostName { get; set; }

        public bool HasProxy { get; set; }

        public string? ProxyEntranceV4 { get; set; }
        public string? ProxyEntranceV6 { get; set; }

        public string? ProxyType { get; set; }

        public string? DatabaseFramework { get; set; }
        public string? DatabaseConnectionString { get; set; }
        public string? DatabaseType { get; set; }

        public bool HasAuthEnabled { get; set; }
        public string? AuthType { get; set; }

        public string? AuditorName { get; set; }
        public string? AuditorEmail { get; set; }
        public string? AuditorEmployeeId { get; set; }
        public string? AuditorDepartment { get; set; }

        public string? BusinessUnitName { get; set; }
        public int BusinessUnitOwnerId { get; set; }

        public int? PrimaryPort { get; set; }
        public int? SecondaryPort { get; set; }

        public string? SecondaryV4 { get; set; }
        public string? SecondaryV6 { get; set; }

        public string? FQDN { get; set; }

        public string? OSType { get; set; }
        public string? FrameworkVersion { get; set; }

        public int GroupId { get; set; }
        public string? GroupDescription { get; set; }

        public int BuildingId { get; set; }
        public string? BuildingName { get; set; }

        public string? TechContactName { get; set; }
        public string? TechContactEmail { get; set; }
        public string? TechContactFax { get; set; }
        public string? TechContactPhone { get; set; }

        public string? SecurityEmail { get; set; }
        public string? SecurityPhone { get; set; }

        public int DBAId { get; set; }
        public string? DBAName { get; set; }

        public string? HashType { get; set; }

        public string? EndpointType { get; set; } // GET POST PUT DELETE

        public bool SslSupported { get; set; }
        public bool HttpSupported { get; set; }

        public int? HttpPortV4 { get; set; }
        public int? HttpPortV6 { get; set; }

        public string? CorsPath { get; set; }

        public string? AllowedRanges { get; set; }
        public string? DeniedRanges { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? LastAuditDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Environment { get; set; }         // Dev/Test/UAT/Prod
        public string? ApplicationName { get; set; }
        public string? SourceRepository { get; set; }

        public Guid? AzureSubscriptionId { get; set; }

        public string? AzureResourceGroup { get; set; }
        public int? ApiHostId { get; set; }
        public int? AuditResultId { get; set; }
        public bool EndpointSecure { get; set; }
        public string? SwaggerOperationId { get; set; }
        public string? SwaggerVersion { get; set; }
        public string? Notes { get; set; }
        public string? SecurityClassification { get; set; }
    }
}
