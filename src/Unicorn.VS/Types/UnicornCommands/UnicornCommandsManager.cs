using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unicorn.VS.Types.Exceptions;
using Unicorn.VS.Types.Interfaces;
using Unicorn.VS.Types.UnicornCommandHandlers;

namespace Unicorn.VS.Types.UnicornCommands
{
    public static class UnicornCommandsManager
    {
        static UnicornCommandsManager()
        {
            Register<ConfigurationsCommand>(new ConfigurationsCommandHandler());
            Register<LegacySynchronizeCommand>(new LegacySynchronizeCommandHandler());
            Register<LegacyReserializeCommand>(new LegacyReserializeCommandHandler());
            Register<HandshakeCommand>(new HandshakeCommandHandler());
            Register<IsLegacyCommand>(new IsLegacyCommandHandler());
        }

        private static Dictionary<Type, object> _commandHandlers = new Dictionary<Type, object>(5);

        public static async Task<TResult> Execute<TResult>(IUnicornCommand<TResult> command)
        {
            var type = command.GetType();
            if (!_commandHandlers.ContainsKey(type))
                throw new CommandHandlerNotFoundException(type.Name);

            var commandHandler = _commandHandlers[type];
            var method = GetProcessMethod(commandHandler.GetType(), command.GetType(), "Execute");
            return await ((Task<TResult>)method.Invoke(commandHandler, new[] { command })).ConfigureAwait(false);
        }

        public static void Register<TCommand>(object instance)
        {
            var type = typeof (TCommand);
            if (_commandHandlers.ContainsKey(type))
                return;
            _commandHandlers[type] = instance;
        }

        private static MethodInfo GetProcessMethod(Type handlerType, Type messageType, string methodName)
        {
            return handlerType
                    .GetMethod(methodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, CallingConventions.HasThis,
                        new[] { messageType },
                        null);
        }

    }
}