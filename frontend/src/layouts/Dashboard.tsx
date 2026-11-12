import { TradingChart } from '../components/TradingCharts';

export function Dashboard() {
    // Тестові дані (5 точок для відображення лінії тренду)
    const testData = [
        { time: '2026-11-10T10:00:00Z', value: 150.5 },
        { time: '2026-11-10T11:00:00Z', value: 152.0 },
        { time: '2026-11-10T12:00:00Z', value: 149.8 },
        { time: '2026-11-10T13:00:00Z', value: 155.2 },
        { time: '2026-11-10T14:00:00Z', value: 158.4 },
    ];

    return (
        <div className="w-full">
            <h2 className="text-2xl font-semibold text-slate-800 mb-6">
                Огляд ринку (Market Overview)
            </h2>
            
            {/* Викликаємо наш компонент графіка і передаємо дані через props */}
            <TradingChart data={testData} />
        </div>
    );
}