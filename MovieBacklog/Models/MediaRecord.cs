using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBacklog.Models
{
    public class MediaRecord
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ImdbUrl { get; set; }

        public int Year { get; set; }

        public bool IsMovie { get; set; }

        public string AddedBy { get; set; }
    }
}
