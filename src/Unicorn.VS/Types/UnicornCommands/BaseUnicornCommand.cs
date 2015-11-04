using System.Threading;
using Unicorn.VS.Helpers;
using Unicorn.VS.Models;
using Unicorn.VS.Types.Interfaces;

namespace Unicorn.VS.Types.UnicornCommands
{
    public abstract class BaseUnicornCommand<TReturn> : IUnicornCommand<TReturn>
    {
        protected BaseUnicornCommand(UnicornConnection connection, string selectedConfigs, CancellationToken token)
        {
            Connection = connection;
            SelectedConfigurations = selectedConfigs;
            CancellationToken = token;
        }

        protected BaseUnicornCommand(UnicornConnection connection, string selectedConfigs)
            : this(connection, selectedConfigs, CancellationToken.None)
        {

        }

        protected BaseUnicornCommand(UnicornConnection connection, CancellationToken token)
            : this(connection, string.Empty, token)
        {

        }

        protected BaseUnicornCommand(UnicornConnection connection)
            : this(connection, string.Empty, CancellationToken.None)
        {

        }

        public string SelectedConfigurations { get; }
        public UnicornConnection Connection { get;  }
        public CancellationToken CancellationToken { get; }
    }

    public abstract class BaseUnicornCommand : BaseUnicornCommand<UnitType>
    {
        protected BaseUnicornCommand(UnicornConnection connection, string selectedConfigs, CancellationToken token) : base(connection, selectedConfigs, token)
        {
        }

        protected BaseUnicornCommand(UnicornConnection connection, string selectedConfigs) : base(connection, selectedConfigs)
        {
        }

        protected BaseUnicornCommand(UnicornConnection connection, CancellationToken token) : base(connection, token)
        {
        }

        protected BaseUnicornCommand(UnicornConnection connection) : base(connection)
        {
        }
    }
}
