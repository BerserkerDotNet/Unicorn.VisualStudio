using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using MicroCHAP;
using Unicorn.VS.Models;

namespace Unicorn.VS.Helpers
{
    public static class UnicornConnectionHelper
    {
        public static HttpHelper Get(this UnicornConnection connection, string command)
        {
            return new HttpHelper(connection, command);
        }

        public static HttpClient CreateClient(this UnicornConnection connection, string url)
        {
            var httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(connection.Token))
            {
                var challenge = GetChallenge(connection);
                if (string.IsNullOrEmpty(challenge))
                {
                    httpClient.DefaultRequestHeaders.Add("Authenticate", connection.Token);
                }
                else
                {
                    var signatureService = new SignatureService(connection.Token);
                    var signature = signatureService.CreateSignature(challenge, url, Enumerable.Empty<SignatureFactor>());
                    httpClient.DefaultRequestHeaders.Add("Authorization", signature);
                    httpClient.DefaultRequestHeaders.Add("X-Nonce", challenge);
                }

            }
            return httpClient;
        }


        private static string GetChallenge(UnicornConnection connection)
        {
            var client = new HttpClient();
            var endPoint = connection.Get("Challenge").Build();

            var challengeResponse = client.GetAsync(endPoint)
                .GetAwaiter()
                .GetResult();

            if (challengeResponse.StatusCode == HttpStatusCode.NotFound)
                return string.Empty;

            if(!challengeResponse.IsSuccessStatusCode)
                throw new UnauthorizedAccessException();

            return challengeResponse.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}