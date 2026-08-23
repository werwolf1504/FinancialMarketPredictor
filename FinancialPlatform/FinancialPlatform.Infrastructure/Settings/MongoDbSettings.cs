using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Driver.Core.Configuration;

namespace FinancialPlatform.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
