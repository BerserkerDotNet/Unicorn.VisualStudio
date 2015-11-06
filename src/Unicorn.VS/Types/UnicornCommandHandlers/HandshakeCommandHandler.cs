using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Models;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class HandshakeCommandHandler : BaseUnicornCommandHandler<HandshakeCommand, bool>
    {
        protected override Task<bool> ProcessResponse(HttpResponseMessage response, HandshakeCommand context)
        {
            return Task.FromResult(response.IsSuccessStatusCode);
        }

        protected override string GetVerb(UnicornConnection connection)
        {
            return connection.IsLegacy ? "Handshake" : "Version";
        }
    }
}