using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialPlatform.Domain.Entities;

public class MarketTick
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public DateTime Timestamp { get; set; }
}
