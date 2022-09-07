using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Exceptions;
using VkNet.Abstractions;

namespace VkBotConstructor.Core
{
    /// <summary>
    /// Менеджер API
    /// </summary>
    public class VkApiManager : IVkApiManager
    {
        private readonly IVkApi _userApi;
        private readonly IVkApi _groupApi;


        /// <inheritdoc />
        public IVkApi UserApi { get => _userApi ?? throw new ManagerIsUninitializedException("user"); }

        /// <inheritdoc />
        public IVkApi GroupApi { get => _groupApi ?? throw new ManagerIsUninitializedException("group"); }

        /// <summary>
        /// Инициализирует менеджер API
        /// </summary>
        /// <param name="userApi">API пользователя</param>
        /// <param name="groupApi">API группы</param>
        public VkApiManager(IVkApi userApi, IVkApi groupApi)
        {
            _userApi = userApi;
            _groupApi = groupApi;
        }
    }
}
