using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class CipherSupport
{
    public int Id { get; set; }

    public string? CipherName { get; set; }

    public string? Version { get; set; }

    public string? Description { get; set; }

    public string? CipherKey { get; set; }

    public int? IsActive { get; set; }

    public string? DateCreated { get; set; }
}
