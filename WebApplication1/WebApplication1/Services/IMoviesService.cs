using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backlog.Models;

namespace MovieBacklog.Services
{
    public interface IMoviesService
    {
        public List<MovieRecord> GetBacklog();

        public void AddMovieToBacklog(MovieRecord movie);

        public void RemoveMovie(int id);
    }
}
