using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Learningmodule
{
    public int Id { get; set; }

    public string? Cataloguesku { get; set; }

    public string? Description { get; set; }

    public string? Videourl { get; set; }

    public string? Author { get; set; }

    public byte? Authorcomp { get; set; }

    public string? Authorcontact { get; set; }

    public byte? Emprequired { get; set; }

    public int? Uisection { get; set; }
}
