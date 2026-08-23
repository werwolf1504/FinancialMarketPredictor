using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using FinancialPlatform.Domain.Entities;

namespace FinancialPlatform.Infrastructure.Data
{
    public class FinancialDbContext : DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options) { }

        public DbSet<StockAsset> StockAssets { get; set; }
        public DbSet<MarketTick> MarketTicks { get; set; }
        public DbSet<PredictionResult> PredictionResults { get; set; }

    }
}
