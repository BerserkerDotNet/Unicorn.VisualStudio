namespace Unicorn.Emulator.Handlers
{
    internal class ConfigurationsHandler : IRequestHandler
    {
        public UnicornResponse Handle(UnicornRequest request)
        {
            return UnicornResponse.CreateOK("Test1,Test2,Test3");
        }
    }
}