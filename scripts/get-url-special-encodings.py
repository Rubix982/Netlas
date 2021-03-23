import requests as req
from bs4 import BeautifulSoup
import os

res = req.get("https://www.w3schools.com/tags/ref_urlencode.asp")

soup = BeautifulSoup(res.text, features='lxml')

table_row_parse_tree = soup.findAll('table')[0].findAll('tr')[1:-1]

encodings = []
numerical_skip = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']
uppercase_alphabatical_skip = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                               'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z']
lowercase_alphabatical_skip = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
                               'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z']
misc_skip = ['/', '\\']

total_skip_characters = numerical_skip + uppercase_alphabatical_skip + \
    lowercase_alphabatical_skip + misc_skip

for idx, row in enumerate(table_row_parse_tree):

    character = table_row_parse_tree[idx].findAll('td')[0].contents[0]

    if character in total_skip_characters:
        continue

    if character == 'space':
        character = ' '

    utf_8 = table_row_parse_tree[idx].findAll('td')[2].contents[0]

    encodings.append([(character, utf_8)])

folder_path = str(os.path.abspath(__file__))[
    0:-len(os.path.basename(__file__))] + "data/"

if not os.path.exists(folder_path):
    os.makedirs(folder_path)

with open(os.path.join(folder_path, f"encodings.csv"), encoding='utf-8', mode='w') as file:
    for entry in encodings:
        file.write(f"'{entry[0][0]}','{entry[0][1]}',\n")
