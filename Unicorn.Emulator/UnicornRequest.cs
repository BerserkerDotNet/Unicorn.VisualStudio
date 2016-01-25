using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Unicorn.Emulator
{
    public class UnicornRequest
    {
        public UnicornRequest()
        {
            Query=new Dictionary<string, string>(5);
            Headers=new Dictionary<string, string>(5);
        }

        public string Verb => this["verb"].ToLower();
        public bool IsAuthenticated => !string.IsNullOrEmpty(this["X-MC-MAC"]) && !string.IsNullOrEmpty("X-MC-Nonce"); 
        public Dictionary<string, string> Query { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public string this[string name] => Query.ContainsKey(name) ? Query[name] : Headers.ContainsKey(name) ? Headers[name] : string.Empty;

        public static UnicornRequest Parse(string request)
        {
            var parts = request.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var querystring = Regex.Match(parts[0], "(?<=\\?)[^\\s]+").Value;
            var r = new UnicornRequest();
            foreach (var queryPart in querystring.Split('&'))
            {
                var keyValue = queryPart.Split('=');
                r.Query.Add(keyValue[0], keyValue[1]);
            }

            for (int i = 1; i < parts.Length; i++)
            {
                var header = parts[i].Split(':');
                var key = header[0].Trim();
                var value = header[1].Trim();

                r.Headers.Add(key, value);
            }

            return r;
        }
    }
}