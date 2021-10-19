using MovieBacklog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBacklog.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieBacklog.Data;

namespace Backlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoviesService moviesService;

        public HomeController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public ActionResult Index()
        {
            return View(moviesService.GetBacklog());
        }

        [HttpPost]
        public ActionResult Search([Bind("movieTitle")] string movieTitle)
        {
            var backlogMovies = moviesService.GetBacklog();
            var movies = ImdbApiCall(movieTitle).Where(movie => backlogMovies.All(el => el.Title != movie.Title) == true);
            return View(movies);
        }

        [HttpDelete]
        public void DeleteMovie(int id)
        {
            moviesService.RemoveMovie(id);
        }

        [HttpPost]
        public void AddMovie(string title, int year, string imdbUrl, string thumbnailUrl)
        {
            MediaRecord newMovie = new MediaRecord() { Title = title, Year = year, ImdbUrl = imdbUrl, ThumbnailUrl = thumbnailUrl };
            moviesService.AddMovieToBacklog(newMovie);
        }

        private List<MediaRecord> ImdbApiCall(string title)
        {
            var client = new RestClient($"https://imdb8.p.rapidapi.com/title/find?q={title}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d5490fab4bmsh0bc23a740cc46a9p1969dejsnee359aef5d88");
            IRestResponse response = client.Execute(request);

            return CreateMovieRecordsFromJson(response.Content);
        }

        private List<MediaRecord> CreateMovieRecordsFromJson(string jsonContent)
        {
            var json = JObject.Parse(jsonContent);
            var data = json["results"];

            return data.Select(CreateMovieRecord).Where(el => el != null).ToList();
        }

        private MediaRecord CreateMovieRecord(JToken item)
        {
            if (item["image"] == null)
            {
                return null;
            }

            int year = item.Value<int>("year");
            string title = item.Value<string>("title");
            string imdbUrl = "https://www.imdb.com" + item.Value<string>("id");
            string thumbnailUrl = item["image"].Value<string>("url");

            MediaRecord newMovie = new MediaRecord() { Title = title, Year = year, ImdbUrl = imdbUrl, ThumbnailUrl = thumbnailUrl };
            return (year == 0 || title == null || imdbUrl == null) ? null : newMovie;
        }
    }
}
