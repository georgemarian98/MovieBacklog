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

        public List<MediaRecord> GetBacklog()
        {
            return applicationDb.Media.OrderBy(media => media.Title).ToList();
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

        public bool ExportBacklogToJson(string filePath)
        {
            if (filePath == null)
            {
                return false;
            }

            string jsonOutput = JsonSerializer.Serialize(applicationDb.Media);

            System.IO.File.WriteAllText(filePath, jsonOutput);
            return true;
        }

        public bool ImportBacklogFromJson(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
            {
                return false;
            }

            string jsonInput = System.IO.File.ReadAllText(filePath);
            var mediaRecords = JsonSerializer.Deserialize<List<MediaRecord>>(jsonInput);

            // Remove all existing media records
            var existing = applicationDb.Media.ToList();
            if (existing.Any())
            {
                applicationDb.Media.RemoveRange(existing);
            }

            applicationDb.Media.AddRange(mediaRecords);
            applicationDb.SaveChanges();

            return true;
        }
    }
}
