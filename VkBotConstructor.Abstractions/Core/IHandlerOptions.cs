using System.Reflection;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция обработчиков сообщений
    /// </summary>
    public interface IHandlerOptions
    {
        /// <summary>
        /// Игнорировать регистр.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Сборки, где находятся определения обработчиков команд
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; set; }

        /// <summary>
        /// Реаигровать ли боту на неизвестную команду
        /// </summary>
        public bool ResponseToUnknownCommand { get; set; }

        /// <summary>
        /// Сообщение, которое должно отправиться при вводе неизвестной команды
        /// </summary>
        public string UnknownCommandResponseMessage { get; set; }
    }
}
