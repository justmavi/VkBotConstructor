using VkBotConstructor.Abstractions.Core;

namespace VkBotConstructor.Core
{
    /// <summary>
    /// Параметры установки бота
    /// </summary>
    public class VkBotConstructorOptions : IVkBotConstructorOptions
    {
        /// <inheritdoc />
        public string ConfirmationCode { get; set; }

        /// <inheritdoc />
        public string GroupAccessToken { get; set; }

        /// <inheritdoc />
        public string UserAccessToken { get; set; }

        internal ICommandHandlerOptions CommandHandlerOptions { get; set; }
        internal IMessageHandlerOptions MessageHandlerOptions { get; set; }

        /// <inheritdoc />
        public void UseCommandHandler(Action<ICommandHandlerOptions> action)
        {
            var options = new CommandHandlerOptions();
            action.Invoke(options);

            CommandHandlerOptions = options;
        }

        public void UseMessageHandler(Action<IMessageHandlerOptions> action)
        {
            var options = new MessageHandlerOptions();
            action.Invoke(options);

            MessageHandlerOptions = options;
        }
    }
}
