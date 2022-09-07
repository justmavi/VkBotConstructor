using VkBotConstructor.Abstractions.Model;

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
        /// <returns>Результат валидации</returns>
        ArgumentsValidationResult Validate(string[] arguments);
    }
}
