using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class LegacyConfigurationsCommandHandler : BaseUnicornCommandHandler<LegacyConfigurationsCommand, IEnumerable<string>>
    {
        public LegacyConfigurationsCommandHandler() : base("Config")
        {
        }

        protected override async Task<IEnumerable<string>> ProcessResponse(HttpResponseMessage response, LegacyConfigurationsCommand context)
        {
            var configsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var configs = response.IsSuccessStatusCode
                ? configsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                : Enumerable.Empty<string>();
            return configs;
        }
    }
}