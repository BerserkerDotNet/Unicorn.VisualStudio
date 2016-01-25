using System;
using System.Text;

namespace Unicorn.Emulator.Handlers
{
    internal class ConfigurationHealthHandler : IRequestHandler
    {
        public UnicornResponse Handle(UnicornRequest request)
        {
            var configuration = request["configuration"];
            if (string.IsNullOrEmpty(configuration))
                configuration = "Test1^Test2^Test3";
            var sb = new StringBuilder();
            foreach (var config in configuration.Split('^'))
            {
                if (config.Equals("Test1", StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine($"{config}|true|true|true|Test2,Test3");
                else if (config.Equals("Test2", StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine($"{config}|false|false|false|");
                else if (config.Equals("Test3", StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine($"{config}|false|true|false|Test2");
            }

            return UnicornResponse.CreateOK(sb.ToString());
        }
    }
}