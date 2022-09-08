namespace VkBotConstructor.Abstractions.Model
{
    /// <summary>
    /// Результат валидации аргументов команды
    /// </summary>
    public class ArgumentsValidationResult
    {
        /// <summary>
        /// Возвращает успешный результат
        /// </summary>
        public static ArgumentsValidationResult Success => new() { IsValid = true };
        /// <summary>
        /// Возвращает неуспешный результат
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Неуспешный результат</returns>
        public static ArgumentsValidationResult Failure(string? errorMessage = null) => new() { IsValid = false, ErrorMessage = errorMessage };

        /// <summary>
        /// Статус валидности модели
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если IsValid == false
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
