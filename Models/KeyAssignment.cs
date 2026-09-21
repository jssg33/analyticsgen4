using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Keyassignment
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public string KeyId { get; set; } = null!;

    public string? Alias { get; set; }

    public string PublicKey { get; set; } = null!;

    public string? PrivateKey { get; set; }

    public int DeliveredToUser { get; set; }

    public string CreatedAt { get; set; } = null!;

    public string UpdatedAt { get; set; } = null!;

    public virtual Usersession Session { get; set; } = null!;
}
