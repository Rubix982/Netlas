import multiprocessing as mp
import os

# local function
from singleton_process_domain import process_domains


def get_csv_domain_data():
    folder_path = str(os.path.abspath(__file__))[
        0:-len(os.path.basename(__file__))] + "data/"

    file_name = "fixed_domains.csv"

    data = []

    if not os.path.exists(folder_path):
        os.makedirs(folder_path)

    with open(os.path.join(folder_path, file_name), mode='r') as file:
        for line in file:
            try:
                data.append(line.strip("\n"))
            except StopIteration:
                continue

    return data[0:-1]


if __name__ == '__main__':

    # Get CSV data
    data = get_csv_domain_data()

    # Data len is 498 - let's make it till 500!
    data.append("figma.com")
    data.append("dev-hearted.software")

    folder_path = str(os.path.abspath(__file__))[
        0:-len(os.path.basename(__file__))] + "dist/"

    if not os.path.exists(folder_path):
        os.makedirs(folder_path)

    if not os.path.exists(folder_path + "metadata/"):
        os.makedirs(folder_path + "metadata/")

    if not os.path.exists(folder_path + "content/"):
        os.makedirs(folder_path + "content/")

    # Setting process number
    active_process = 10

    # spitting the data
    split_data = []

    # iteration step size
    iteration_step_size = len(data) // active_process

    for i in range(0, len(data), iteration_step_size):
        split_data.append(data[i: i+iteration_step_size])

    for found_domain_data in split_data:
        with mp.Pool(active_process) as p:
            p.map(process_domains, found_domain_data)
