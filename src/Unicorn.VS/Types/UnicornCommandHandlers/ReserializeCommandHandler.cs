using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class ReserializeCommandHandler : BaseReportableCommandHandler
    {
        protected override string GetVerb(UnicornConnection connection)
        {
            return connection.IsLegacy ? "Reserialize" : "VSReserialize";
        }
    }
}