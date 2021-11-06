using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MusicServices.Services.Shared
{
    public class SingleHttpClientInstanceController : ApiController
    {
        private static readonly HttpClient httpClient;
        static SingleHttpClientInstanceController()
        {
            httpClient = new HttpClient();
        }
        
        // This method uses the shared instance of HttpClient for every call to GET
        public async Task<string> Get(string url)
        {
            return await httpClient.GetStringAsync(url);
        }
        
        // This method uses the shared instance of HttpClient for every call to POST
        public static async Task<string> Post(string url, Dictionary<string,string> data)
        {
            var postData = new FormUrlEncodedContent(data);

            var response = await httpClient.PostAsync(url, postData);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
