namespace VkBotConstructor.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MessagePayloadAttribute : System.Attribute
    {
        public string[] Payloads { get; }

        public MessagePayloadAttribute(string[] payloads)
        {
            Payloads = payloads;
        }
    }
}
