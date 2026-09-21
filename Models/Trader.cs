using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Trader
{
    public int Id { get; set; }

    public string StaffUserId { get; set; } = null!;

    public string? FullName { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TradeOrder> TradeOrders { get; set; } = new List<TradeOrder>();
}
