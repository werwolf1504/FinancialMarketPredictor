using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialPlatform.Domain.Entities;

class StockAsset
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public string CompanyName { get; set; }
    public string Exchange { get; set; }
}
