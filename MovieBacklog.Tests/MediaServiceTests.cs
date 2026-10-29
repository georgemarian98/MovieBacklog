using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieBacklog.Data;
using MovieBacklog.Models;
using MovieBacklog.Services;
using Xunit;

namespace MovieBacklog.Tests;

public class MediaServiceTests
{
    [Fact]
    public void GetBacklog_ReturnsRecordsSortedByTitleAndFilteredByUser()
    {
        using var context = CreateContext();
        context.Media.AddRange(
            new MediaRecord { Title = "Zebra", AddedBy = "George", IsMovie = true },
            new MediaRecord { Title = "Alpha", AddedBy = "George", IsMovie = true },
            new MediaRecord { Title = "Beta", AddedBy = "Rebeka", IsMovie = false });
        context.SaveChanges();
        var service = new MediaService(context);

        var allRecords = service.GetBacklog();
        var georgeRecords = service.GetBacklog("George");

        Assert.Equal(new[] { "Alpha", "Beta", "Zebra" }, allRecords.Select(record => record.Title));
        Assert.Equal(new[] { "Alpha", "Zebra" }, georgeRecords.Select(record => record.Title));
    }

    [Fact]
    public void AddGetUpdateAndRemove_ManageMediaRecord()
    {
        using var context = CreateContext();
        var service = new MediaService(context);
        var record = new MediaRecord { Title = "Arrival", AddedBy = "George", Year = 2016 };

        service.AddMediaRecordToBacklog(record);
        var stored = service.GetMediaRecord(record.Id);
        Assert.NotNull(stored);
        Assert.Equal("Arrival", stored!.Title);

        stored.Title = "Arrival Updated";
        service.UpdateMediaRecord(stored);
        Assert.Equal("Arrival Updated", service.GetMediaRecord(record.Id)!.Title);

        service.RemoveMediaRecord(record.Id);
        Assert.Null(service.GetMediaRecord(record.Id));
    }

    [Fact]
    public void ImportBacklogFromJson_ReturnsFalseForMissingOrEmptyFile()
    {
        using var context = CreateContext();
        var service = new MediaService(context);
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var emptyPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        File.WriteAllText(emptyPath, "[]");
        try
        {
            Assert.False(service.ImportBacklogFromJson(missingPath, "George"));
            Assert.False(service.ImportBacklogFromJson(emptyPath, "George"));
        }
        finally
        {
            File.Delete(emptyPath);
        }
    }

    [Fact]
    public void ImportBacklog_ReplacesOnlyTheSelectedUsersRecords()
    {
        using var context = CreateContext();
        context.Media.AddRange(
            new MediaRecord { Title = "Old George", AddedBy = "George" },
            new MediaRecord { Title = "Keep Rebeka", AddedBy = "Rebeka" });
        context.SaveChanges();
        var service = new MediaService(context);
        var imported = new List<MediaRecord>
        {
            new() { Title = "New George A", AddedBy = "George" },
            new() { Title = "New George B", AddedBy = "George" }
        };

        var result = service.ImportBacklog(imported, "George");

        Assert.True(result);
        Assert.Equal(new[] { "New George A", "New George B" }, service.GetBacklog("George").Select(record => record.Title));
        Assert.Equal(new[] { "Keep Rebeka" }, service.GetBacklog("Rebeka").Select(record => record.Title));
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
