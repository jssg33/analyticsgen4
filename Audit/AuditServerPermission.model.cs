namespace Enterprise.Models
{
    public class AuditServerPermission
    {
        public int Id { get; set; }

        public int PrincipalId { get; set; }

        public int SqlServerId { get; set; }

        public string PermissionLevel { get; set; } = string.Empty;

        public DateTime? GrantedDate { get; set; }

        public DateTime? RevokedDate { get; set; }
    }
}