using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class LegacySynchronizeCommandHandler : BaseReportableCommandHandler
    {
        protected override string GetVerb(UnicornConnection connection)
        {
            return connection.IsLegacy ? "Sync" : "VSSync";
        }
    }
}