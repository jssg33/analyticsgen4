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
 
public bool IsActive { get; set; } = true;
 
public DateTime CreatedDate { get; set; }
 
public DateTime? ModifiedDate { get; set; }
}
