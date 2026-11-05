using RestSharp;
using SkiaSharp;
using System;

namespace MovieBacklog.Utils
{
    public class ImageTools
    {
        public static byte[] DownloadImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }
            var client = new RestClient(imageUrl);
            var request = new RestRequest();
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful || response.RawBytes == null || response.RawBytes.Length == 0)
            {
                return null;
            }

            return response.RawBytes;
        }

        public static byte[] ResizeImage(byte[] imageBytes, int width = 300, int height = 450)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            using SKBitmap source = SKBitmap.Decode(imageBytes);
            if (source == null)
            {
                return null;
            }

            SKBitmap resized = source;
            bool disposeResized = false;

            try
            {
                if (source.Width > width || source.Height > height)
                {
                    float scale = Math.Min((float)width / source.Width, (float)height / source.Height);
                    int resizedWidth = Math.Max(1, (int)(source.Width * scale));
                    int resizedHeight = Math.Max(1, (int)(source.Height * scale));

                    resized = source.Resize(
                        new SKImageInfo(resizedWidth, resizedHeight),
                        new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

                    if (resized == null)
                    {
                        return null;
                    }

                    disposeResized = true;
                }

                using SKImage image = SKImage.FromBitmap(resized);
                using SKData output = image.Encode(SKEncodedImageFormat.Webp, 75);

                return output?.ToArray();
            }
            finally
            {
                if (disposeResized)
                {
                    resized.Dispose();
                }
            }
        }
    }
}
