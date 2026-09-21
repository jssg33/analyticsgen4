using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Adminlog
{
    public int Id { get; set; }

    public string Userid { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? Description { get; set; }

    public string? Acknowledged { get; set; }

    public int? Techid { get; set; }

    public int? Managerescid { get; set; }

    public string? Threatlevel { get; set; }
}
