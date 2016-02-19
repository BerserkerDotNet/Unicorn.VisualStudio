using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unicorn.Emulator.Handlers;

namespace Unicorn.Emulator
{
    class Program
    {
        static Random _random = new Random(0);
        private static Dictionary<string, IRequestHandler> _handlers = new Dictionary<string, IRequestHandler>();
        private static bool _isAdminMode = false;

        static void Main(string[] args)
        {
            _isAdminMode = true;
            InitializeHandlers();
            StartServer();

            Console.ReadLine();
        }

        private static void InitializeHandlers()
        {
            _handlers.Add("challenge", new ChallengeHandler());
            _handlers.Add("version", new VersionHandler());
            _handlers.Add("vssync", new SyncHandler());
            _handlers.Add("vsreserialize", new ReSerializeHandler());
            _handlers.Add("configurationhealth", new ConfigurationHealthHandler());
            _handlers.Add("configurations", new ConfigurationsHandler());
        }

        private static async void StartServer()
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 2222));
            listener.Start();
            Console.WriteLine("Server is running on port 2222");
            while (true)
            {
                var connection = await listener.AcceptTcpClientAsync();
                int t = 0;
                while (t < 3)
                {
                    if (connection.Available != 0)
                        break;

                    Thread.Sleep(100);
                    t++;
                }
                var stream = connection.GetStream();
                var buffer = new byte[connection.Available];
                await stream.ReadAsync(buffer, 0, connection.Available);

                var request = Encoding.UTF8.GetString(buffer);
                var response = Execute(UnicornRequest.Parse(request));

                do
                {
                    var renderResponse = response.RenderResponse();
                    var rData = Encoding.UTF8.GetBytes(renderResponse);
                    await stream.WriteAsync(rData, 0, rData.Length);
                    await stream.FlushAsync();
                    Thread.Sleep(_random.Next(100, 500));
                } while (!response.IsComplete);
                connection.Close();
                connection.Dispose();

            }
        }

        private static UnicornResponse Execute(UnicornRequest request)
        {
            if(!_isAdminMode && !request.IsAuthenticated)
                return UnicornResponse.CreateUnauthorized();

            if(!_handlers.ContainsKey(request.Verb))
                return UnicornResponse.CreateNotFound();

            return _handlers[request.Verb].Handle(request);
        }
    }
}
