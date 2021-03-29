# Automating Scripts
python-test:
	python3 ./scripts/get_requests.py

python-encodings:
	python3 ./scripts/get_url_special_encodings.py

python-fetch-results:
	python3 ./scripts/multithread_domains.py

python-fetch-domain-names:
	pytohn3: ./scripts/get_authoritative_domain_names.py

rm-dist:
	rm -rf ./scripts/dist/

generate-multiprocess-graph:
	cd scripts/
	python3 ./generate_graphs_multiprocess.py
	cd ..

generate-request-graph:
	cd scripts/
	python3 ./generate_graphs_request_test.py
	cd ..

stop:
	docker container stop $(docker ps -q)

delete:
	docker container rm $(docker ps -q)

remove:
	sudo docker-compose down --remove-orphans

# Building Docker Images

## For Docker Compose, Production
build-prod:
	sudo docker-compose -f docker-compose.yaml up --build

## For Docker Compose, Development
build-dev:
	sudo chmod +x scripts/build-dev.sh scripts/wait-for-it.sh
	./scripts/build-dev.sh

## For building only server image
build-server:
	sudo chmod +x scripts/build-server.sh
	./scripts/build-server.sh

## For building only client image
build-client:
	sudo chmod +x scripts/build-client.sh
	./scripts/build-client.sh	

# Starting Docker Images

## For Docker Comppose, Production
up-prod:
	sudo docker-compose -f docker-compose.yaml up

## For Docker Comppose, Development
up-dev:
	sudo docker-compose -f docker-compose.dev.yaml up

## For starting only server image
up-server:
	sudo chmod +x scripts/up-server.sh
	./scripts/up-server.sh

## For starting only client iamge
up-client:
	sudo chmod +x scripts/up-client.sh
	./scripts/up-client.sh	

# List all process
ps:
	sudo docker-compose ps -a

# List all images
images:
	sudo docker images

web-server:
	sudo docker exec -it web-server-synet /bin/bash

server:
	sudo docker exec -it server-synet /bin/bash

client:
	sudo docker exec -it client-synet /bin/bash

cache-lru:
	sudo docker exec -it cache-lru-synet /bin/bash

cache-mru:
	sudo docker exec -it cache-mru-synet /bin/bash