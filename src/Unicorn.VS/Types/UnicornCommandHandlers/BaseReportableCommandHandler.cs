using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Helpers;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public abstract class BaseReportableCommandHandler : BaseUnicornCommandHandler<BaseReportableCommand, UnitType>
    {
        protected BaseReportableCommandHandler() 
            : base(true)
        {
        }

        protected override async Task<UnitType> ProcessResponse(HttpResponseMessage response, BaseReportableCommand context)
        {
            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var sr = new StreamReader(stream))
                {
                    while (!sr.EndOfStream)
                    {
                        if (context.CancellationToken.IsCancellationRequested)
                            break;

                        var message = await sr.ReadLineAsync()
                            .ConfigureAwait(false);
                        var statusReport = message.ToReport();
                        context.ReportHandler(statusReport);
                    }
                }
            }

            return await Task.FromResult(UnitType.Value);
        }
    }
}