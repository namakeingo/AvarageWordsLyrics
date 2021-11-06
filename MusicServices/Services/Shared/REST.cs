using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicServices.Services.Shared
{
    public class REST
    {
        // This method uses the shared instance of HttpClient for every call to GET
        public static async Task<dynamic> Get<T>(HttpClient httpClient, string url)
        {
            return JsonSerializer.Deserialize<T>(await httpClient.GetStringAsync(url)); ;
        }

        // This method uses the shared instance of HttpClient for every call to POST
        public static async Task<dynamic> Post<T>(HttpClient httpClient, string url, Dictionary<string, string> data)
        {
            var postData = new FormUrlEncodedContent(data);

            var response = await httpClient.PostAsync(url, postData);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseString);
        }
    }
}
