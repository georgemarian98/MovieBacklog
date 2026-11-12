# MovieBacklog
This project is a site for keeping track of the movies and TV shows that I want to watch.

Technologies used:
* .Net
* RestAPI for quering IMDB databse
* SQLite

`CONFIG_PATH` is the desired location for the sqllite database and keys files.

In order to build the project run:
`docker compose build moviebacklog`

When you run the app for the first time, the database need to be initialized, Use the following commands in movie-backlog/Moviebacklog folder:
- `dotnet ef migrations add <MESSAGE>`
- `dotnet ef database update`