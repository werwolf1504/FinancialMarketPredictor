using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Domain.Entities;

namespace FinancialPlatform.Application.Interfaces;

public interface IMarketDataRepository
{
    Task InsertASync(MarketTick marketTick);
}
