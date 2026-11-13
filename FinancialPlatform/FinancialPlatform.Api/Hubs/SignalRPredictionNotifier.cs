using Microsoft.AspNetCore.SignalR;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Contracts;

namespace FinancialPlatform.Api.Hubs;

public class SignalRPredictionNotifier : IPredictionNotifier
{
    private readonly IHubContext<PredictionHub> _hubContext;

    public SignalRPredictionNotifier(IHubContext<PredictionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastPredictionAsync(PredictionCompletedEvent prediction)
    {
        await _hubContext.Clients.All.SendAsync("ReceivePrediction", prediction);
    }
}