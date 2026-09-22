public class ApiHostException
{
    public int Id { get; set; }

    public int ApiHostId { get; set; }

    public string? ExceptionType { get; set; }
    public string? Reason { get; set; }

    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public bool Active { get; set; } = true;

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }
}