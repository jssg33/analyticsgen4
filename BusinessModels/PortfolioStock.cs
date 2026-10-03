using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class PortfolioStock
{
    public int Id { get; set; }

    public string? Ticker { get; set; }

    public int PortfolioId { get; set; }

    public string? CompanyName { get; set; }

    public decimal Shares { get; set; }

    public decimal PreviousDayPrice { get; set; }

    public string PreviousDayDate { get; set; } = null!;

    public decimal StockTargetInvestment { get; set; }

    public virtual ICollection<TradeOrder> TradeOrders { get; set; } = new List<TradeOrder>();
}
