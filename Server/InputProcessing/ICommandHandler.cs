namespace Server.InputProcessing
{
    public interface ICommandHandler
    {
        Command HandleInput(string input);
    }
}