
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

printf "${GREEN}------------------------------STOP Container------------------------------${NC}\n"
docker stop moviebacklog

echo $?

printf "${GREEN}------------------------------REMOVE Container------------------------------${NC}\n"
docker rm moviebacklog

echo $?

printf "${GREEN}------------------------------BUILD Container------------------------------${NC}\n"
docker build ./MovieBacklog -t movie_backlog_image

if [ ! $? -eq 0 ]
then
    printf "${RED}[ERROR] Exit program due to unexpected errors from docker build command...${NC}\n"
    exit 1
fi

printf "${GREEN}------------------------------RUN Container------------------------------${NC}\n"
docker run -d -p 8082:80 --name moviebacklog movie_backlog_image

if [ ! $? -eq 0 ]
then
    printf "${RED}[ERROR] Exit program due to unexpected errors from docker run command...${NC}\n"
    exit 1
fi
