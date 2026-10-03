using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace Enterprise.Models
{
[Table("ApiServices")]
public partial class ApiService
{
[Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public int Id { get; set; }
 
public string ServiceName { get; set; } = string.Empty;
 
public int? InterfaceId { get; set; }
 
public string? InterfaceName { get; set; }
 
public string? ImplementationClass { get; set; }
 
public string? Description { get; set; }
 
public string? ServiceType { get; set; }
 
public string? ServiceOwner { get; set; }
 
public bool IsActive { get; set; } = true;
 
public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
 
public DateTime? ModifiedDate { get; set; }
}
}