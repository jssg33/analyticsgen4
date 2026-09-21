using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class CockyCipherBlock
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public string? Key1 { get; set; }
    public string? Key2 { get; set; }
    public string? Key3 { get; set; }
    public string? Key4 { get; set; }
    public string? Key5 { get; set; }
    public CipherType type { get; set; }
    public string? SessionToken { get; set; }
}

public enum CipherType
{
DateBase = 1, //Cipher Base Algorithm Off Date Range - Assumes SSL in place as well.
Default = 2,  //Global Primes For General Use Cases Modulo Date
Session = 3  //Session Based Per PWT Token
}
