using System.Reflection;
using VkBotConstructor.Abstractions.Core;

namespace VkBotConstructor.Core
{
    /// <summary>
    /// Параметры обработчика команд
    /// </summary>
    public class CommandHandlerOptions : ICommandHandlerOptions
    {
        /// <inheritdoc />
        public string CommandToken { get; set; }

        /// <inheritdoc />
        public bool ResponseToUnknownCommand { get; set; }

        /// <inheritdoc />
        public string UnknownCommandResponseMessage { get; set; } = "Команда не найдена";

        /// <summary>
        /// Разделитель аргументов. По умолчанию - пробел.
        /// </summary>
        public string ArgumentsSeparator { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool IgnoreCase { get; set; }

        /// <inheritdoc />
        public IEnumerable<Assembly> Assemblies { get; set; }
    }
}
