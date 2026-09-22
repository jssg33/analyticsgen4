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

        // THE API HOST CONFIGURATION -> DICTATES WHAT THE SWAGGER WALK IS GOING TO DO....AND CAN POPULATE DEFAULT VALUES FOR EACH RECORD....
        // AS SUCH THE HOST TABLE SHOULD BE PRETTY CLOSE TO THE ENDPOINT RESULT.

        public int AuditorId { get; set; }
        public string? Family { get; set; }
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
        public bool SslSupported { get; set; }
        public bool HttpSupported { get; set; }
        public int? HttpPortV4 { get; set; }
        public int? HttpPortV6 { get; set; }
        public string? CorsPath { get; set; }
        public string? AllowedRanges { get; set; }
        public string? DeniedRanges { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Environment { get; set; }         // Dev/Test/UAT/Prod
        public string? ApplicationName { get; set; }
        public string? SourceRepository { get; set; }
        public Guid? AzureSubscriptionId { get; set; }
        public string? AzureResourceGroup { get; set; }
        public string? SecurityClassification { get; set; }

    }
}
