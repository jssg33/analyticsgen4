using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Sessionlog
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public int? Hashid { get; set; }

    public DateTime? Sessionstart { get; set; }

    public DateTime? Sessionend { get; set; }

    public string? Moduleid { get; set; }

    public string? Description { get; set; }
}
