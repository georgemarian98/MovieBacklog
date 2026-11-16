using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieBacklog.Models;
using MovieBacklog.Models.ViewModels;
using MovieBacklog.Services;
using MovieBacklog.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MovieBacklog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IMediaRecordService mediaRecordService;
        private static readonly UsersViewModel user = new();

        public HomeController(ILogger<HomeController> logger, IMediaRecordService mediaRecordService)
        {
            this.logger = logger;
            this.mediaRecordService = mediaRecordService;
        }

        public ActionResult Movies()
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            var movies = mediaRecordService.GetBacklogView(user.CurrentUser).Where(mediaRecord => mediaRecord.IsMovie == true);
            logger.LogInformation("Returned '{}' movies for user '{}'.", movies.Count(), user.CurrentUser);
            foreach (var item in movies)
            {
                logger.LogDebug("\tReturning movie: '{}'.", item.Title);
            }
            return View(movies);
        }
        
        public ActionResult TvShows()
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            var shows = mediaRecordService.GetBacklogView(user.CurrentUser).Where(mediaRecord => mediaRecord.IsMovie == false);
            logger.LogInformation("Returned '{}' TV shows for user '{}'.", shows.Count(), user.CurrentUser);
            foreach (var item in shows)
            {
                logger.LogDebug("\tReturning TV show: '{}'.", item.Title);
            }
            return View(shows);
        }

        [HttpGet]
        public ActionResult Thumbnail(int id)
        {
            byte[] image = mediaRecordService.GetThumbnail(id);
            logger.LogDebug("Returning thumbnail for media record with ID '{}'.", id);
            return image == null ? NotFound() : File(image, "image/webp");
        }

        [HttpPost]
        public ActionResult Users(string newUser)
        {
            user.CurrentUser = newUser;
            logger.LogInformation("Changed current user to '{}'.", newUser);
            return RedirectToAction("Movies");
        }

        [HttpGet]
        public JsonResult CurrentUser()
        {
            logger.LogDebug("Returning current user: '{}'.", user.CurrentUser);
            return Json(user.CurrentUser);
        }

        [HttpPost]
        public ActionResult Search([Bind("mediaRecordTitle")] string mediaRecordTitle)
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            var mediaRecordBacklog = mediaRecordService.GetBacklog(user.CurrentUser);
            var foundMediaRcords = ImdbApiCall(mediaRecordTitle)
                .Where(mediaRecord => 
                mediaRecordBacklog.Exists(mediaRecordBacklog => mediaRecordBacklog.Title == mediaRecord.Title) == false);
            
            logger.LogInformation("Found '{}' media records for user '{}'.", foundMediaRcords.Count(), user.CurrentUser);
            foreach (var item in mediaRecordBacklog)
            {
                logger.LogDebug("\tReturning movie: '{}'.", item.Title);
            }

            return View(foundMediaRcords);
        }

        [HttpDelete]
        public void DeleteMediaRecord(int id)
        {
            logger.LogInformation("Deleting media record with ID '{}' for user '{}'.", id, user.CurrentUser);
            if (!mediaRecordService.RemoveMediaRecord(id))
            {
                throw new System.Exception("Failed to remove media record from backlog.");
            }
        }

        [HttpPost]
        public void AddMediaRecord(string title, int year, string imdbUrl, string thumbnailUrl, bool isMovie)
        {
            SearchedMediaRecord newSearchedMediaRecord = new SearchedMediaRecord() { 
                Title = title, 
                Year = year, 
                ImdbUrl = imdbUrl, 
                ThumbnailUrl = thumbnailUrl,
                IsMovie = isMovie
            };

            MediaRecord media = newSearchedMediaRecord.ToMediaRecord(user.CurrentUser);
            logger.LogInformation("Adding media record '{}' to backlog for user '{}'.", media, user.CurrentUser);
            if (!mediaRecordService.AddMediaRecordToBacklog(media))
            {
                throw new System.Exception("Failed to add media record to backlog.");
            }
        }

        [HttpPost]
        public JsonResult UpdateRecord(int id)
        {
            MediaRecord record = mediaRecordService.GetMediaRecord(id);
            string titleId = record.ImdbUrl.Substring("https://www.imdb.com/title/".Length).Trim('/');
            JObject jsonObject = new JObject();

            // RapidAPI limits to just 5 requests per second. Sometime it is not enough
            for (int attempts = 0; attempts < 5; ++attempts)
            {
                var imdbClient = new RestClient($"https://imdb8.p.rapidapi.com/title/v2/get-details?tconst={titleId}");
                var imdbRequest = new RestRequest();
                imdbRequest.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
                imdbRequest.AddHeader("x-rapidapi-key", System.Environment.GetEnvironmentVariable("RAPIDAPI_KEY"));
                RestResponse imdbResponse = imdbClient.Execute(imdbRequest);

                if (imdbResponse.IsSuccessful)
                {
                    jsonObject = JObject.Parse(imdbResponse.Content);
                    break;
                }

                if (imdbResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                throw new System.Exception($"IMDB API error: {imdbResponse.StatusCode} - {imdbResponse.Content}");
            }

            string thumbnailUrl = jsonObject["data"]["title"]["primaryImage"].Value<string>("url");
            record.Image = ImageTools.DownloadImage(thumbnailUrl);
            if (!mediaRecordService.UpdateMediaRecord(record))
            {
                throw new System.Exception("Failed to update media record.");
            }

            return Json(record.Image);
        }

        [HttpPost]
        public void ImportBacklog(string jsonInputString)
        {
            var mediaRecords = JsonConvert.DeserializeObject<List<MediaRecord>>(jsonInputString);
            mediaRecords.RemoveAll(mediaRecord => mediaRecord.AddedBy != user.CurrentUser);
            mediaRecords.ForEach(mediaRecord => mediaRecord.Id = 0); // Reset IDs to avoid conflicts

            if (mediaRecords == null || mediaRecords.Count == 0)
            {
                throw new System.Exception("No valid media records found in the provided JSON input.");
            }

            if (!mediaRecordService.ImportBacklog(mediaRecords, user.CurrentUser))
            {
                throw new System.Exception("Failed to import media records.");
            }

            logger.LogInformation("Imported '{}' media records for user '{}'.", mediaRecords.Count, user.CurrentUser);
            foreach (var item in mediaRecords)
            {
                logger.LogDebug("Imported media '{}'.", item);
            }
        }

        [HttpPost]
        public JsonResult ExportBacklog()
        {
            List<MediaRecord> backlog = mediaRecordService.GetBacklog(user.CurrentUser);
            string jsonString = JsonConvert.SerializeObject(backlog, Formatting.Indented);

            logger.LogInformation("Exported '{}' media records for user '{}'.", backlog.Count, user.CurrentUser);
            foreach (var item in jsonString)
            {
                logger.LogDebug("Exported media '{}'", item);
            }
            return Json(jsonString);
        }

        private List<SearchedMediaRecord> ImdbApiCall(string title)
        {
            var client = new RestClient($"https://imdb8.p.rapidapi.com/title/find?q={title}");
            var request = new RestRequest();
            request.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", System.Environment.GetEnvironmentVariable("RAPIDAPI_KEY"));
            RestResponse response = client.Execute(request);

            logger.LogDebug("IMDB API call for title '{}' returned status code '{}'.", title, response.StatusCode);
            logger.LogDebug("IMDB API response content: '{}'.", response.Content);
            return CreateMediaRecordsFromJson(response.Content);
        }

        private List<SearchedMediaRecord> CreateMediaRecordsFromJson(string jsonContent)
        {
            var json = JObject.Parse(jsonContent);
            var data = json["results"];

            return data.Select(CreateNewSearchedMediaRecord).Where(el => el != null).ToList();
        }

        private SearchedMediaRecord CreateNewSearchedMediaRecord(JToken item)
        {
            if (item["image"] == null)
            {
                return null;
            }

            int year = item.Value<int>("year");
            string title = item.Value<string>("title");
            string imdbUrl = "https://www.imdb.com" + item.Value<string>("id");
            string thumbnailUrl = item["image"].Value<string>("url");
            bool isMovie = item.Value<string>("titleType") == "movie";
            string titleType = item.Value<string>("titleType"); //TODO: Keep track for each media type

            SearchedMediaRecord newSearchedMediaRecord = new SearchedMediaRecord
            { 
                Title = title,
                Year = year,
                ImdbUrl = imdbUrl,
                ThumbnailUrl = thumbnailUrl,
                IsMovie = isMovie
            };
            return (year == 0 || title == null || imdbUrl == null) ? null : newSearchedMediaRecord;
        }
    }
}
