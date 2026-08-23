using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinancialPlatform.Infrastructure.DTO;

public class FinnhubQouteDTO
{
    [JsonPropertyName("c")]
    public decimal CurrentPrice { get; set; } // Current price
    
    [JsonPropertyName("d")]
    public decimal Change { get; set; }

    [JsonPropertyName("dp")]
    public decimal PercentChange { get; set; } // Percent change

    [JsonPropertyName("h")]
    public decimal HighPrice { get; set; } // High price of the day
    
    [JsonPropertyName("l")]
    public decimal LowPrice { get; set; } // Low price of the day
    
    [JsonPropertyName("o")]
    public decimal OpenPrice { get; set; } // Open price of the day
    
    [JsonPropertyName("pc")]
    public decimal PreviousClosePrice { get; set; } // Previous close price
    
    [JsonPropertyName("t")]
    public long Timestamp { get; set; } // Timestamp
}
