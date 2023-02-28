#!/bin/bash

DEBUG_FILE=/home/george/MovieBacklog_Startup.txt
POST_GRES_PID=$(netstat -tulpn | grep LISTEN | grep 5432 | awk  '{print $NF}' | awk -F '/' 'NR==1{print $1}')

printf "Postgres PID: $POST_GRES_PID\n" >> $DEBUG_FILE

if [ -z $POST_GRES_PID ];
then 
	kill $POST_GRES_PID
	printf "Kill Postgres return code: $?\n" >> $DEBUG_FILE
else
	printf "No PID found for Postgres process\n" >> $DEBUG_FILE
fi

cd /home/george/MovieBacklog
docker-compose up -d

printf "Docker started Movie Backlog App, return code: $?\n\n" >> $DEBUG_FILE
