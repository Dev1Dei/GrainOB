import random
import datetime
import json
import time
import requests

from collections import namedtuple

Grain = namedtuple('Grain', ['type', 'class_', 'dryness', 'cleanliness'])
FarmerTruck = namedtuple('FarmerTruck',
                         ['farmer_first_name', 'farmer_last_name', 'truck_numbers', 'truck_storage', 'grain',
                          'grain_weight', 'arrival', 'wanted_pay', 'price_per_tonne'])

# Load farmer names from JSON
with open('farmer_data.json', 'r', encoding='utf-8') as f:
    farmer_data = json.load(f)


def generate_grain():
    grain_type = random.choice(['Wheat', 'Corn', 'Rapeseed'])

    if grain_type == 'Wheat':
        classes = ['Class 1', 'Class 2', 'Class 3']
        grain_class = random.choice(classes)
        dryness = round(random.uniform(0, 100), 2)
        cleanliness = round(random.uniform(0, 100), 2)
    elif grain_type == 'Corn':
        grain_class = 'None'
        dryness = round(random.uniform(80, 100), 2)
        cleanliness = round(random.uniform(30, 60), 2)
    else:  # Rapeseed
        grain_class = 'None'
        dryness = round(random.uniform(20, 100), 2)
        cleanliness = round(random.uniform(50, 98), 2)

    return Grain(grain_type, grain_class, dryness, cleanliness)


def generate_farmer_with_truck():
    # Choose a gender randomly
    gender = random.choice(['male', 'female'])

    # Names
    FarmerFirstName = random.choice(farmer_data[gender]["first_names"])
    FarmerLastName = random.choice(farmer_data[gender]["last_names"])

    # Truck numbers
    letters = "ABCDEFGHIJKLMNOPRSTUVZ"
    TruckNumbers = ''.join(random.choice(letters) for _ in range(3)) + ':' + ''.join(
        random.choice('0123456789') for _ in range(3))

    # Truck storage
    TruckStorage_choices = list(range(10, 41)) + [50, 60, 70, 80, 90, 100]
    weights = [10] * 31 + [1] * 6
    TruckStorage = random.choices(TruckStorage_choices, weights)[0]

    # Grain details
    grain = generate_grain()

    # Grain weight brought by farmer (less than or equal to truck storage)
    grain_weight = round(random.uniform(0.1, TruckStorage), 2)

    # Arrival
    arrival = datetime.datetime.now().strftime('%Y-%m-%d %H:%M')

    # Wanted pay and price per tonne
    if grain.type == 'Wheat':
        if grain.class_ == "Class 1":
            price_per_tonne = round(random.uniform(200, 250), 2)
        elif grain.class_ == "Class 2":
            price_per_tonne = round(random.uniform(150, 200), 2)
        else:  # Class 3
            price_per_tonne = round(random.uniform(100, 150), 2)
    elif grain.type == 'Corn':
        price_per_tonne = round(random.uniform(180, 230), 2)
    else:  # Rapeseed
        price_per_tonne = round(random.uniform(400, 500), 2)

    wanted_pay = price_per_tonne * grain_weight

    return FarmerTruck(FarmerFirstName, FarmerLastName, TruckNumbers, TruckStorage, grain, grain_weight, arrival,
                       wanted_pay, price_per_tonne)


def convert_to_json_string(ft):
    # Convert the named tuple to a dictionary
    dict_representation = ft._asdict()
    dict_representation['FarmerFirstName'] = dict_representation.pop('farmer_first_name')
    dict_representation['FarmerLastName'] = dict_representation.pop('farmer_last_name')
    dict_representation['TruckNumbers'] = dict_representation.pop('truck_numbers')
    dict_representation['TruckStorage'] = dict_representation.pop('truck_storage')
    # Convert nested named tuples to dictionaries
    grain_dict = dict_representation['grain'] = dict_representation['grain']._asdict()
    # Change 'class_' to 'Class' to match DTO in backend
    grain_dict['Class'] = grain_dict.pop('class_')

    dict_representation['Grain'] = grain_dict
    dict_representation['GrainWeight'] = dict_representation.pop('grain_weight')
    dict_representation['Arrival'] = dict_representation.pop('arrival')
    dict_representation['WantedPay'] = dict_representation.pop('wanted_pay')
    dict_representation['PricePerTonne'] = dict_representation.pop('price_per_tonne')
    # Convert dictionary to JSON string
    return json.dumps(dict_representation, indent=4, ensure_ascii=False)


def post_data_to_api(json_data):
    url = "http://grainoperationapi:5003/api/transactions"
    headers = {"Content-Type": "application/json"}

    response = requests.post(url, headers=headers, data=json_data.encode('utf-8'))

    if response.status_code == 201:
        print("Data posted successfully")
    else:
        print(f"Failed to post data. Status code: {response.status_code}, Response: {response.text}")


# Continuously generate data every 1 to 3 minutes
while True:
    farmer_truck_data = generate_farmer_with_truck()
    json_string = convert_to_json_string(farmer_truck_data)
    print(json_string, flush=True)

    post_data_to_api(json_string)
    time.sleep(random.randint(60, 180))
