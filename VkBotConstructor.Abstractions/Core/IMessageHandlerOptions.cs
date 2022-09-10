using VkBotConstructor.Abstractions.Event.Message;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция параметров обработки поступавшего сообщения
    /// </summary>
    public interface IMessageHandlerOptions : IHandlerOptions
    {
        /// <summary>
        /// Обработчик нажатия на кнопку "Начать"
        /// </summary>
        Action<IVkApiManager, IMessageInfo, IServiceProvider> OnStart { get; set; } 
    }
}
