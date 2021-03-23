import requests as req
import json
import os

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

for idx, name in enumerate(domain_names):
    res = req.get('http://localhost/request/',
                  params=[('domain', f'http://{name}'),
                          ('clientId', 1),
                          ('requestId', idx)])

    data = json.loads(res.text)

    if '/' in data['domain']:
        data['domain'] = data['domain'].replace('/', '.')

    with open(os.path.join(folder_path, f"{data['domain']}.json"), mode='w') as file:
        json.dump(data, file)

