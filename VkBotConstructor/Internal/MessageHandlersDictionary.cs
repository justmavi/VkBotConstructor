using System.Reflection;
using System.Text.RegularExpressions;

namespace VkBotConstructor.Internal
{
    static internal class MessageHandlersDictionary
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
            var comparer = ignoreCase ? StringComparison.OrdinalIgnoreCase : default;
            var value = handlerAssemblies.FirstOrDefault(x => string.Equals(x.Key, key, comparer)).Value;

            return value;
        }

        public static TypeInfo Get(Regex regex)
        {
            var value = handlerAssemblies.FirstOrDefault(x => regex.IsMatch(x.Key)).Value;

            return value;
        }
    }
}
