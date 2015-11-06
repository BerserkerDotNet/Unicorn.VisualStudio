using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Models;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class IsLegacyCommandHandler : BaseUnicornCommandHandler<IsLegacyCommand, bool>
    {
        protected override Task<bool> ProcessResponse(HttpResponseMessage response, IsLegacyCommand context)
        {
            return Task.FromResult(!response.IsSuccessStatusCode);
        }

        protected override string GetVerb(UnicornConnection connection)
        {
            return "Version";
        }
    }
}