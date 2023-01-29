using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieBacklog.Models;

namespace MovieBacklog.Services
{
    public interface IMediaRecordService
    {
        public List<MediaRecord> GetBacklog();

        public void AddMediaRecordToBacklog(MediaRecord movie);

        public void RemoveMediaRecord(int id);
    }
}
