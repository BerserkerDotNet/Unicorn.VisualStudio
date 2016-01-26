namespace Unicorn.Emulator.Handlers
{
    internal interface IRequestHandler
    {
        UnicornResponse Handle(UnicornRequest request);
    }
}