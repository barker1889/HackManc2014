using System;
using PusherServer;
using RestSharp;
using Server.InputProcessing;

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

            ListenForInput(new CommandHandler());

            CloseServer();
        }

        private static void ListenForInput(ICommandHandler handler)
        {
            var isRunning = true;

            while (isRunning)
            {
                var input = Console.ReadLine();

                var actionToTake = handler.HandleInput(input);

                switch (actionToTake.Action)
                {
                    case CommandAction.Quit:
                        isRunning = false;
                        break;

                    case CommandAction.SetScannerLocation:
                        _fakeLocationId = actionToTake.Data;
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

        static void sensor_TagReceived(object sender, Events.TagReceivedEvent e)
        {
            Console.WriteLine("Received: " + e.TagData);

            var data = new JsonObject {{"location_id", _fakeLocationId}};

            _pusher.Trigger(e.TagData, "location_change", data);
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CloseServer();
        }

        private static void CloseServer()
        {
            _sensor.Dispose();
        }
    }
}
