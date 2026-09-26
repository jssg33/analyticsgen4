
namespace Enterprise.Models
{
    public class ApiInterfacesAudit
    {
        public int Id { get; set; }

        public int ApiAuditId { get; set; }

        public string? ControllerName { get; set; }

        public string? Route { get; set; }

        public string? HttpMethod { get; set; }

        public string? InterfaceName { get; set; }

        public string? ImplementationName { get; set; }

        public string? ServiceLifetime { get; set; }

        public bool IsRegistered { get; set; }
        public string? MethodName { get; set; } 

        public DateTime DiscoveredDate { get; set; }
    }
}