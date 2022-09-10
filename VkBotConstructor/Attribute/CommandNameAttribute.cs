namespace VkBotConstructor.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandNameAttribute : System.Attribute
    {
        public string[] Names { get; }
        public CommandNameAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
