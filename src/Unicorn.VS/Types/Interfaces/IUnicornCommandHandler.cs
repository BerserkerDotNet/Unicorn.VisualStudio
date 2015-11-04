using System.Threading.Tasks;
using Unicorn.VS.Types.UnicornCommands;

namespace Unicorn.VS.Types.Interfaces
{
    public interface IUnicornCommandHandler<TCommand>:IUnicornCommandHandler<TCommand, UnitType> 
        where TCommand : IUnicornCommand<UnitType>
    {

    }

    public interface IUnicornCommandHandler<TCommand, TResult>
        where TCommand:IUnicornCommand<TResult>
    {
        Task<TResult> Execute(TCommand ctx);
    }
}