using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class TradeOrder
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public int PortfolioStockId { get; set; }

    public int TraderId { get; set; }

    public decimal Quantity { get; set; }

    public decimal ExecutionPrice { get; set; }

    public DateTime ExecutedAt { get; set; }

    public string OrderType { get; set; } = null!;

    public virtual Portfolio Portfolio { get; set; } = null!;

    public virtual PortfolioStock PortfolioStock { get; set; } = null!;

    public virtual Trader Trader { get; set; } = null!;
}
