namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция параметров установки бота
    /// </summary>
    public interface IVkBotConstructorOptions
    {
        /// <summary>
        /// Код подтверждения
        /// </summary>
        string ConfirmationCode { get; set; }

        /// <summary>
        /// Токен доступа группы
        /// </summary>
        string GroupAccessToken { get; set; }

        /// <summary>
        /// Токен доступа пользователя
        /// </summary>
        string UserAccessToken { get; set; }

        /// <summary>
        /// Использовать обработчик команд
        /// </summary>
        /// <param name="action">Функция, задающая нужные конфигурации</param>
        void UseCommandHandler(Action<ICommandHandlerOptions> action);
    }
}