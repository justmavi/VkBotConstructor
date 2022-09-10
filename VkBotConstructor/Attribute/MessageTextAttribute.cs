namespace VkBotConstructor.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MessageTextAttribute : System.Attribute
    {
        public string[] Texts { get; }

        public MessageTextAttribute(string[] texts)
        {
            Texts = texts;
        }
    }
}
