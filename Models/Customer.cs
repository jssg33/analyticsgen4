using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Customer
{
    public int Id { get; set; }
    public string? Tenantid {get; set;}
    public string UserId { get; set; } = null!;
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
}
