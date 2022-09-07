using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Event.Message;

namespace VkBotConstructor.Abstractions.Handler
{
    /// <summary>
    /// Абстракция обработчика команд
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Информация о поступавшей сообщении
        /// </summary>
        IMessageInfo MessageInfo { get; set; }
        /// <summary>
        /// Обработчик поступавшей команды
        /// </summary>
        /// <param name="cmdtext">Команда</param>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="options">Параметры обработчика команд</param>
        /// <returns></returns>
        Task HandleAsync(string cmdtext, string[] arguments, ICommandHandlerOptions options);
    }
}
