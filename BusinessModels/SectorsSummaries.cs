using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Sectorsummary
{
    public int Id { get; set; }

    public string? Company { get; set; }

    public string? Ticker { get; set; }

    public string? Sector { get; set; }

    public double? Avgdividend { get; set; }

    public double? Totaldividends { get; set; }

    public double? PriceStart { get; set; }

    public double? PriceEnd { get; set; }

    public double? Change { get; set; }

    public double? Totalreturn { get; set; }

    public double? Totalreturnover10 { get; set; }

    public double? Shares500 { get; set; }

    public double? Totalspend { get; set; }

    public double? Fiveyearequityproj { get; set; }

    public double? Fiveyeardivproj { get; set; }

    public double? Totalfiveyearview { get; set; }

    public string? Selected { get; set; }

    public string? useridasstring { get; set; } //ADDED-0828

    public int? userid { get; set; } //ADDED-0828
}
