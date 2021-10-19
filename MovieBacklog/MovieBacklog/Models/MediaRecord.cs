using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backlog.Models
{
    public class MediaRecord
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ImdbUrl { get; set; }

        public int Year { get; set; }
    }
}
