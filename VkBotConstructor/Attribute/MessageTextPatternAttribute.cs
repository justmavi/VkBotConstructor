namespace VkBotConstructor.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MessageTextPatternAttribute : System.Attribute
    {
        public string[] Patterns { get; }

        public MessageTextPatternAttribute(string[] patterns)
        {
            Patterns = patterns;
        }
    }
}
