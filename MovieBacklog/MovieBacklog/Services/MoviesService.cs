using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backlog.Models;

namespace MovieBacklog.Services
{
    public class MoviesService : IMoviesService
    {
        public List<MovieRecord> Movies { get; set; }

        public MoviesService()
        {
            var movie = new MovieRecord("American history X", 1998, "https://www.imdb.com/title/tt0120586/", "https://m.media-amazon.com/images/M/MV5BZTJhN2FkYWEtMGI0My00YWM4LWI2MjAtM2UwNjY4MTI2ZTQyXkEyXkFqcGdeQXVyNjc3MjQzNTI@._V1_.jpg");
            Movies = new List<MovieRecord>();
            Movies.Add(movie);
        }

        public void AddMovieToBacklog(MovieRecord movie)
        {
            Movies.Add(movie);
        }

        public List<MovieRecord> GetBacklog()
        {
            return Movies;
        }

        public void RemoveMovie(int id)
        {
            var movie = Movies.Find(movie => movie.Id == id);
            Movies.Remove(movie);
        }
    }
}
