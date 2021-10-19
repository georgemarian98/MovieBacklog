using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieBacklog.Models;
using MovieBacklog.Data;

namespace MovieBacklog.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext applicationDb;

        public MoviesService(ApplicationDbContext applicationDb)
        {
            this.applicationDb = applicationDb;
        }

        public void AddMovieToBacklog(MediaRecord movie)
        {
            applicationDb.Add(movie);
            applicationDb.SaveChanges();
        }

        public List<MediaRecord> GetBacklog()
        {
            return applicationDb.Media.ToList();
        }

        public void RemoveMovie(int id)
        {
            var movie = applicationDb.Media.Find(id);
            applicationDb.Media.Remove(movie);
            applicationDb.SaveChanges();
        }
    }
}
