using System;
using System.Text;

namespace Unicorn.Emulator.Handlers
{
    internal class SyncHandler : IRequestHandler
    {
        public UnicornResponse Handle(UnicornRequest request)
        {
            var response = UnicornResponse.CreateChunked();
            var config = request["configuration"];
            response.Body.Add($"0|Info|{Encode($"{config} is being sync")}{Environment.NewLine}");
            for (int i = 0; i < 10; i++)
            {
                response.Body.Add($"0|Info|{Encode($"[D] Dummy template {i} was deleted.")}{Environment.NewLine}1|Info|{Encode((i * 10).ToString())}{Environment.NewLine}");
            }
            response.Body.Add($"0|Info|{Encode($"{config} is synced")}{Environment.NewLine}1|Info|{Encode((100).ToString())}{Environment.NewLine}");
            return response;
        }

        private string Encode(string msg)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
        }
    }
}