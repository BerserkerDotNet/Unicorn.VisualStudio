using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class HandshakeCommandHandler : BaseUnicornCommandHandler<HandshakeCommand, bool>
    {
        public HandshakeCommandHandler()
            : base("Handshake")
        {

        }

        protected override Task<bool> ProcessResponse(HttpResponseMessage response, HandshakeCommand context)
        {
            return Task.FromResult(response.IsSuccessStatusCode);
        }
    }
}