from datetime import datetime
import json
import aio_pika
from fastapi import FastAPI
from pydantic import AliasChoices, BaseModel, Field
import asyncio
from contextlib import asynccontextmanager
from model import LSTMPricePredictor
from data_processor import MarketDataProcessor
from collections import defaultdict
import torch

#RabbitMQ global variables
RABBITMQ_URL = "amqp://guest:guest@localhost:5672/"
QUEUE_NAME = "python_market_data_queue"
prediction_exchange = None


#Global variables ML
model = None
preprocessor = MarketDataProcessor()

#Dictionary
history_buffer = defaultdict(list)
SEQUENCE_LENGTH = 10

class MarketDataCollectedEvent(BaseModel):
    ticker: str = Field(validation_alias=AliasChoices('ticker', 'Ticker'))
    price: float = Field(validation_alias=AliasChoices('price', 'Price'))
    timestamp: datetime = Field(validation_alias=AliasChoices('timestamp', 'Timestamp', 'timeStamp', 'TimeStamp'))

async def start_consumer():
    global prediction_exchange
    print("Try to connect the consumer")

    connection = await aio_pika.connect_robust(RABBITMQ_URL)
    chanel = await connection.channel()

    queue = await chanel.declare_queue(QUEUE_NAME, durable=True)
    await queue.consume(process_message)

    prediction_exchange = await chanel.declare_exchange("prediction_result_exchange", aio_pika.ExchangeType.FANOUT)

    return connection

async def process_message(message: aio_pika.abc.AbstractIncomingMessage):
    async with message.process():
        body = message.body.decode("utf-8")
        data = json.loads(body)

        event = data.get("message", data)

        event_model = MarketDataCollectedEvent(**event)

        ticker = event_model.ticker
        price = event_model.price

        history_buffer[ticker].append(price)

        if len(history_buffer[ticker]) > SEQUENCE_LENGTH + 40 :
            history_buffer[ticker].pop(0)

        print(f"Got that data {event_model.ticker}: price {event_model.price} timespan {event_model.timestamp}")

        if len(history_buffer[ticker]) >= SEQUENCE_LENGTH + 40 :
            processed_df = preprocessor.process(history_buffer[ticker])

            if len(processed_df) < SEQUENCE_LENGTH:
                print(f"Clearing the data... Waiting more for {ticker}")
                return

            features = ['log_return', 'rsi', 'macd']

            recent_window = processed_df[features].tail(SEQUENCE_LENGTH).values

            input_tensor = torch.FloatTensor(recent_window.copy()).unsqueeze(0)

            with torch.no_grad():
                prediction = model(input_tensor)

            prediction_value = prediction.item()

            print(f"Prediction for {ticker}: {prediction_value}.4f")

            prediction_data = {
                "ticker" : ticker,
                "price" : prediction_value,
                "timestamp" : datetime.utcnow().isoformat()
            }

            msg_body = json.dumps(prediction_data).encode("utf-8")
            msg = aio_pika.Message(body=msg_body, content_type="application/json")

            if prediction_exchange:
                await prediction_exchange.publish(msg, routing_key="")
                print("Prediction sent")


@asynccontextmanager
async def lifespan(app: FastAPI):
    global model
    print("Loading model...")

    model = LSTMPricePredictor(input_size=3, hidden_size=64, num_layers=2)

    model.load_state_dict(torch.load('model_weights.pth', weights_only=True))

    model.eval()

    connection = await start_consumer()

    yield

    await connection.close()


app = FastAPI(title="Financial Market Predictor API", lifespan=lifespan)

@app.get("/health")
def health_check():
    return { "status": "ok", "message" : "ML service is running" }