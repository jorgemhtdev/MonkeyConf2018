﻿namespace MonkeyConf2018.Services
{
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using MonkeyConf2018.Models;
    using Newtonsoft.Json;
    using Plugin.Media.Abstractions;

    public class PredictionService
    {
        public async Task<VisionResult> MakePredictionRequestAsync(MediaFile file)
        {
            var client = new HttpClient();

            // Request headers - Send in the header your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", AppSettings.PredictionKey);

            // Request body. Loads Image from Disk.
            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                // Set Content Type to Stream
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Call the Prediction API
                HttpResponseMessage response = await client.PostAsync(AppSettings.PredictionURL, content);

                var responseString = await response.Content.ReadAsStringAsync();

                // Convert into VisionResult Model in hotdog.model for easier manipulation
                return JsonConvert.DeserializeObject<VisionResult>(responseString);
            }
        }

        private byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }
    }
}
