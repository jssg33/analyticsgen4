namespace Enterprise.Models;
 
public class ApplicationApi
{
public int Id { get; set; }
 
public int ApplicationId { get; set; }
 
public int ApiHostId { get; set; }
 
public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
