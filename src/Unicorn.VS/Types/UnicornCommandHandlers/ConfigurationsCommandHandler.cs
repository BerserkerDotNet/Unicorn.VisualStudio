using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Models;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class ConfigurationsCommandHandler : BaseUnicornCommandHandler<ConfigurationsCommand, IEnumerable<string>>
    {

        protected override async Task<IEnumerable<string>> ProcessResponse(HttpResponseMessage response, ConfigurationsCommand context)
        {
            var configsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var configs = response.IsSuccessStatusCode
                ? configsString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                : Enumerable.Empty<string>();
            return configs;
        }

        protected override string GetVerb(UnicornConnection connection)
        {
            return connection.IsLegacy ? "Config" : "Configurations";
        }
    }

    

}