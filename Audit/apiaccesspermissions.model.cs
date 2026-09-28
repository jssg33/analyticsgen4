using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("ApiAccessPermissions")]
    
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
}
