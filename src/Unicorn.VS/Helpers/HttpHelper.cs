using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Unicorn.VS.Models;

namespace Unicorn.VS.Helpers
{
    public class HttpHelper
    {
        public static string CurrentClientVersion = "1.0.0.0";
        private readonly UnicornConnection _connection;
        private readonly string _command;
        private string _id;
        private string _configuration;
        private readonly Dictionary<string, string> _additionalKeys;

        public const string ConfigCommand = "Config";
        public const string StartSyncCommand = "Sync";
        public const string StartReserializeCommand = "Reserialize";
        public const string FinishSyncCommand = "Finish";
        public const string ReportCommand = "Report";
        public const string HandshakeCommand = "Handshake";
        public const string DefaultConfiguration = "All";

        public HttpHelper(UnicornConnection connection, string command)
        {
            _connection = connection;
            _command = command;
            _additionalKeys = new Dictionary<string, string>(1);
        }

        public HttpHelper WithId(string id)
        {
            _id = id;
            return this;
        }

        public HttpHelper WithConfiguration(string configuration)
        {
            _configuration = configuration;
            return this;
        }

        public HttpHelper WithKey(string key, string value)
        {
            _additionalKeys.Add(key, value);
            return this;
        }

        public async Task<T> Execute<T>(CancellationToken ct)
        {
            if (_connection == null)
            {
                MessageBox.Show("Please select server to synchronize", "Server not selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return default(T);
            }

            var response = await ExecuteInternal(ct);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }

            return default(T);
        }

        public async Task<HttpResponseMessage> ExecuteRaw(CancellationToken ct)
        {
            if (_connection == null)
            {
                MessageBox.Show("Please select server to synchronize", "Server not selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return null;
            }

            return await ExecuteInternal(ct);
        }

        private async Task<HttpResponseMessage> ExecuteInternal(CancellationToken ct)
        {
            using (var client = new HttpClient())
            {
                var endpointUrl = _connection.ServerUrl;
                if (!endpointUrl.StartsWith("http"))
                    endpointUrl = "http://" + endpointUrl;
                var configuration = GetConfiguration();
                endpointUrl += "/unicornRemote.aspx?verb=" + _command;
                if (!string.IsNullOrEmpty(_id))
                    endpointUrl += "&id=" + _id;
                if (!string.IsNullOrEmpty(configuration))
                    endpointUrl += configuration;
                var keys = _additionalKeys.Aggregate(string.Empty,
                    (s, p) => "&" + s + p.Key + "=" + Uri.EscapeDataString(p.Value));
                endpointUrl += keys.TrimEnd('&', '=');
                var response = await client.GetAsync(endpointUrl, ct);
                IEnumerable<string> values;
                _connection.IsUpdateRequired = !response.Headers.TryGetValues("X-Remote-Version", out values) || values.All(v => v != CurrentClientVersion);
                return response;
            }
        }

        private string GetConfiguration()
        {
            if (string.IsNullOrEmpty(_configuration))
                return String.Empty;

            return (_configuration.Equals(DefaultConfiguration, StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : "&configuration=" + Uri.EscapeUriString(_configuration));
        }
        
    }
}