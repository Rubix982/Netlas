import requests as req

domain_names = ['facebook.com', 'linkedin.com',
                'youtube.com', 'instagram.com', 'twitter.com']

for name in domain_names:
    res = req.get('http://localhost/filterJSON/',
                  params={'domainname': name})
    print(res.text)
