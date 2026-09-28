namespace Enterprise.Models;

public class ApplicationUser
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public int UserId { get; set; }

    public string? Permission { get; set; }

    public string? RoleName { get; set; }

    public string? AccessType { get; set; }      // User, Service Account, Shared Account

    public string? ApprovalGroup { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? LastReviewedDate { get; set; }

    public string? ReviewedBy { get; set; }

    public string? RequestTicket { get; set; }

    public string? BusinessJustification { get; set; }

    public DateTime? AccessGrantedDate { get; set; }

    public DateTime? AccessRemovedDate { get; set; }
}