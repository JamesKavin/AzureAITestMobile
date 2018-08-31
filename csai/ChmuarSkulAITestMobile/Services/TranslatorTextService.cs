using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChmurSkulAITestMobile.Services
{
    public class TranslatorTextService
    {
        const string HOST = "https://api.cognitive.microsofttranslator.com";
        readonly string KEY = AppConfig.Get(AppConfig.TranslatorTextKey);
        readonly string _path = $"{HOST}/translate?api-version=3.0";
        readonly string _params = "&to={0}";

        public async Task<string> Translate(string text, string targetLanguage = "pl")
        {
            string url = _path + string.Format(_params, targetLanguage);

            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(url);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", KEY);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseBody);

                try
                {
                    return result.First.translations.First.text;
                }
                catch (Exception ex)
                {

                }

                return null;
            }
        }
    }
}
