using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PusherServer;
using Server.BusTimeApi;
using Server.BusTimeApi.Models;
using Server.Events;
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

        static void sensor_TagReceived(object sender, TagReceivedEvent e)
        {
            if (e.TagData == "0107ee6929")
            {

                Console.WriteLine("Bus arrived, generating message...");
                var busArrivedFilename = Guid.NewGuid() + ".wav";
                var busArrivedContent = "The bus that has just arrived is the number 3 to Salford keys";

                var busTimesPublisher = new AudioPublisher(new BlobPublisher(), new AudioStreamCreator());
                busTimesPublisher.GenerateFileAndPublish(busArrivedFilename, busArrivedContent);

                var completeFileName = AudioBaseUrl + busArrivedFilename;

                _pusher.Trigger("0107ee6b3d", "bus_arrived", completeFileName);

                Console.WriteLine("Done");

                return;
            }

            Console.WriteLine("Received: " + e.TagData);

            _pusher.Trigger(e.TagData, "request_received", "");

            Console.WriteLine("Fetching bus times...");
            var busStopSdk = new BusScheduleWrapper(BusStopId);
            var schedule = busStopSdk.GetBusTimes(DateTime.Now);
            Console.WriteLine("Done.");
            
            var quickPreferencesAudioFile = CreatePreferedRoutesAudio(e.TagData, schedule);
            var nextThreeBussesAudioFile = CreateNextThreeBuseseAudio(schedule);

            nextThreeBussesAudioFile = nextThreeBussesAudioFile.Insert(0, AudioBaseUrl);
            quickPreferencesAudioFile = quickPreferencesAudioFile.Insert(0, AudioBaseUrl);

            //var responseJson = new Newtonsoft.Json.();
            //    {
            //        {"next_departures", nextThreeBussesAudioFile},
            //        {"prefered_departures", quickPreferencesAudioFile}
            //    };

            var response = new[]
                {
                    nextThreeBussesAudioFile,
                    quickPreferencesAudioFile
                };

            _pusher.Trigger(e.TagData, "audio_updated", response);
        }

        private static string CreatePreferedRoutesAudio(string tagId, BusStopSchedule schedule)
        {
            var quickPrefAudioFile = Guid.NewGuid() + ".wav";

            var userPreferencesService = new UserPreferenceService();
            var usersRegularRoutes = userPreferencesService.GetBusRoutesPreferencesForTag(tagId);
            var preferenceMessageContent = new StringBuilder();

            if (usersRegularRoutes.Any())
            {
                foreach (var userPreference in usersRegularRoutes)
                {
                    var nextDepartureForRegularRoute =
                        schedule.departures.all.FirstOrDefault(
                            dept => dept.line == userPreference.RouteNumber && dept.direction == userPreference.Direction);
                    
                    if (nextDepartureForRegularRoute != null)
                    {
                        var departureDate = DateTime.Parse(nextDepartureForRegularRoute.aimed_departure_time);
                        preferenceMessageContent.Append("The number " + nextDepartureForRegularRoute.line + " departs at " +
                                                        departureDate.ToString("HH mm") + ", ");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(preferenceMessageContent.ToString()))
            {
                var completeMessage = "Your prefered buses departing from this stop are " + preferenceMessageContent + " Triple tap to repeat.";

                Console.WriteLine("Pushing prefered routes to azure...");
                var busTimesPublisher = new AudioPublisher(new BlobPublisher(), new AudioStreamCreator());
                busTimesPublisher.GenerateFileAndPublish(quickPrefAudioFile, completeMessage);
                Console.WriteLine("Done.");

                return quickPrefAudioFile;
            }

            return "";
        }

        private static string CreateNextThreeBuseseAudio(BusStopSchedule schedule)
        {
            var processor = new BusTimeProcessor(schedule);

            var nextThreeBussesAudioFile = Guid.NewGuid() + ".wav";

            var nextBusses = processor.GetNextThreeBuses();

            var messageGenerator = new MessageGenerator();
            var messageContent = messageGenerator.GenerateNextDeparturesMessage(BusStopName, nextBusses);

            Console.WriteLine("Pushing to azure...");
            var busTimesPublisher = new AudioPublisher(new BlobPublisher(), new AudioStreamCreator());
            busTimesPublisher.GenerateFileAndPublish(nextThreeBussesAudioFile, messageContent);
            Console.WriteLine("Done.");

            return nextThreeBussesAudioFile;
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
