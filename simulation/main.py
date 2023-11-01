import random
import datetime
import json
import time
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
        grain_class = None
        dryness = round(random.uniform(80, 100), 2)
        cleanliness = round(random.uniform(30, 60), 2)
    else:  # Rapeseed
        grain_class = None
        dryness = round(random.uniform(20, 100), 2)
        cleanliness = round(random.uniform(50, 98), 2)

    return Grain(grain_type, grain_class, dryness, cleanliness)

def generate_farmer_with_truck():
    # Choose a gender randomly
    gender = random.choice(['male', 'female'])

    # Names
    farmer_first_name = random.choice(farmer_data[gender]["first_names"])
    farmer_last_name = random.choice(farmer_data[gender]["last_names"])

    # Truck numbers
    letters = "ABCDEFGHIJKLMNOPRSTUVZ"
    truck_numbers = ''.join(random.choice(letters) for _ in range(3)) + ':' + ''.join(
        random.choice('0123456789') for _ in range(3))

    # Truck storage
    truck_storage_choices = list(range(10, 41)) + [50, 60, 70, 80, 90, 100]
    weights = [10] * 31 + [1] * 6
    truck_storage = random.choices(truck_storage_choices, weights)[0]

    # Grain details
    grain = generate_grain()

    # Grain weight brought by farmer (less than or equal to truck storage)
    grain_weight = round(random.uniform(0.1, truck_storage), 2)

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

    return FarmerTruck(farmer_first_name, farmer_last_name, truck_numbers, truck_storage, grain, grain_weight, arrival,
                       wanted_pay, price_per_tonne)

def convert_to_json_string(ft):
    # Convert the named tuple to a dictionary
    dict_representation = ft._asdict()
    # Convert nested named tuples to dictionaries
    dict_representation['grain'] = dict_representation['grain']._asdict()
    # Convert dictionary to JSON string
    return json.dumps(dict_representation, indent=4, ensure_ascii=False)

# Continuously generate data every 1 to 3 minutes
while True:
    print(convert_to_json_string(generate_farmer_with_truck()), flush=True)
    time.sleep(random.randint(60, 180))
