namespace Server.InputProcessing
{
    public class Command
    {
        public Command(CommandAction action)
        {
            Action = action;
        }

        public Command(CommandAction action, string data)
        {
            Action = action;
            Data = data;
        }

        public CommandAction Action { get; set; }
        public string Data { get; set; }
    }
}