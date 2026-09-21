using System;
using System.Collections.Generic;
namespace Enterprise.Models;
public partial class Lunalog
{
public int Id { get; set; }
public int? Apiid { get; set; }
public DateTime? AccessTime { get; set; }
public string? QueryText { get; set; }
public string? SourceIpAddress { get; set; }
public string? DestinationIpAddress { get; set; }
public string? Uid { get; set; }
public string? User1 { get; set; }
public string? User2 { get; set; }
public string? User3 { get; set; }
public string? User4 { get; set; }
public string? User5 { get; set; }
}
