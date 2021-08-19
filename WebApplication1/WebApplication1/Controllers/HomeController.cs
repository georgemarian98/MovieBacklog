using Backlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBacklog.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoviesService moviesService;

        public HomeController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        // GET: MovieController
        public ActionResult Index()
        {
            return View(moviesService.GetBacklog());
        }

        [HttpDelete]
        public void DeleteMovie(int Id)
        {
            moviesService.RemoveMovie(Id);
        }

        [HttpPost]
        public void AddMovie(string title)
        {
            var movies = ImdbApiCall(title);

            moviesService.AddMovieToBacklog(movies[0]);
        }

        private List<MovieRecord> ImdbApiCall(string title)
        {
            title = title.Replace(" ", "%20");
            var client = new RestClient($"https://imdb8.p.rapidapi.com/title/find?q={title}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d5490fab4bmsh0bc23a740cc46a9p1969dejsnee359aef5d88");
            IRestResponse response = client.Execute(request);

            return CreateMovieRecordsFromJson(response.Content);
        }

        private List<MovieRecord> CreateMovieRecordsFromJson(string jsonContent)
        {
            var json = JObject.Parse(jsonContent);
            var data = json["results"];

            return data.Select(CreateMovieRecord).Where(el => el != null).ToList();
        }

        private MovieRecord CreateMovieRecord(JToken item)
        {
            if (item["image"] == null)
            {
                return null;
            }

            int year = item.Value<int>("year");
            string title = item.Value<string>("title");
            string imdbUrl = "https://www.imdb.com" + item.Value<string>("id");


            string thumbnailUrl = item["image"].Value<string>("url");


            return new MovieRecord(title, year, imdbUrl, thumbnailUrl);
        }
    }
}
