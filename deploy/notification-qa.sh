#!/bin/bash
docker load -i /home/docker-images/notification.tar
docker stop testokur-notification-qa && docker rm testokur-notification-qa --force
docker run -d \
	--env-file  /home/docker-images/notification-qa.env \
	--name testokur-notification-qa \
	--restart=always  \
	--network=testokur \
	--network-alias=notification-qa \
	-m=200M \
	testokur-notification:latest