using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChmurSkulAITestMobile.Services
{
    public class ComputerVisionService
    {
        const string HOST = "https://northeurope.api.cognitive.microsoft.com";
        readonly string KEY = AppConfig.Get(AppConfig.ComputerVisionKey);
        readonly string _path = $"{HOST}/vision/v2.0/describe";
        readonly string _params = "?maxCandidates=1";
        
        public async Task<string> GetImageDescription(byte[] image)
        {
            var content = new ByteArrayContent(image);
            content.Headers.Add("Content-Type", "application/octet-stream");
            return await GetImageDescriptionInternal(content);
        }

        public async Task<string> GetImageDescription(string imageUrl)
        {
            var body = new { url = imageUrl };
            var requestBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            return await GetImageDescriptionInternal(content);
        }

        private async Task<string> GetImageDescriptionInternal(ByteArrayContent content)
        {
            string url = _path + _params;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(url);
                request.Content = content;
                request.Headers.Add("Ocp-Apim-Subscription-Key", KEY);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);

                    return result.description.captions.First.text;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }
    }
}
