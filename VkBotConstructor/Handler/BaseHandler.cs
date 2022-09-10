using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Event.Message;
using VkBotConstructor.Abstractions.Model;

namespace VkBotConstructor.Handler
{
    public abstract class BaseHandler
    {
        /// <summary>
        /// Информация о поступавшей сообщении
        /// </summary>
        public IMessageInfo MessageInfo { get; set; }

        /// <summary>
        /// Ответ обработчика по умолчанию на случай, если аргументы не соответствуют критериям
        /// </summary>
        protected virtual string DefaultAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Результат валидации модели команды
        /// </summary>
        protected ArgumentsValidationResult ModelState = ArgumentsValidationResult.Success;

        /// <summary>
        /// Обработчик поступавшей команды
        /// </summary>
        /// <param name="cmdtext">Название команды</param>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="options">Опции обработчика команды</param>
        public abstract Task HandleAsync(string cmdtext, string[] arguments, IHandlerOptions options);


    }
}
