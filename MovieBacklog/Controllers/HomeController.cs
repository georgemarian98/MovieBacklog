using MovieBacklog.Models;
using Microsoft.AspNetCore.Mvc;
using MovieBacklog.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using MovieBacklog.Models.ViewModels;

namespace Backlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediaRecordService mediaRecordService;
        private static readonly UsersViewModel user = new();

        public HomeController(IMediaRecordService mediaRecordService)
        {
            this.mediaRecordService = mediaRecordService;
        }

        public ActionResult Movies()
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            var list = mediaRecordService.GetBacklog().Where(mediaRecord => mediaRecord.IsMovie == true && mediaRecord.AddedBy == user.CurrentUser);
            return View(list);
        }
        
        public ActionResult TvShows()
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            return View(mediaRecordService.GetBacklog().Where(mediaRecord => mediaRecord.IsMovie == false && mediaRecord.AddedBy == user.CurrentUser));
        }

        [HttpPost]
        public ActionResult Users(string newUser)
        {
            user.CurrentUser = newUser;
            return RedirectToAction("Movies");
        }

        [HttpGet]
        public JsonResult CurrentUser()
        {
            return Json(user.CurrentUser);
        }

        [HttpPost]
        public ActionResult Search([Bind("mediaRecordTitle")] string mediaRecordTitle)
        {
            ViewBag.ItemTypesSelectList = user.AllUsers;

            var mediaRecordBacklog = mediaRecordService.GetBacklog();
            var foundMediaRcords = ImdbApiCall(mediaRecordTitle)
                .Where(mediaRecord => 
                mediaRecordBacklog.Exists(mediaRecordBacklog => mediaRecordBacklog.Title == mediaRecord.Title && mediaRecordBacklog.AddedBy == user.CurrentUser) == false);

            return View(foundMediaRcords);
        }

        [HttpDelete]
        public void DeleteMediaRecord(int id)
        {
            mediaRecordService.RemoveMediaRecord(id);
        }

        [HttpPost]
        public void AddMediaRecord(string title, int year, string imdbUrl, string thumbnailUrl, bool isMovie)
        {
            MediaRecord newMediaRecord = new() { 
                Title = title, 
                Year = year, 
                ImdbUrl = imdbUrl, 
                ThumbnailUrl = thumbnailUrl, 
                IsMovie = isMovie, 
                AddedBy = user.CurrentUser 
            };

            mediaRecordService.AddMediaRecordToBacklog(newMediaRecord);
        }

        private List<MediaRecord> ImdbApiCall(string title)
        {
            var client = new RestClient($"https://imdb8.p.rapidapi.com/title/find?q={title}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d5490fab4bmsh0bc23a740cc46a9p1969dejsnee359aef5d88");
            IRestResponse response = client.Execute(request);

            return CreateMediaRecordsFromJson(response.Content);
        }

        private List<MediaRecord> CreateMediaRecordsFromJson(string jsonContent)
        {
            var json = JObject.Parse(jsonContent);
            var data = json["results"];

            return data.Select(CreateNewMediaRecord).Where(el => el != null).ToList();
        }

        private MediaRecord CreateNewMediaRecord(JToken item)
        {
            if (item["image"] == null)
            {
                return null;
            }

            int year = item.Value<int>("year");
            string title = item.Value<string>("title");
            string imdbUrl = "https://www.imdb.com" + item.Value<string>("id");
            string thumbnailUrl = item["image"].Value<string>("url");
            bool type = item.Value<string>("titleType") == "movie";
            string titleType = item.Value<string>("titleType"); //TODO: Keep track for each media type

            MediaRecord newMediaRecord = new MediaRecord() { Title = title, Year = year, ImdbUrl = imdbUrl, ThumbnailUrl = thumbnailUrl, IsMovie = type };
            return (year == 0 || title == null || imdbUrl == null) ? null : newMediaRecord;
        }
    }
}
