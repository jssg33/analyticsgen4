using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public class TradeOrders
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;

    public int PortfolioStockId { get; set; }
    public PortfolioStock PortfolioStock { get; set; } = null!;

    public int TraderId { get; set; }
    public Trader Trader { get; set; } = null!;

    public decimal Quantity { get; set; }
    public decimal ExecutionPrice { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public string OrderType { get; set; } = "BUY"; // BUY or SELL
}
