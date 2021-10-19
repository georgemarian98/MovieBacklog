using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backlog.Models;

namespace MovieBacklog.Services
{
    public interface IMoviesService
    {
        public List<MediaRecord> GetBacklog();

        public void AddMovieToBacklog(MediaRecord movie);

        public void RemoveMovie(int id);
    }
}
