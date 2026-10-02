namespace Enterprise.Models
{
    public class AuditPrincipal
    {
        public int Id { get; set; }

        public string PrincipalType { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        public string? Email { get; set; }

        public string? ObjectId { get; set; }

        public bool Active { get; set; }
    }
}
