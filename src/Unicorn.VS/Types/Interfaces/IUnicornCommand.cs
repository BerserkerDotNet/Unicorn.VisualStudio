using System.Threading;
using Unicorn.VS.Models;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.Interfaces
{
    public interface IUnicornCommand : IUnicornCommand<UnitType>
    {

    }

    public interface IUnicornCommand<TReturn>
    {
        string SelectedConfigurations { get;}
        UnicornConnection Connection { get;}
        CancellationToken CancellationToken { get; }
    }
}