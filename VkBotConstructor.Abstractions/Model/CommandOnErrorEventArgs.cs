namespace VkBotConstructor.Abstractions.Model
{
    /// <summary>
    /// Информация о команде, при обработке которого произошла ошибка
    /// </summary>
    public class CommandOnErrorEventArgs
    {
        /// <summary>
        /// Название обработчика
        /// </summary>
        public string HandlerName { get; set; }
        /// <summary>
        /// Аргументы
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Инициализация объекта
        /// </summary>
        /// <param name="commandText">Название команды</param>
        /// <param name="arguments">Аргументы</param>
        public CommandOnErrorEventArgs(string handlerName, string[] arguments)
        {
            HandlerName = handlerName;
            Arguments = arguments;
        }
    }
}
