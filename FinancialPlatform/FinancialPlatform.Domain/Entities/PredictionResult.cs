using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialPlatform.Domain.Entities;

public class PredictionResult
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public decimal PredictedPrice { get; set; }
    public DateTime TargetDate { get; set; }
    public string ModelVersion { get; set; }
}
