FROM python:3.9-slim

WORKDIR /app

COPY main.py /app
COPY requirements.txt /app
COPY farmer_data.json /app

RUN pip install --no-cache-dir -r requirements.txt

CMD ["python", "main.py"]
