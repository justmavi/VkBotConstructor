namespace VkBotConstructor.Exceptions
{
    public class InvalidRouteException : Exception
    {
        public InvalidRouteException() : base("Http route is invalid") { }
    }
}
