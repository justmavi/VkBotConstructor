using System.Reflection;

namespace VkBotConstructor.Internal
{
    static internal class HandlersDictionary
    {
        private readonly static Dictionary<string, TypeInfo> handlerAssemblies = new();

        public static void Add(string key, TypeInfo assembly)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cannot be empty or null", nameof(key));
            }

            handlerAssemblies.Add(key, assembly);
        }

        public static TypeInfo Get(string key, bool ignoreCase = false)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;
            var value = handlerAssemblies.FirstOrDefault(x => string.Equals(x.Key, key, ignoreCase ? comparer : default)).Value;

            return value;
        }
    }
}
