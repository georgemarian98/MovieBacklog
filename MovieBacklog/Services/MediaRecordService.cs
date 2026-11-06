using System.Collections.Generic;
using System.Linq;
using MovieBacklog.Models;
using MovieBacklog.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MovieBacklog.Services
{
    public class MediaService : IMediaRecordService
    {
        private readonly ApplicationDbContext applicationDb;

        public MediaService(ApplicationDbContext applicationDb)
        {
            this.applicationDb = applicationDb;
        }

        public bool AddMediaRecordToBacklog(MediaRecord media)
        {
            if (media == null)
            {
                return false;
            }

            applicationDb.Add(media);
            int rowsAffected = applicationDb.SaveChanges();
            return rowsAffected == 1;
        }

        public List<MediaRecord> GetBacklog(string user = null)
        {
            IQueryable<MediaRecord> query = applicationDb.Media.OrderBy(media => media.Title);

            if (user != null)
            {
                query = query.Where(media => media.AddedBy == user);
            }
            return query.ToList();
        }

        public List<MediaRecord> GetBacklogView(string user = null)
        {
            IQueryable<MediaRecord> query = applicationDb.Media
                .AsNoTracking()
                .OrderBy(media => media.Title)
                .Select(media => new MediaRecord
                {
                    Id = media.Id,
                    Title = media.Title,
                    DateAdded = media.DateAdded,
                    ImdbUrl = media.ImdbUrl,
                    Year = media.Year,
                    IsMovie = media.IsMovie,
                    AddedBy = media.AddedBy
                });

            if (user != null)
            {
                query = query.Where(media => media.AddedBy == user);
            }
            return query.ToList();
        }

        public byte[] GetThumbnail(int id)
        {
            return applicationDb.Media
                .Where(media => media.Id == id)
                .Select(media => media.Image)
                .FirstOrDefault();
        }

        public bool RemoveMediaRecord(int id)
        {
            var media = applicationDb.Media.Find(id);
            if (media == null)
            {
                return false;
            }

            applicationDb.Media.Remove(media);
            int rowsAffected = applicationDb.SaveChanges();
            return rowsAffected == 1;
        }

        public MediaRecord GetMediaRecord(int id)
        {
            return applicationDb.Media.Find(id);
        }

        public bool UpdateMediaRecord(MediaRecord record)
        {
            if (record == null)
            {
                return false;
            }
            applicationDb.Media.Update(record);
            int rowsAffected = applicationDb.SaveChanges();
            return rowsAffected == 1;
        }

        public bool ImportBacklogFromJsonFile(string filePath, string user)
        {
            if (System.IO.File.Exists(filePath) == false)
            {
                return false;
            }

            string jsonInput = System.IO.File.ReadAllText(filePath);
            var mediaRecords = JsonSerializer.Deserialize<List<MediaRecord>>(jsonInput);

            if (mediaRecords == null || mediaRecords.Count == 0)
            {
                return false;
            }

            return ImportBacklog(mediaRecords, user);
        }

        public bool ImportBacklog(List<MediaRecord> mediaRecords, string user)
        {
            // Remove all existing media records
            List<MediaRecord> existing = GetBacklog(user);

            if (existing.Any())
            {
                applicationDb.Media.RemoveRange(existing);
            }

            applicationDb.Media.AddRange(mediaRecords);
            applicationDb.SaveChanges();

            List<MediaRecord> currentMediaRecords = GetBacklog(user);
            currentMediaRecords.Sort((x, y) => x.Title.CompareTo(y.Title));
            mediaRecords.Sort((x, y) => x.Title.CompareTo(y.Title));

            if (currentMediaRecords.Count != mediaRecords.Count)
            {
                return false;
            }

            return currentMediaRecords.SequenceEqual(mediaRecords);
        }
    }
}
