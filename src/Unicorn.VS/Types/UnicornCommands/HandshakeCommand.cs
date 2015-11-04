using System.Threading;
using Unicorn.VS.Models;

namespace Unicorn.VS.Types.UnicornCommands
{
    public class HandshakeCommand : BaseUnicornCommand<bool>
    {
        public HandshakeCommand(UnicornConnection connection, CancellationToken token) 
            : base(connection, token)
        {
        }
    }
}