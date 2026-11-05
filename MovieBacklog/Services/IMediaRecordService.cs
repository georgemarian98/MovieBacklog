using System.Collections.Generic;
using MovieBacklog.Models;

namespace MovieBacklog.Services
{
    public interface IMediaRecordService
    {
        public List<MediaRecord> GetBacklog(string user = null);

        public byte[] GetThumbnail(int id);

        public bool AddMediaRecordToBacklog(MediaRecord movie);

        public bool RemoveMediaRecord(int id);

        public MediaRecord GetMediaRecord(int id);

        public bool UpdateMediaRecord(MediaRecord record);

        // Import backlog from a JSON file. Warning: this will erase the existing backlog!
        public bool ImportBacklogFromJsonFile(string filePath, string user);

        // Replace the existing backlog with the list of MediaRecords
        public bool ImportBacklog(List<MediaRecord> mediaRecords, string user);
    }
}
