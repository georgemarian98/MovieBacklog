using System.Collections.Generic;
using System.Linq;
using MovieBacklog.Models;
using MovieBacklog.Data;
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

        public void AddMediaRecordToBacklog(MediaRecord media)
        {
            applicationDb.Add(media);
            applicationDb.SaveChanges();
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

        public void RemoveMediaRecord(int id)
        {
            var media = applicationDb.Media.Find(id);
            applicationDb.Media.Remove(media);
            applicationDb.SaveChanges();
        }

        public MediaRecord GetMediaRecord(int id)
        {
            return applicationDb.Media.Find(id);
        }

        public void UpdateMediaRecord(MediaRecord record)
        {
            applicationDb.Media.Update(record);
            applicationDb.SaveChanges();
        }

        public bool ImportBacklogFromJson(string filePath, string user)
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

            return currentMediaRecords.SequenceEqual(mediaRecords);
        }
    }
}
