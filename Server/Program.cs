using System;
using PusherServer;

namespace Server
{
    class Program
    {
        private static RfidSensor _sensor;
        private static Pusher _pusher;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.WriteLine("Server starting...");

            _pusher = new Pusher("94187", "2d681985720e46e6f974", "8ab83d4148b4809dff09");

            _sensor = new RfidSensor();
            _sensor.TagReceived += sensor_TagReceived;
            
            Console.WriteLine("Server ready.");
            Console.ReadLine();

            CloseServer();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CloseServer();
        }

        static void sensor_TagReceived(object sender, Events.TagReceivedEvent e)
        {
            Console.WriteLine("Received: " + e.TagData);

            _pusher.Trigger("test_channel", "TagDetected", DateTime.Now.ToString());
        }

        private static void CloseServer()
        {
            _sensor.Dispose();
        }
    }
}
