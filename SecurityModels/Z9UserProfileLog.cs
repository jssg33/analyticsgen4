using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class UserProfileLog
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int Uid { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public string? NotificationType { get; set; }

    public DateTime? NotificationTimestamp { get; set; }

    public string? NotificationDestination { get; set; }
}
