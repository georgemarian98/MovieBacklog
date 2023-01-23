GREEN_TEXT='\033[0;32m'
RED_TEXT='\033[0;31m'
NO_COLOR='\033[0m'

printf "${GREEN_TEXT}------------------------------STOP Container------------------------------${NO_COLOR}\n"
docker stop moviebacklog

printf "\n${GREEN_TEXT}------------------------------REMOVE Container------------------------------${NO_COLOR}\n"
docker rm moviebacklog

printf "\n${GREEN_TEXT}------------------------------BUILD Container------------------------------${NO_COLOR}\n"
docker build ./MovieBacklog -t movie_backlog_image

if [ ! $? -eq 0 ]
then
    printf "${RED_TEXT}[ERROR] Exit program due to unexpected errors from docker build command...${NO_COLOR}\n"
    exit 1
fi

printf "\n${GREEN_TEXT}------------------------------RUN Container------------------------------${NO_COLOR}\n"
docker run -d -p 8082:80 --name moviebacklog movie_backlog_image

if [ ! $? -eq 0 ]
then
    printf "${RED_TEXT}[ERROR] Exit program due to unexpected errors from docker run command...${NO_COLOR}\n"
    exit 1
fi
