using System;
using System.Globalization;
using PusherServer;
using Server.BusTimeApi;
using Server.InputProcessing;

namespace Server
{
    class Program
    {
        private const string BusStopId = "1800SB09201";
        private static RfidSensor _sensor;
        private static Pusher _pusher;

        static void Main(string[] args)
        {
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

            // TODO: Immediate eedback since the request might take a while
            // _pusher.Trigger(e.TagData, "request_received", DateTime.Now.ToString(CultureInfo.InvariantCulture));

            Console.WriteLine("Fetching bus times...");
            var schedule = new BusScheduleWrapper(BusStopId);
            schedule.GetBusTimes(DateTime.Now);
            Console.WriteLine("Done.");

            _pusher.Trigger(e.TagData, "location_change", "TBC");
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
