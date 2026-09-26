namespace Enterprise.Models
{
    public class WebFarm
    {
        public int Id { get; set; }

        public string? InventoryId { get; set; }

        public string? FarmName { get; set; }

        public string? Description { get; set; }

        public string? LoadBalancerName { get; set; }

        public string? LoadBalancerType { get; set; }

        public string? VirtualIpAddress { get; set; }

        public string? Url { get; set; }

        public string? AzureURL { get; set; }

        public string? AzureIP { get; set; }

        public string? CName1 { get; set; }

        public string? CName2 { get; set; }

        public string? CName3 { get; set; }

        public string? ApplicationURL { get; set; }

        public string? Environment { get; set; }

        public string? Location { get; set; }

        public string? AzureSubscription { get; set; }

        public string? ResourceGroup { get; set; }

        public string? Region { get; set; }

        public string? Owner { get; set; }

        public string? SupportTeam { get; set; }

        public string? TLSVersion { get; set; }

        public int? BackendPoolCount { get; set; }

        public string? HealthProbeUrl { get; set; }

        public bool PublicFacing { get; set; }

        public bool Enabled { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}