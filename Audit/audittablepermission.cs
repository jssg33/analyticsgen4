namespace Enterprise.Models
{
    public class AuditTablePermission
    {
        public int Id { get; set; }

        public int PrincipalId { get; set; }

        public int SqlTableId { get; set; }

        public bool CanSelect { get; set; }

        public bool CanInsert { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }
    }
}