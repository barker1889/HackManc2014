using System;
using PusherServer;
using Server.BusTimeApi;
using Server.InputProcessing;
using Server.Services;

namespace Server
{
    class Program
    {
        private const string BusStopId = "1800SB09201";
        private const string BusStopName = "Lower Byrom Street/Science Museum";
        private const string AudioBaseUrl = "https://busbuddyjb.blob.core.windows.net/buses/";

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

            _pusher.Trigger(e.TagData, "request_received", "");

            Console.WriteLine("Fetching bus times...");
            var busStopSdk = new BusScheduleWrapper(BusStopId);
            var schedule = busStopSdk.GetBusTimes(DateTime.Now);
            Console.WriteLine("Done.");

            var processor = new BusTimeProcessor(schedule);
            var nextBusses = processor.GetNextThreeBuses();

            var messageGenerator = new MessageGenerator();
            var messageContent = messageGenerator.GenerateNextDeparturesMessage(BusStopName, nextBusses);

            var fileName = Guid.NewGuid() + ".wav";

            Console.WriteLine("Pushing to azure...");
            var busTimesPublisher = new AudioPublisher(new BlobPublisher(), new AudioStreamCreator());
            busTimesPublisher.GenerateFileAndPublish(fileName, messageContent);
            Console.WriteLine("Done.");

            var fullFilePath = AudioBaseUrl + fileName;

            _pusher.Trigger(e.TagData, "audio_updated", fullFilePath);
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
