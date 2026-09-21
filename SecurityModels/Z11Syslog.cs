using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Syslog
{
public int Id { get; set; }
public DateTime LogDate { get; set; }
public string? LogLevel { get; set; }
public string? Source { get; set; }
public string? Message { get; set; }
public string? Exception { get; set; }
public string? StackTrace { get; set; }
public string? Username { get; set; }
public string? MachineName { get; set; }
public string? IpAddress { get; set; }
public string? AdditionalData { get; set; }
}
