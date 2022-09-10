using VkBotConstructor.Abstractions.Model;
using VkNet.Model.Attachments;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция валидатора аргументов
    /// </summary>
    public interface IArgumentsValidator
    {
        /// <summary>
        /// Валидация аргументов команды
        /// </summary>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="attachments">Вложения сообщения</param>
        /// <returns>Результат валидации</returns>
        ArgumentsValidationResult Validate(string[] arguments, ICollection<Attachment>? attachments);
    }
}
