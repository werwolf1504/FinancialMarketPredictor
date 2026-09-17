from datetime import datetime
import json
import aio_pika
from fastapi import FastAPI
from pydantic import AliasChoices, BaseModel, Field
import asyncio
from contextlib import asynccontextmanager

RABBITMQ_URL = "amqp://guest:guest@localhost:5672/"
QUEUE_NAME = "python_market_data_queue"

class MarketDataCollectedEvent(BaseModel):
    ticker: str = Field(validation_alias=AliasChoices('ticker', 'Ticker'))
    price: float = Field(validation_alias=AliasChoices('price', 'Price'))
    timestamp: datetime = Field(validation_alias=AliasChoices('timestamp', 'Timestamp', 'timeStamp', 'TimeStamp'))

async def start_consumer():
    print("Try to connect the consumer")

    connection = await aio_pika.connect_robust(RABBITMQ_URL)
    chanel = await connection.channel()

    queue = await chanel.declare_queue(QUEUE_NAME, durable=True)

    await queue.consume(process_message)

    return connection

async def process_message(message: aio_pika.abc.AbstractIncomingMessage):
    async with message.process():
        body = message.body.decode("utf-8")
        data = json.loads(body)

        event = data.get("message", data)

        event_model = MarketDataCollectedEvent(**event)

        print(f"Got that data {event_model.ticker}: price {event_model.price} timespan {event_model.timestamp}")


@asynccontextmanager
async def lifespan(app: FastAPI):
    connection = await start_consumer()

    yield

    await connection.close()


app = FastAPI(title="Financial Market Predictor API", lifespan=lifespan)

@app.get("/health")
def health_check():
    return { "status": "ok", "message" : "ML service is running" }