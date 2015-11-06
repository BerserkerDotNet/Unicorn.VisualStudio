using System.Collections.Generic;
using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class ConfigurationsCommand : BaseUnicornCommand<IEnumerable<string>>
    {
        public ConfigurationsCommand(UnicornConnection connection, CancellationToken token)
            : base(connection, token)
        {
        }

        public ConfigurationsCommand(UnicornConnection connection) : base(connection)
        {
        }
    }
}