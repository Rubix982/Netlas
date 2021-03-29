import warnings
import requests as req
import json
import os

# For the 'verify=False' in the below request being made
with warnings.catch_warnings():
    warnings.simplefilter("ignore")

domain_names = ['facebook.com', 'linkedin.com',
                'youtube.com', 'instagram.com',
                'twitter.com', 'figma.com',
                'stackoverflow.com', 'github.com',
                'programiz.com', 'dev-hearted.software',
                'jsonplaceholder.typicode.com/posts', 'jsonplaceholder.typicode.com/posts/1',
                'jsonplaceholder.typicode.com/posts/1/comments', 'jsonplaceholder.typicode.com/comments?postId=1']

folder_path = str(os.path.abspath(__file__))[
    0:-len(os.path.basename(__file__))] + "logs/"

if not os.path.exists(folder_path):
    os.makedirs(folder_path)

if not os.path.exists(folder_path + "metadata/"):
    os.makedirs(folder_path + "metadata/")

if not os.path.exists(folder_path + "content/"):
    os.makedirs(folder_path + "content/")

for idx, name in enumerate(domain_names):
    try:
        res = req.get('https://localhost/request/',
                      verify=False,
                      params=[('domain', f'https://{name}'),
                              ('clientId', 1),
                              ('requestId', idx)])
    except ConnectionError:
        print(f"Could not send a request for {name}. Moving to the next")
        continue

    data = None

    if (res.text):
        data = json.loads(res.text)

        if '/' in data['domain']:
            data['domain'] = data['domain'].replace('https://', '')
            data['domain'] = data['domain'].replace('https://', '')

        with open(os.path.join(folder_path + "metadata/", f"{data['domain']}.json"), mode='w') as file:
            select_data = data.pop('content', None)
            json.dump(
                data, file)

        with open(os.path.join(folder_path + "content/", f"{data['domain']}.json"), mode='w') as file:
            json.dump(select_data, file)
