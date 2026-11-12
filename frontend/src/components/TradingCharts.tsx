import { createChart, ColorType, type IChartApi, LineSeries, type Time } from "lightweight-charts";
import { useEffect, useRef } from "react";

interface ChartProps {
    // Дозволяємо передавати і рядок (ISO), і готовий числовий таймстамп
    data: { time: string | number; value: number }[];
}

export function TradingChart({ data }: ChartProps) {
    const chartContainerRef = useRef<HTMLDivElement>(null);
    const chartRef = useRef<IChartApi | null>(null);

    useEffect(() => {
        if (!chartContainerRef.current)
            return;

        const chart = createChart(chartContainerRef.current, {
            layout: {
                background: { type: ColorType.Solid, color: "#f8fafc" },
                textColor: "#334155",
            },
            width: chartContainerRef.current.clientWidth,
            height: 400,
            timeScale: {
                timeVisible: true,
                secondsVisible: false,
            },
        });

        const lineSeries = chart.addSeries(LineSeries, {
            color: "#2563eb",
            lineWidth: 2,
        });

        // Автоматично конвертуємо рядки (ISO 8601) у Unix-таймстампи в секундах, якщо це необхідно
        const formattedData = data.map(item => ({
            time: (typeof item.time === 'string'
                ? Math.floor(new Date(item.time).getTime() / 1000)
                : item.time) as Time,
            value: item.value,
        }));

        lineSeries.setData(formattedData as any);
        chartRef.current = chart;

        return () => {
            chart.remove();
        };
    }, [data]);

    return (
        <div className="w-full max-w-4xl mx-auto shadow-lg rounded-xl overflow-hidden border border-slate-200">
            <div ref={chartContainerRef} />
        </div>
    );
}