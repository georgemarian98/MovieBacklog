#!/bin/bash

DEBUG_FILE=/home/george/MovieBacklog_Startup.txt
POSTGRES_PID=$(netstat -tulpn | grep LISTEN | grep 5432 | awk  '{print $NF}' | awk -F '/' 'NR==1{print $1}')

printf "Postgres PID: $POSTGRES_PID\n" >> $DEBUG_FILE

if [ ! -z "$POSTGRES_PID" ];
then
	kill $POSTGRES_PID
	printf "Kill Postgres return code: $?\n" >> $DEBUG_FILE
else
	printf "No PID found for Postgres process\n" >> $DEBUG_FILE
fi

cd /home/george/MovieBacklog
docker-compose up -d

printf "Docker started Movie Backlog App, return code: $?\n\n" >> $DEBUG_FILE
