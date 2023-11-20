import random
import time
import json
import requests
import traceback

# Default base prices for reference
base_prices = {
    "Wheat Class 1": 225,
    "Wheat Class 2": 175,
    "Wheat Class 3": 125,
    "Corn": 205,
    "Rapeseed": 450
}


def fetch_latest_prices():
    try:
        api_url = "http://grainoperationapi:5003/api/prices/latest"
        response = requests.get(api_url)
        if response.status_code == 200:
            prices = response.json()
            print("Fetched latest prices:", prices)
            return prices
        else:
            print("Failed to fetch latest prices, using base prices.")
            return base_prices
    except Exception as exc:
        print("Error fetching prices:", exc)
        return base_prices


def simulate_price_change(price):
    fluctuation = random.uniform(-0.1, 0.1)
    updated_price = price * (1 + fluctuation)
    if updated_price > price * 1.5 and random.random() < 0.05:
        updated_price = price * 0.5
    elif updated_price < price * 0.5 and random.random() < 0.05:
        updated_price = price * 1.5
    return round(updated_price, 2)


def post_data_to_api(price_updates):
    try:
        api_url = "http://grainoperationapi:5003/api/prices"
        headers = {"Content-Type": "application/json"}
        response = requests.post(api_url, headers=headers, json=price_updates)
        if response.status_code in [200, 201]:
            print("Bulk grain prices updated successfully")
        else:
            print(
                f"Failed to update grain prices. Status code: {response.status_code}, Response: {response.text}")
    except Exception as exc:
        print("Error posting prices in bulk:", exc)


def main():
    while True:
        try:
            prices = fetch_latest_prices()
            price_updates = []
            for grain, price in prices.items():
                updated_price = simulate_price_change(price)
                grain_type, grain_class = grain.rsplit(' ', 1) if 'Class' in grain else (grain, None)
                price_updates.append({
                    "GrainType": grain_type,
                    "GrainClass": grain_class,
                    "Price": updated_price
                })

            if price_updates:
                print(json.dumps(price_updates, indent=4))
                post_data_to_api(price_updates)

            time.sleep(random.randint(300, 600))
        except Exception as exc:
            print("Error in main loop:", exc)
            traceback.print_exc()
            time.sleep(60)


if __name__ == "__main__":
    main()
