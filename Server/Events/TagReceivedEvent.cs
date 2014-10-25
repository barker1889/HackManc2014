namespace Server.Events
{
    public class TagReceivedEvent
    {
        public TagReceivedEvent(string data)
        {
            TagData = data;
        }

        public string TagData { get; private set; }
    }
}
