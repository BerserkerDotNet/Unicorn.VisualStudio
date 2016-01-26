using System;

namespace Unicorn.Emulator.Handlers
{
    internal class ChallengeHandler : IRequestHandler
    {
        public UnicornResponse Handle(UnicornRequest request)
        {
            return UnicornResponse.CreateOK(Guid.NewGuid().ToString());
        }
    }
}