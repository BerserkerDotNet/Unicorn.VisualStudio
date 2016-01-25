using System.Collections.Generic;
using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class CheckConfigurationHealthCommand : BaseUnicornCommand<IEnumerable<StatusReport>>
    {
        public CheckConfigurationHealthCommand(UnicornConnection connection, string configurations, CancellationToken token)
            : base(connection, configurations, token)
        {
        }

        public CheckConfigurationHealthCommand(UnicornConnection connection, string configurations) : base(connection, configurations)
        {
        }
    }
}