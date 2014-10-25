namespace Server.InputProcessing
{
    public class CommandHandler : ICommandHandler
    {
        public Command HandleInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new Command(CommandAction.None);

            var commandParts = input.Split(' ');
            var command = commandParts[0];

            switch (command.ToUpper())
            {
                case "SET_LOCATION":
                    var locationId = commandParts[1];
                    return new Command(CommandAction.SetScannerLocation, locationId);
                case "QUIT":
                    return new Command(CommandAction.Quit);
            }

            return new Command(CommandAction.None);
        }
    }
}