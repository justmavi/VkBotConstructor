namespace VkBotConstructor.Exceptions
{
    public class ManagerIsUninitializedException : Exception
    {
        public ManagerIsUninitializedException(string apiName) : base($"Manager is unintialized. API: ${apiName}") { }
    }
}
