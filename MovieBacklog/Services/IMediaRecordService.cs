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

        public MediaRecord GetMediaRecord(int id);

        public void UpdateMediaRecord(MediaRecord record);

        // Export the entire backlog to a JSON file
        public bool ExportBacklogToJson(string filePath);

        // Import backlog from a JSON file. Warning: this will erase the existing backlog!
        public bool ImportBacklogFromJson(string filePath);
    }
}
