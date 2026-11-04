using System;
using System.Net.Http;

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
            return new MediaRecord
            {
                Title = this.Title,
                Year = this.Year,
                ImdbUrl = this.ImdbUrl,
                IsMovie = this.IsMovie,
                AddedBy = addedBy,
                DateAdded = DateTime.Now,
                Image = DownloadImage(this.ThumbnailUrl)
            };
        }

        public static byte[] DownloadImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            HttpClient client = new HttpClient(); // TODO: use RestSharp to remain consistant with the rest of the project
            HttpResponseMessage response = client.GetAsync(imageUrl).GetAwaiter().GetResult();
            byte[] imageBytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            return imageBytes;
        }
    }
}
