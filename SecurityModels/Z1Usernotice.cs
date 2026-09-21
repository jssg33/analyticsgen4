using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Usernotice
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public DateTime? NoticeDatetime { get; set; }

    public string? Noticetype { get; set; }

    public string? Emailgwtype { get; set; }

    public int? Userid { get; set; }

    public string? Useridstring { get; set; }

    public string? Emailaddress { get; set; }
}
