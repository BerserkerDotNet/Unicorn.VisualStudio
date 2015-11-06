using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class IsLegacyCommand : BaseUnicornCommand<bool>
    {
        public IsLegacyCommand(UnicornConnection connection, CancellationToken token)
            : base(connection, token)
        {
        }

        public IsLegacyCommand(UnicornConnection connection) : base(connection)
        {
        }
    }
}