using System;

namespace Unicorn.VS.Types.Exceptions
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(string name)
            :base($"Handler not found for command {name}")
        {
            
        }
    }
}