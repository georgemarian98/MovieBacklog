using System;
using MovieBacklog.Utils;

namespace MovieBacklog.Models.ViewModels
{
    public class SearchedMediaRecord
    {
        public string Title { get; set; }
        public int Year { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ImdbUrl { get; set; }

        public bool IsMovie { get; set; }

        public MediaRecord ToMediaRecord(string addedBy)
        {
            byte[] mediaImage = ImageTools.DownloadImage(this.ThumbnailUrl);

            return new MediaRecord
            {
                Title = this.Title,
                Year = this.Year,
                ImdbUrl = this.ImdbUrl,
                IsMovie = this.IsMovie,
                AddedBy = addedBy,
                DateAdded = DateTime.Now,
                Image = ImageTools.ResizeImage(mediaImage)
            };
        }
    }
}
