using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Unicorn.VS.Helpers
{
    public static class VersionHelper
    {
        public static Version SupportedClientVersion = new Version(1, 1, 0, 0);
        public static Version SupportedUnicornVersion = new Version(3, 0, 0, 0);

        public static bool IsUpdateRequired(this HttpResponseMessage response)
        {
            IEnumerable<string> values;
            return !response.Headers.TryGetValues("X-Remote-Version", out values) || values.Any(v => !IsUpToDate(v));

        }

        public static bool IsSupportedUnicornVersion(string version)
        {
            Version current;
            return Version.TryParse(version, out current) && current >= SupportedUnicornVersion;
        }

        private static bool IsUpToDate(string currentVersion)
        {
            Version current;
            return Version.TryParse(currentVersion, out current) &&
                  (current == SupportedClientVersion || current >= SupportedUnicornVersion);
        }
    }
}