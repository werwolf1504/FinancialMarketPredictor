using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialPlatform.Contracts;

public record PredictionCompletedEvent(string Ticker, decimal PredictedPrice, DateTime TimeStamp);
