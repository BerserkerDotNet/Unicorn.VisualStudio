using System.Collections.Generic;
using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class LegacyConfigurationsCommand : BaseUnicornCommand<IEnumerable<string>>
    {
        public LegacyConfigurationsCommand(UnicornConnection connection, CancellationToken token)
            : base(connection, token)
        {
        }

        public LegacyConfigurationsCommand(UnicornConnection connection) : base(connection)
        {
        }
    }
}