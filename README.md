# MovieBacklog
This project is a site for keeping track of the movies and TV shows that I want to watch.

Technologies used:
* .Net
* RestAPI for quering IMDB databse
* PostgresDB

Must create the .env file with `POSTGRES_HOST`, `POSTGRES_PORT`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` and `RAPIDAPI_KEY` variables

`CONFIG_PATH` is the desired location for the postgres database files.

In order to build the project run:
`docker-compose build moviebacklog`

If there is problem when inserting a new media into the data base this command in the postgres database console:
`SELECT setval(pg_get_serial_sequence('"Media"', 'Id'), COALESCE(MAX("Id"), 1)) FROM "Media";`
This command will set the next assigend index to be the next in the sequence thus avoiding collisions.

When you run the app for the first time, the database need to be initialized, Use the following commands in movie-backlog/Moviebacklog folder:
- `dotnet ef migrations add <MESSAGE>`
- `dotnet ef database update`