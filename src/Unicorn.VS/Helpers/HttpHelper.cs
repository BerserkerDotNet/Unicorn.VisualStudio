using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Helpers
{
    public class HttpHelper
    {
        private readonly UnicornConnection _connection;
        private readonly string _command;
        private string _configuration;
        private readonly Dictionary<string, string> _additionalKeys;

        public const string ConfigCommand = "Config";
        public const string StartSyncCommand = "Sync";
        public const string StartReserializeCommand = "Reserialize";
        public const string HandshakeCommand = "Handshake";
        public const string DefaultConfiguration = "All";

        public HttpHelper(UnicornConnection connection, string command)
        {
            _connection = connection;
            _command = command;
            _additionalKeys = new Dictionary<string, string>(1);
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

        public string Build()
        {
            var endpointUrl = _connection.ServerUrl;
            if (!endpointUrl.StartsWith("http"))
                endpointUrl = "http://" + endpointUrl;
            var configuration = GetConfiguration();
            endpointUrl += "/unicornRemote.aspx?verb=" + _command;
            if (!string.IsNullOrEmpty(configuration))
                endpointUrl += configuration;
            var keys = _additionalKeys.Aggregate(string.Empty,
                (s, p) => "&" + s + p.Key + "=" + Uri.EscapeDataString(p.Value));
            endpointUrl += keys.TrimEnd('&', '=');
            return endpointUrl;
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