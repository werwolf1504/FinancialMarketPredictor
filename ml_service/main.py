from fastapi import FastAPI

app = FastAPI(title="Financial Market Predictor API")

@app.get("/health")
def health_check():
    return { "status": "ok", "message" : "ML service is running" }