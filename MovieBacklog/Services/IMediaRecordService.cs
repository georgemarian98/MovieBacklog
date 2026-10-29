using System.Collections.Generic;
using MovieBacklog.Models;

namespace MovieBacklog.Services
{
    public interface IMediaRecordService
    {
        public List<MediaRecord> GetBacklog(string user = null);

        public void AddMediaRecordToBacklog(MediaRecord movie);

        public void RemoveMediaRecord(int id);

        public MediaRecord GetMediaRecord(int id);

        public void UpdateMediaRecord(MediaRecord record);

        // Import backlog from a JSON file. Warning: this will erase the existing backlog!
        public bool ImportBacklogFromJson(string filePath, string user);

        // Replace the existing backlog with the list of MediaRecords
        public bool ImportBacklog(List<MediaRecord> mediaRecords, string user);
    }
}
