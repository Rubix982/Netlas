import requests as req
from bs4 import BeautifulSoup
import os

URL = "https://moz.com/top500"

fileName = "domains.csv"

folder_path = str(os.path.abspath(__file__))[
    0:-len(os.path.basename(__file__))] + "data/"

res = req.get(URL)

soup = BeautifulSoup(res.text, features='lxml')

table_row_parse_tree = soup.findAll('tr')[1:-1]

if not os.path.exists(folder_path):
    os.makedirs(folder_path)

domains = []

for table_row in table_row_parse_tree:
    soup = BeautifulSoup(str(table_row), features='lxml')
    get_td = soup.findAll('td')
    domains.append(get_td[1].find('a')['href'].strip('http://'))

if (domains):
    with open(os.path.join(folder_path, fileName), mode='w') as file:
        for domain in domains:
            file.write(domain + ",\n")