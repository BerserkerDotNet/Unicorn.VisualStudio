using System;
using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public abstract class BaseReportableCommand : BaseUnicornCommand<UnitType>
    {
        protected BaseReportableCommand(UnicornConnection connection, string selectedConfigs, CancellationToken token, Action<StatusReport> reportHandler) 
            : base(connection, selectedConfigs, token)
        {
            ReportHandler = reportHandler;
        }

        public Action<StatusReport> ReportHandler { get; }
    }
}