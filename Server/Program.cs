using System;
using PusherServer;
using RestSharp;

namespace Server
{
    class Program
    {
        private static RfidSensor _sensor;
        private static Pusher _pusher;
        private static string _fakeLocationId;

        static void Main(string[] args)
        {
            _fakeLocationId = "1";

            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine("Server starting...");

            Initialise();

            Console.WriteLine("Server ready.");

            ListenForInput();

            CloseServer();
        }

        private static void ListenForInput()
        {
            var isRunning = true;

            while (isRunning)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var commandParts = input.Split(' ');
                var command = commandParts[0];

                switch (command.ToUpper())
                {
                    case "LOCATION":
                        _fakeLocationId = commandParts[1];
                        break;
                    case "QUIT":
                        isRunning = false;
                        break;
                }
            }
        }

        private static void Initialise()
        {
            _pusher = new Pusher("94187", "2d681985720e46e6f974", "8ab83d4148b4809dff09");

            _sensor = new RfidSensor();
            _sensor.TagReceived += sensor_TagReceived;
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CloseServer();
        }

        static void sensor_TagReceived(object sender, Events.TagReceivedEvent e)
        {
            Console.WriteLine("Received: " + e.TagData);

            var data = new JsonObject {{"location_id", _fakeLocationId}};

            _pusher.Trigger("110057f2cd", "location_changed", data);
        }

        private static void CloseServer()
        {
            _sensor.Dispose();
        }
    }
}
