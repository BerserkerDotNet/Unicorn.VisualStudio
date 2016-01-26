using System;
using System.Text;

namespace Unicorn.Emulator.Handlers
{
    internal class SyncHandler : IRequestHandler
    {
        private Random _random = new Random(0);
        private string[] _opTypes = new[] {"[D]", "[S]", "[A]", "[M]", "[U]", "[R]", "[T]"};
        private string[] _phrases = new[] {"was", "deleted", "moved", "updated", "recycle", "renamed", "cannot", "find", "error", "template", "has", "new", "have", "run", "good", "bad", "default", "value", "set", "changed"};
        public UnicornResponse Handle(UnicornRequest request)
        {
            var response = UnicornResponse.CreateChunked();
            var config = request["configuration"];
            response.Body.Add($"0|Info|{Encode($"{config} is being sync")}{Environment.NewLine}");
            for (int i = 0; i < 100; i++)
            {
                response.Body.Add($"0|Info|{Encode($"{GetOperationType()} Item {i} {GetText()}")}{Environment.NewLine}1|Info|{Encode((i).ToString())}{Environment.NewLine}");
            }
            response.Body.Add($"0|Info|{Encode($"{config} is synced")}{Environment.NewLine}1|Info|{Encode((100).ToString())}{Environment.NewLine}");
            return response;
        }

        private string GetText()
        {
            return $"{_phrases[_random.Next(0, _phrases.Length - 1)]} {_phrases[_random.Next(0, _phrases.Length - 1)]} {_phrases[_random.Next(0, _phrases.Length - 1)]} {_phrases[_random.Next(0, _phrases.Length - 1)]}";
        }

        private string GetOperationType()
        {
            return _opTypes[_random.Next(0, 6)];
        }

        private string Encode(string msg)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
        }
    }
}