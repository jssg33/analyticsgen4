namespace Enterprise.Models
{
    public class AuditFieldPermission
    {
        public int Id { get; set; }

        public int PrincipalId { get; set; }

        public int SqlFieldId { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanMask { get; set; }

        public bool CanDecrypt { get; set; }
    }
}