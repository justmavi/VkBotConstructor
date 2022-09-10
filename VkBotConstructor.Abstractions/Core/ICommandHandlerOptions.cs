using System.Reflection;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция параметров обработчика команд
    /// </summary>
    public interface ICommandHandlerOptions : IHandlerOptions
    {
        /// <summary>
        /// Специальный ключ, с которого начинается команда. Например / или !
        /// </summary>
        public string CommandToken { get; set; }

        /// <summary>
        /// Разделитель аргументов
        /// </summary>
        public string ArgumentsSeparator { get; set; }
    }
}
