import requests as req
import json
import os
import multiprocessing as mp

folder_path = str(os.path.abspath(__file__))[
    0:-len(os.path.basename(__file__))] + "dist"

req.packages.urllib3.disable_warnings() 

def process_domains(domain_name: list):

    process_id = str(mp.current_process().name).split('-')[1]
    domain_name = domain_name.strip(',  ')

    try:
        res = req.get('https://localhost/request/',
                      verify=False,  # DO NOT verify
                      params=[('domain', f'https://{domain_name}'),
                              ('clientId', 1),
                              ('requestId', process_id)])
    except req.exceptions.Timeout:
        # TODO: Maybe set up for a retry, or continue in a retry loop
        print(f"Timeout error! Occurred with {domain_name}")
    except req.exceptions.TooManyRedirects:
        # TODO: Tell the user their URL was bad and try a different one
        print(f"TooManyRedirects error! Occurred with {domain_name}")
    except ConnectionError:
        # TODO: Error returned by the request
        print(f"ConnectionError error! Occurred with {domain_name}")
    except req.exceptions.RequestException as e:
        # catastrophic error. bail.
        print(f"RequestException error! Occurred with {domain_name}")
        raise SystemExit(e)

    data = None

    if (res.text):

        metadata_folder = f"{folder_path}/metadata/{process_id}"

        content_folder = f"{folder_path}/content/{process_id}"

        error_folder = f"{folder_path}/error/{process_id}"

        try:
            data = json.loads(res.text)
        except json.decoder.JSONDecodeError as e:

            if not os.path.exists(f"{folder_path}/error/{process_id}"):
                os.makedirs(f"{folder_path}/error/{process_id}")

            print(
                f"Error occured when loading json with {domain_name} due to error: {e}")
            with open(os.path.join(error_folder, f"{domain_name}.json"), mode='w') as file:
                json.dump(
                    res.text, file)
            exit()

        if not os.path.exists(f"{folder_path}/metadata/{process_id}"):
            os.makedirs(f"{folder_path}/metadata/{process_id}")

        if not os.path.exists(f"{folder_path}/content/{process_id}"):
            os.makedirs(f"{folder_path}/content/{process_id}")

        if '/' in data['domain']:
            data['domain'] = data['domain'].replace('https://', '')
            data['domain'] = data['domain'].replace('https://', '')

        select_data = data.pop('content', None)
        with open(os.path.join(f"{metadata_folder}", f"{data['domain']}.json"), mode='w') as file:
            json.dump(
                data, file)

        with open(os.path.join(f"{content_folder}", f"{data['domain']}.html"), mode='w') as file:
            # json.dump(select_data, file)
            file.write(select_data)
