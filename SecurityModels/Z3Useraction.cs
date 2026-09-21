using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Useraction
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public string? Description { get; set; }

    public int? Acknowledged { get; set; }

    public int? Actionpriority { get; set; }

    public string? Actiondate { get; set; }
}
