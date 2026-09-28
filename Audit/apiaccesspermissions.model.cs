public class ApiAccessPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ApiId { get; set; }

    public string? PermissionLevel { get; set; }

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedDate { get; set; }

    public string? GrantedBy { get; set; }
}
