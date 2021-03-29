import matplotlib.pyplot as plt
import pandas as pd
from pathlib import Path
import json
import os
import glob

from io import StringIO

dataframe = pd.DataFrame(data={'time_taken': [], 'cache': bool})

metadata_filename_list = []

for path in Path(os.path.abspath('.') + '/dist/metadata/').rglob('*.json'):
    metadata_filename_list.append(str(path.parent) + "/" + path.name)

for filename in metadata_filename_list:
    data = json.load(open(filename))
    dataframe = dataframe.append(
        {'time_taken': data['requestTimeMeasured'], 'cache': data['isFromCache']}, ignore_index=True)

dataframe.to_csv(str(os.path.abspath(__file__))[
    0:-len(os.path.basename(__file__))] + "data/cache_results.csv")
