import numpy as np
import pandas as pd
from sklearn.preprocessing import MinMaxScaler
from ta.momentum import RSIIndicator
from ta.trend import MACD

class MarketDataProcessor:
    def __init__(self):
        self.scaler = MinMaxScaler(feature_range=(0,1))

    def process(self, prices: list[float]) -> pd.DataFrame :
        df = pd.DataFrame(prices, columns=['close'])

        df['log_return'] = np.log(df['close'] / df['close'].shift(1))

        rsi = RSIIndicator(close=df['close'], window=14)
        df['rsi'] = rsi.rsi()

        macd = MACD(close=df['close'])
        df['macd'] = macd.macd()

        df = df.dropna()

        if df.empty:
            return df

        scaled_data = self.scaler.fit_transform(df)

        normalized_data = pd.DataFrame(scaled_data, columns=df.columns)

        return normalized_data