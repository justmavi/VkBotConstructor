using VkBotConstructor.Abstractions.Event.Message;

namespace VkBotConstructor.Event.Message
{
    /// <summary>
    /// Информация о сообщении
    /// </summary>
    public class MessageInfo : IMessageInfo
    {
        /// <inheritdoc/>
        public long PeerId { get; set; }

        /// <inheritdoc/>
        public long FromId { get; set; }

        /// <inheritdoc/>
        public string Payload { get; set; }

        /// <inheritdoc/>
        public VkNet.Model.Message Instance { get; set; }

        /// <summary>
        /// Инициализация объекта информации о сообщения
        /// </summary>
        /// <param name="peerId">Идентификатор чата</param>
        /// <param name="fromId">Идентификатор отправителя</param>
        /// <param name="payload">Полезная нагрузка</param>
        /// <param name="instance">Объект самого сообщения</param>
        public MessageInfo(long peerId, long fromId, string payload, VkNet.Model.Message instance)
        {
            PeerId = peerId;
            FromId = fromId;
            Payload = payload;
            Instance = instance;
        }

        /// <summary>
        /// Инициализация объекта информации о сообщения
        /// </summary>
        /// <param name="message">Объект сообщения</param>
        public MessageInfo(VkNet.Model.Message message): this(message.PeerId.Value, message.FromId.Value, message.Payload, message) { }
    }
}
