namespace Enterprise.Models;
 
public class Application
{
public int Id { get; set; }
public string ApplicationName { get; set; } = string.Empty;
public string? ApplicationCode { get; set; }
public string? Description { get; set; }
public string? OwnerName { get; set; }
public string? OwnerEmail { get; set; }
public string? SupportGroup { get; set; }
public string? BusinessUnit { get; set; }
public string? Environment { get; set; }
public string? InventoryId { get; set; }
public int? WebServerId { get; set; }
public int? WebFarmId { get; set; }
public int? DatabaseId { get; set; }
public bool? IsActive { get; set; } = true;
public string? UI_Codebase { get; set; }
public string? APIVendor { get; set; }
public string? UI_HTMLVendor { get; set; }
public string? UI_REACTVendor { get; set; }
public string? OS_UIHTML { get; set; }
public string? OS_UIREACT { get; set; }
public string? OS_API { get; set; }

public DateTime CreatedDate { get; set; }

public DateTime? ModifiedDate { get; set; }
}
