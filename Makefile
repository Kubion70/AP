SHELL := /bin/bash

build:
	docker build ./ -t ap
ap:
	docker run --rm -d -p 443:80 -v $(CURDIR)/:/usr/src --name ap ap
stop:
	docker kill ap