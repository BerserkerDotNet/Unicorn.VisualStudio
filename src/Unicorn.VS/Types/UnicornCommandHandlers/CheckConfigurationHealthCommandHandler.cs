using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Models;
using Unicorn.VS.Types.Logging;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public class CheckConfigurationHealthCommandHandler : BaseUnicornCommandHandler<CheckConfigurationHealthCommand, IEnumerable<StatusReport>>
    {
        protected override async Task<IEnumerable<StatusReport>> ProcessResponse(HttpResponseMessage response, CheckConfigurationHealthCommand context)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return Enumerable.Empty<StatusReport>();

                if (!response.IsSuccessStatusCode)
                    return new[] { StatusReport.CreateOperation("Configuration status cannot be read. Most likely your version of Unicorn does not support it. You can turn it off in settings to remove this message.", MessageLevel.Warning, OperationType.None) };

                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var reports = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                return reports.SelectMany(GetStatusReport);
            }
            catch (Exception ex)
            {
                return new[] {StatusReport.CreateOperation($"Error parsing response from server {ex.Message}", MessageLevel.Error, OperationType.None)};
            }
        }

        private IEnumerable<StatusReport> GetStatusReport(string report)
        {
            var parts = report.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() < 4)
                yield return StatusReport.CreateOperation("Error parsing configuration report", MessageLevel.Error, OperationType.None);

            var name = parts[0];
            var hasAnySerializedItems = bool.Parse(parts[1]);
            var hasValidRootPaths = bool.Parse(parts[2]);
            var transparentSyncEnabled = bool.Parse(parts[3]);
            var dependents = parts.Count() == 5 ? parts[4] : string.Empty;

            if (!string.IsNullOrEmpty(dependents))
                yield return StatusReport.CreateOperation($"{name} depends on {dependents}, which should sync before it.", MessageLevel.Info, OperationType.None);

            if (!hasValidRootPaths && !hasAnySerializedItems)
                yield return StatusReport.CreateOperation($"{name} configuration's predicate cannot resolve any valid root items. This usually means the predicate is configured to include paths that do not exist in the Sitecore database.",
                    MessageLevel.Warning, OperationType.None);
            else if (!hasAnySerializedItems)
                yield return StatusReport.CreateOperation($"{name} does not currently have any valid serialized items. You cannot sync it until you perform an initial serialization, which will write the current state of Sitecore to serialized items.", MessageLevel.Warning, OperationType.None);

            if (transparentSyncEnabled)
                yield return StatusReport.CreateOperation($"Transparent sync is enabled for {name}.", MessageLevel.Info, OperationType.None);
        }

        protected override string GetVerb(UnicornConnection connection)
        {
            return "ConfigurationHealth";
        }
    }
}