using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;

namespace MusicServices.Services.Shared
{
    public class REST
    {
        /// <summary>
        /// Execure a http GET Request and deserialize response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<dynamic> Get<T>(HttpClient httpClient, string url)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string responseString = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}
