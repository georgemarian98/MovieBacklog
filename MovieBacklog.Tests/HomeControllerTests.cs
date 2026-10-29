using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MovieBacklog.Controllers;
using MovieBacklog.Models;
using MovieBacklog.Services;
using Xunit;

namespace MovieBacklog.Tests;

public class HomeControllerTests
{
    [Fact]
    public void Movies_ReturnsOnlyMoviesForTheCurrentUser()
    {
        var service = new FakeMediaRecordService
        {
            Records =
            [
                new MediaRecord { Title = "Movie", IsMovie = true, AddedBy = "George" },
                new MediaRecord { Title = "Show", IsMovie = false, AddedBy = "George" },
                new MediaRecord { Title = "Other User Movie", IsMovie = true, AddedBy = "Rebeka" }
            ]
        };
        var controller = new HomeController(service);

        var result = Assert.IsType<ViewResult>(controller.Movies());
        var model = Assert.IsAssignableFrom<IEnumerable<MediaRecord>>(result.Model);

        Assert.Equal(new[] { "Movie" }, model.Select(record => record.Title));
        Assert.Equal("George", service.LastRequestedUser);
    }

    [Fact]
    public void Users_ChangesCurrentUserAndRedirectsToMovies()
    {
        var controller = new HomeController(new FakeMediaRecordService());

        var result = Assert.IsType<RedirectToActionResult>(controller.Users("Rebeka"));

        Assert.Equal("Movies", result.ActionName);
        var currentUser = Assert.IsType<JsonResult>(controller.CurrentUser());
        Assert.Equal("Rebeka", currentUser.Value);
    }

    [Fact]
    public void AddMediaRecord_CreatesRecordForCurrentUser()
    {
        var service = new FakeMediaRecordService();
        var controller = new HomeController(service);
        controller.Users("George");

        controller.AddMediaRecord("Arrival", 2016, "imdb-url", "thumbnail-url", true);

        var added = Assert.Single(service.AddedRecords);
        Assert.Equal("Arrival", added.Title);
        Assert.Equal(2016, added.Year);
        Assert.Equal("imdb-url", added.ImdbUrl);
        Assert.Equal("thumbnail-url", added.ThumbnailUrl);
        Assert.True(added.IsMovie);
        Assert.Equal("George", added.AddedBy);
    }

    [Fact]
    public void DeleteMediaRecord_DelegatesToService()
    {
        var service = new FakeMediaRecordService();
        var controller = new HomeController(service);

        controller.DeleteMediaRecord(42);

        Assert.Equal(42, service.RemovedId);
    }

    [Fact]
    public void ExportBacklog_ReturnsSerializedRecordsForCurrentUser()
    {
        var service = new FakeMediaRecordService
        {
            Records = [new MediaRecord { Id = 7, Title = "Arrival", AddedBy = "George" }]
        };
        var controller = new HomeController(service);
        controller.Users("George");

        var result = Assert.IsType<JsonResult>(controller.ExportBacklog());
        var json = Assert.IsType<string>(result.Value);

        Assert.Contains("Arrival", json);
        Assert.Contains("\"Id\": 7", json);
        Assert.Equal("George", service.LastRequestedUser);
    }

    private sealed class FakeMediaRecordService : IMediaRecordService
    {
        public List<MediaRecord> Records { get; set; } = [];
        public List<MediaRecord> AddedRecords { get; } = [];
        public int? RemovedId { get; private set; }
        public string? LastRequestedUser { get; private set; }

        public List<MediaRecord> GetBacklog(string user)
        {
            LastRequestedUser = user;
            return user is null
                ? Records
                : Records.Where(record => record.AddedBy == user).ToList();
        }

        public void AddMediaRecordToBacklog(MediaRecord movie) => AddedRecords.Add(movie);

        public void RemoveMediaRecord(int id) => RemovedId = id;

        public MediaRecord GetMediaRecord(int id) => Records.SingleOrDefault(record => record.Id == id, new MediaRecord());

        public void UpdateMediaRecord(MediaRecord record) { }

        public bool ImportBacklogFromJson(string filePath, string user) => true;

        public bool ImportBacklog(List<MediaRecord> mediaRecords, string user) => true;
    }
}
