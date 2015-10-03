using System.Net.Http;
using System.Net.Http.Headers;
using Unicorn.VS.Models;

namespace Unicorn.VS.Helpers
{
    public static class UnicornConnectionHelper
    {
        public static HttpHelper Get(this UnicornConnection connection, string command)
        {
            return new HttpHelper(connection, command);
        }

        public static HttpClient CreateClient(this UnicornConnection connection)
        {
            var httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(connection.Token))
                httpClient.DefaultRequestHeaders.Add("Authenticate", connection.Token);
            return httpClient;
        }
    }
}