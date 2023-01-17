using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Enums;

namespace VkBotConstructor.Handler
{
    /// <summary>
    /// Базовый обработчик команд
    /// </summary>
    public abstract class MessageHandlerBase : BaseHandler
    {
        /// <summary>
        /// Инициализация обработчика сообщений
        /// </summary>
        /// <param name="apiManager">Менеджер API</param>
        /// <param name="provider">DI контейнер</param>
        public MessageHandlerBase(IVkApiManager apiManager, IServiceProvider provider) : base(apiManager, provider, HandlerType.Message) {}
    }
}
