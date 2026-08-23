using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinancialPlatform.Infrastructure.DTO;

public class AlphaVantageQouteDTO
{
    [JsonPropertyName("Global Quote")]
    public AlphaVantageGlobalQouteDTO GlobalQuote { get; set; }
}
