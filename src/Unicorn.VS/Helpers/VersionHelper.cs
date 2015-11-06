using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Unicorn.VS.Helpers
{
    public static class VersionHelper
    {
        public static Version SupportedClientVersion = new Version(1, 1, 0, 0);
        public static Version IntegratedUnicornVersion = new Version(3, 0, 0, 0);
        public static Version CompatibleUnicornVersion = new Version(3, 1, 0, 0);

        public static bool IsSupportedUnicornVersion(string version)
        {
            Version current;
            return Version.TryParse(version, out current) && current >= IntegratedUnicornVersion;
        }

        public static bool IsLegacy(Version version)
        {
            return version < CompatibleUnicornVersion;
        }
    }
}