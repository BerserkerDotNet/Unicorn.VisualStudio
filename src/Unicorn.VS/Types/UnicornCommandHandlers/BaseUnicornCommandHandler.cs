using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MicroCHAP;
using Unicorn.VS.Data;
using Unicorn.VS.Models;
using Unicorn.VS.Types.Interfaces;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public abstract class BaseUnicornCommandHandler<TCommand, TResult> : IUnicornCommandHandler<TCommand, TResult> 
        where TCommand : IUnicornCommand<TResult>
    {
        public const string DefaultConfiguration = "All";
        protected readonly bool _isStreamed;

        protected BaseUnicornCommandHandler(bool isStreamed=false)
        {
            _isStreamed = isStreamed;
        }

        public async Task<TResult> Execute(TCommand ctx)
        {
            var endPoint = BuildUrl(ctx.Connection, ctx.SelectedConfigurations);
            using (var client = await GetAuthorizedClient(ctx, endPoint).ConfigureAwait(false))
            {
                var response = await client.GetAsync(endPoint,
                    _isStreamed ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead,
                    ctx.CancellationToken).ConfigureAwait(false);
                IEnumerable<string> values;
                if (response.Headers.TryGetValues("X-Unicorn-Version", out values))
                {
                    Version version;
                    Version.TryParse(values.First(), out version);
                    ctx.UnicornVersion = version ?? new Version();
                }
                return await ProcessResponse(response, ctx).ConfigureAwait(false);
            }
        }

        protected abstract Task<TResult> ProcessResponse(HttpResponseMessage response, TCommand context);

        protected virtual async Task<HttpClient> GetAuthorizedClient(TCommand context, string url)
        {
            return await CreateClient(context.Connection, url).ConfigureAwait(false);
        }

        protected virtual string BuildUrl(UnicornConnection connection, string selectedConfiguration = "", string verbOverride = null)
        {
            var endpointUrl = connection.ServerUrl;
            if (!endpointUrl.StartsWith("http"))
                endpointUrl = "http://" + endpointUrl;
            var configuration = GetConfiguration(selectedConfiguration);
            endpointUrl += $"{SettingsHelper.GetSettings().EndPoint}?verb={verbOverride ?? GetVerb(connection)}";
            if (!string.IsNullOrEmpty(configuration))
                endpointUrl += configuration;
            return endpointUrl;
        }

        protected abstract string GetVerb(UnicornConnection connection);

        private async Task<HttpClient> CreateClient(UnicornConnection connection, string url)
        {
            var httpClient = new HttpClient();
            if(_isStreamed)
                httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

            if (!string.IsNullOrEmpty(connection.Token))
            {
                var challenge = await GetChallenge(connection).ConfigureAwait(false);
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

        private async Task<string> GetChallenge(UnicornConnection connection)
        {
            using (var client = new HttpClient())
            {
                var endPoint = BuildUrl(connection, verbOverride: "Challenge");

                var challengeResponse = await client.GetAsync(endPoint);

                if (challengeResponse.StatusCode == HttpStatusCode.NotFound ||
                    challengeResponse.StatusCode == HttpStatusCode.Unauthorized)
                    return string.Empty;

                if (!challengeResponse.IsSuccessStatusCode)
                    throw new UnauthorizedAccessException();

                return await challengeResponse.Content.ReadAsStringAsync();
            }
        }

        private string GetConfiguration(string selectedConfigurations)
        {
            if (string.IsNullOrEmpty(selectedConfigurations))
                return string.Empty;

            return (selectedConfigurations.Equals(DefaultConfiguration, StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : "&configuration=" + Uri.EscapeUriString(selectedConfigurations));
        }

    }
}