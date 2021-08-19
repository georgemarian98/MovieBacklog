using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backlog.Models
{
    public class MovieRecord
    {
        private static int idCounter = 0; 
        public MovieRecord(string title, int year, string imdbUrl, string thumbnailUrl)
        {
            Id = idCounter++;
            Title = title;
            Year = year;
            ImdbUrl = imdbUrl;
            ThumbnailUrl = thumbnailUrl;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ImdbUrl { get; set; }

        public int Year { get; set; }
    }
}
