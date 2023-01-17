namespace VkBotConstructor.Exceptions
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string handlerName) : base("Handler not found. Handler name: " + handlerName) { }
    }
}
