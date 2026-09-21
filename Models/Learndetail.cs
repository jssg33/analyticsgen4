using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Learndetail
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Startdate { get; set; }

    public string? Enddate { get; set; }

    public string? Certauthority { get; set; }

    public string? Status { get; set; }

    public int? Userid { get; set; }

    public int? Employeeidasint { get; set; }

    public string? Employeeid { get; set; }

    public int? Employee { get; set; }

    public int? Learningmodulesid { get; set; }

    public string? Cataloguesku { get; set; }

    public string? Fullname { get; set; }
}
