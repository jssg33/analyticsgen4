namespace Enterprise.Models;

public class Site
{
    public int Id { get; set; }

    public string SiteName { get; set; } = string.Empty;

    public string? SiteCode { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? PhoneNumber { get; set; }

    public string? SiteManager { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }
}
