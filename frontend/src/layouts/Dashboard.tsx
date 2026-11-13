import { TradingChart } from '../components/TradingCharts';
import { useEffect, useState } from 'react';
import * as signalr from '@microsoft/signalr';

interface PredictionEvent{
    ticker: string;
    predictedPrice: number;
    timestamp: string;
}

export function Dashboard() {
    const [selectedTicket, setSelectedTicker] = useState<string>('AAPL');

    // Було: useState([{ time: '2026-11-10...', value: 150.5 }, ...])
    // Стало — чистий масив для реальних даних:
    const [chartData, setChartData] = useState<{ time: string | number; value: number }[]>([]);

    useEffect(() => {
        const connection = new signalr.HubConnectionBuilder()
            .withUrl('/hubs/predictions')
            .withAutomaticReconnect()
            .build();

        connection.on('ReceivePrediction', (prediction: any) => {
            console.log('Got prediction from SignalR:', prediction);

            const priceValue = prediction.price ?? prediction.predictedPrice;

            if (prediction.ticker === selectedTicket && priceValue !== undefined) {
                const timeInSeconds = Math.floor(new Date(prediction.timestamp).getTime() / 1000);

                setChartData(prev => [
                    ...prev,
                    { time: timeInSeconds, value: Number(priceValue) }
                ]);
            }
        });

        connection.start()
            .then(() => console.log('SignalR connected'))
            .catch(err => console.error('SignalR connection Error: ', err));

        return() => {
            connection.stop();
        };
    }, [selectedTicket]);

    return (
        <div className="w-full">
            <div className='flex items-center justify-between mb-6 max-w-4xl mx-auto'>
                <h2 className="text-2xl font-semibold text-slate-800">
                    Live prediction: <span className="text-blue-600">{selectedTicket}</span>                
                </h2>

                <div className="flex gap-2">
                    {['APPL', 'TSLA', 'MSFT'].map(ticker => (
                        <button key={ticker}
                            onClick={() => setSelectedTicker(ticker)}
                            className={`px-4 py-2 rounded-lg font-medium transition-colors ${
                                selectedTicket === ticker
                                    ? 'bg-blue-600 text-white'
                                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                            }`}
                        >
                            {ticker}
                        </button>
                    ))}
                </div>
            </div>
            
            
            {/* Викликаємо наш компонент графіка і передаємо дані через props */}
            <TradingChart data={chartData} />
        </div>
    );
}