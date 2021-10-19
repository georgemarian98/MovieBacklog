using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backlog.Models;
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
            return applicationDb.Movies.ToList();
        }

        public void RemoveMovie(int id)
        {
            var movie = applicationDb.Movies.Find(id);
            applicationDb.Movies.Remove(movie);
            applicationDb.SaveChanges();
        }
    }
}
