using System.Reflection;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция параметров обработчика команд
    /// </summary>
    public interface ICommandHandlerOptions
    {
        /// <summary>
        /// Специальный ключ, с которого начинается команда. Например / или !
        /// </summary>
        public string CommandToken { get; set; }

        /// <summary>
        /// Реаигровать ли боту на неизвестную команду
        /// </summary>
        public bool ResponseToUnknownCommand { get; set; }

        /// <summary>
        /// Сообщение, которое должно отправиться при вводе неизвестной команды
        /// </summary>
        public string UnknownCommandResponseMessage { get; set; }

        /// <summary>
        /// Разделитель аргументов
        /// </summary>
        public string ArgumentsSeparator { get; set; }

        /// <summary>
        /// Игнорировать регистр.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Сборки, где находятся определения обработчиков команд
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; set; }
    }
}
