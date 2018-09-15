using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureAITestMobile.Services
{
    public class BingSpeechService
    {
        const string AUTH_HOST = "https://api.cognitive.microsoft.com/sts/v1.0";
        const string API_HOST = "https://speech.platform.bing.com/synthesize";
        readonly string KEY = Helpers.Secrets.BingSpeechKey;
        readonly string _authPath = $"{AUTH_HOST}/issueToken";

        async Task<string> GetToken()
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_authPath);
                request.Headers.Add("Ocp-Apim-Subscription-Key", KEY);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                return responseBody;
            }

        }

        public async Task<Stream> Speak(string text)
        {
            var token = await GetToken();

            var requestBody = $@"<speak version='1.0' xml:lang='pl-PL'><voice xml:lang='pl-PL' xml:gender='Female' name='Microsoft Server Speech Text to Speech Voice (pl-PL, PaulinaRUS)'>{text}</voice></speak>";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(API_HOST);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/ssml+xml");
                request.Headers.Add("X-Microsoft-OutputFormat", "raw-16khz-16bit-mono-pcm");
                request.Headers.Add("X-Search-AppId", Guid.NewGuid().ToString().Replace("-", ""));
                request.Headers.Add("X-Search-ClientID", Guid.NewGuid().ToString().Replace("-", ""));
                request.Headers.Add("User-Agent", "Azure AI Test");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStreamAsync();

                return responseBody;
            }
        }
    }
}
