namespace VkBotConstructor.Abstractions.Event.Message
{
    /// <summary>
    /// Абстрация информации о сообщения
    /// </summary>
    public interface IMessageInfo
    {
        /// <summary>
        /// Идентификатор назначения (чат, от которого поступило сообщение)
        /// </summary>
        public long PeerId { get; set; }

        /// <summary>
        /// Идентификатор отправителя (тот, кто написал сообщение)
        /// </summary>
        public long FromId { get; set; }

        /// <summary>
        /// Полезная нагрузка сообщения
        /// </summary>
        public string? Payload { get; set; }

        /// <summary>
        /// Объект самого сообщения. Содержит всю остальную информацию
        /// </summary>
        public VkNet.Model.Message Instance { get; set; }
    }
}
