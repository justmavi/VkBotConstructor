namespace VkBotConstructor.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandNameAttribute : System.Attribute
    {
        public string[] Names { get; }
        public CommandNameAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
