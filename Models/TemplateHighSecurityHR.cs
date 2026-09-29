namespace Enterprise.Models
{
    public class EmployeeHR
    {
        public int EmployeeId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string? SocialSecurityNo { get; set; }

        public decimal? Salary { get; set; }

        public int? TotalStockOptions { get; set; }
    }
}
