namespace Enterprise.Models
{
    public class DatabaseServer
    {
        public int Id { get; set; }

        public string? InventoryId { get; set; }

        public string? ServerName { get; set; }

        public string? Hostname { get; set; }

        public string? IpAddress { get; set; }

        public string? DatabasePlatform { get; set; }

        public string? Version { get; set; }

        public string? DatabaseInstance { get; set; }

        public string? AzureURL { get; set; }

        public string? AzureIP { get; set; }

        public string? CName1 { get; set; }

        public string? CName2 { get; set; }

        public string? CName3 { get; set; }

        public string? Environment { get; set; }

        public string? Location { get; set; }

        public string? AzureSubscription { get; set; }

        public string? ResourceGroup { get; set; }

        public string? Region { get; set; }

        public string? Owner { get; set; }

        public string? SupportTeam { get; set; }

        public string? BackupSolution { get; set; }

        public string? RecoveryModel { get; set; }

        public bool PublicFacing { get; set; }

        public bool Active { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}