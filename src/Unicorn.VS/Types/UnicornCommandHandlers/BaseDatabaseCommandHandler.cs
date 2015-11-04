using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn.VS.Helpers;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.UnicornCommandHandlers
{
    public abstract class BaseDatabaseCommandHandler : BaseUnicornCommandHandler<BaseDatabaseCommand, UnitType>
    {
        protected BaseDatabaseCommandHandler(string verb) 
            : base(verb, true)
        {
        }

        protected override async Task<UnitType> ProcessResponse(HttpResponseMessage response, BaseDatabaseCommand context)
        {
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