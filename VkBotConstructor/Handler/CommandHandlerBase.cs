using System.Reflection;
using VkBotConstructor.Attribute;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Model;
using VkNet.Model.RequestParams;
using VkBotConstructor.Internal;
using VkBotConstructor.Abstractions.Enums;

namespace VkBotConstructor.Handler
{
    /// <summary>
    /// Базовый обработчик команд
    /// </summary>
    public abstract class CommandHandlerBase : BaseHandler
    {
        /// <summary>
        /// Инициализация обработчика команд
        /// </summary>
        /// <param name="apiManager">Менеджер API</param>
        /// <param name="provider">DI контейнер</param>
        public CommandHandlerBase(IVkApiManager apiManager, IServiceProvider provider) : base(apiManager, provider, HandlerType.Command) {}
    }
}
