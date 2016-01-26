namespace Unicorn.Emulator.Handlers
{
    internal class VersionHandler : IRequestHandler
    {
        public UnicornResponse Handle(UnicornRequest request)
        {
            return UnicornResponse.CreateOK("3.1.0.0");
        }
    }
}