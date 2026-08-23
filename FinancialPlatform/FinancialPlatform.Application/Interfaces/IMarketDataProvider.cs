using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Domain.Entities;

namespace FinancialPlatform.Application.Interfaces
{
    public interface IMarketDataProvider
    {
        Task<MarketTick> GetCurrentPriceAsync(string ticker);
    }
}
