namespace Enterprise.Models
{
public class Database
{
public int Id { get; set; }
public string? InventoryId { get; set; }
public int DatabaseServerId { get; set; }
public string? DatabaseName { get; set; }
public string? Description { get; set; }
public string? DatabaseType { get; set; }
public string? Version { get; set; }
public string? Classification { get; set; }
public string? Owner { get; set; }
public string? SupportTeam { get; set; }
public bool BackupRequired { get; set; }
public bool EncryptionEnabled { get; set; }
public bool ContainsPII { get; set; }
public bool ContainsPHI { get; set; }
public bool ContainsPCI { get; set; }
public bool Active { get; set; }
public string? Notes { get; set; }
public DateTime CreatedDate { get; set; }
}
}
