using System;

namespace Server
{
    class Program
    {
        private static RfidSensor _sensor;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.WriteLine("Server starting...");
            
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
        }

        private static void CloseServer()
        {
            _sensor.Dispose();
        }
    }
}
