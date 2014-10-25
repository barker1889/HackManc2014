using System;
using Phidgets;
using Server.Events;

namespace Server
{
    public class RfidSensor : IDisposable
    {
        public event EventHandler<TagReceivedEvent> TagReceived;

        private readonly RFID _scanner;

        public RfidSensor()
        {
            _scanner = new RFID();

            _scanner.Tag += _scanner_Tag;

            _scanner.open();
            _scanner.waitForAttachment(2000);

            _scanner.Antenna = true;
            _scanner.LED = true;
        }

        void _scanner_Tag(object sender, Phidgets.Events.TagEventArgs e)
        {
            var tagData = e.Tag;
            TagReceived(this, new TagReceivedEvent(tagData));
        }

        public void Dispose()
        {
            _scanner.Antenna = false;
            _scanner.LED = false;
        }
    }
}
