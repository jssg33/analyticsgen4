using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class UserLocation
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Label { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? CreatedAt { get; set; }

    public int? IsPrimary { get; set; }
}
