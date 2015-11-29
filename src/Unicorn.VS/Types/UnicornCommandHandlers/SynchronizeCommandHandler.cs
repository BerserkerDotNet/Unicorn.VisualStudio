using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class SynchronizeCommandHandler : BaseReportableCommandHandler
    {
        protected override string GetVerb(UnicornConnection connection)
        {
            return connection.IsLegacy ? "Sync" : "VSSync";
        }
    }
}