using System;
using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class LegacyReserializeCommand : BaseDatabaseCommand
    {
        public LegacyReserializeCommand(UnicornConnection connection, string selectedConfigs, CancellationToken token, Action<StatusReport> reportHandler) 
            : base(connection, selectedConfigs, token, reportHandler)
        {
        }
    }
}