using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieBacklog.Models;
using MovieBacklog.Data;

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
            return applicationDb.Media.ToList();
        }

        public void RemoveMediaRecord(int id)
        {
            var media = applicationDb.Media.Find(id);
            applicationDb.Media.Remove(media);
            applicationDb.SaveChanges();
        }
    }
}
