using System.Reflection;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Event.Message;

namespace VkBotConstructor.Core
{
    /// <summary>
    /// Параметры обработчика поступавшего сообщения
    /// </summary>
    public class MessageHandlerOptions : IMessageHandlerOptions
    {
        /// <inheritdoc />
        public string CommandToken { get; set; }

        /// <inheritdoc />
        public bool ResponseToUnknownCommand { get; set; }

        /// <inheritdoc />
        public string UnknownCommandResponseMessage { get; set; } = "Я не понимаю Вас";

        /// <inheritdoc />
        public bool IgnoreCase { get; set; }

        /// <inheritdoc />
        public IEnumerable<Assembly> Assemblies { get; set; }

        /// <inheritdoc />
        public Action<IVkApiManager, IMessageInfo, IServiceProvider> OnStart { get; set; }
    }
}
