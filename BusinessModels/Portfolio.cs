using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Portfolio
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public decimal PortfolioTargetInvestment { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<TradeOrder> TradeOrders { get; set; } = new List<TradeOrder>();
}
