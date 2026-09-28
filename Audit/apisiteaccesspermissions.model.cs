using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Models
{
    [Table("SiteAccessPermissions")]

public class SiteAccessPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string? AccessLevel { get; set; }

    public bool IsEnabled { get; set; } = true;

    public DateTime GrantedDate { get; set; }

    public string? GrantedBy { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    public string? ReviewedBy { get; set; }
}
}
