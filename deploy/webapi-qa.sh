#!/bin/bash
docker load -i /home/images/webapi.tar
docker stop testokur-webapi-qa && docker rm testokur-webapi-qa --force
docker run -d \
	--env-file  /home/env/webapi-qa.env \
	-v /home/data-qa:/app/Data \
	-v /home/uploads-qa:/app/wwwroot/uploads \
	--name testokur-webapi-qa \
	--restart=always  \
	--network=testokur \
	--network-alias=webapi-qa \
	-m=200M \
	testokur-webapi:latest