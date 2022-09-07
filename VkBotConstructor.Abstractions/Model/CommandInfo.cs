namespace VkBotConstructor.Abstractions.Model
{
    public class CommandInfo
    {
        public string CommandText { get; set; }
        public string[] Arguments { get; set; }

        public CommandInfo(string commandText, string[] arguments)
        {
            CommandText = commandText;
            Arguments = arguments;
        }
    }
}
