using VkNet.Abstractions;

namespace VkBotConstructor.Abstractions.Core
{
    /// <summary>
    /// Абстракция менеджера API
    /// </summary>
    public interface IVkApiManager
    {
        /// <summary>
        /// Менеджер API пользователя. Инициализируется, если задан токен пользователя при конфигурации
        /// </summary>
        IVkApi UserApi { get; }
        /// <summary>
        /// Менеджер API пользователя. Инициализируется, если задан токен группы при конфигурации
        /// </summary>
        IVkApi GroupApi { get; }
    }
}
