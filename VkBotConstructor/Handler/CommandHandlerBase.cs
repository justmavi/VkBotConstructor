using System.Reflection;
using VkBotConstructor.Attribute;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Event.Message;
using VkBotConstructor.Abstractions.Model;
using VkNet.Model.RequestParams;

namespace VkBotConstructor.Handler
{
    /// <summary>
    /// Базовый обработчик команд
    /// </summary>
    public abstract class CommandHandlerBase
    {
        /// <summary>
        /// Менеджер API
        /// </summary>
        protected readonly IVkApiManager ApiManager;

        /// <summary>
        /// Информация о поступавшей сообщении
        /// </summary>
        public IMessageInfo MessageInfo { get; set; }

        /// <summary>
        /// Ответ обработчика по умолчанию на случай, если аргументы не соответствуют критериям
        /// </summary>
        protected virtual string DefaultAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Результат валидации модели команды
        /// </summary>
        protected ArgumentsValidationResult ModelState = ArgumentsValidationResult.Success;

        /// <summary>
        /// Инициализация обработчика команд
        /// </summary>
        /// <param name="apiManager">Менеджер API</param>
        public CommandHandlerBase(IVkApiManager apiManager)
        {
            ApiManager = apiManager;
        }

        /// <summary>
        /// Обработчик поступавшей команды
        /// </summary>
        /// <param name="cmdtext">Название команды</param>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="options">Опции обработчика команды</param>
        /// <returns></returns>
        public async virtual Task HandleAsync(string cmdtext, string[] arguments, ICommandHandlerOptions options)
        {
            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var commandHandler = methods.FirstOrDefault(m => m.Name == "Invoke" || m.Name == "InvokeAsync");

            if (commandHandler is null)
            {
                // Controller mode

                cmdtext = arguments.FirstOrDefault();
                arguments = arguments.Skip(1).ToArray();

                commandHandler = methods.FirstOrDefault(m =>
                {
                    var attribute = m.GetCustomAttribute<CommandNameAttribute>();
                    return attribute is null ? m.Name.ToLower() == cmdtext : attribute.Names.Contains(cmdtext);
                });
            }

            if (commandHandler is null)
            {
                await OnError(new(cmdtext, arguments));
                return;
            }

            var parameters = commandHandler.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetCustomAttribute<FromArgumentsAttribute>() is not null
                    && parameters[i].ParameterType.IsAssignableTo(typeof(IArgumentsValidator)))
                {
                    var instance = Activator.CreateInstance(parameters[i].ParameterType) as IArgumentsValidator;
                    var validationResult = instance.Validate(arguments);

                    args[i] = validationResult.IsValid ? instance : null;
                    ModelState = validationResult;
                }
                else
                {
                    if (i >= arguments.Length || arguments[i] is null)
                    {
                        if (parameters[i].ParameterType.IsValueType
                            && Nullable.GetUnderlyingType(parameters[i].ParameterType) is null)
                        {
                            await OnError(new(cmdtext, arguments));
                            return;
                        }
                        args[i] = null;
                    }
                    else
                    {
                        args[i] = Convert.ChangeType(arguments[i], 
                            Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType);
                    }
                }
            }

            object result;
            if (commandHandler.ReturnType.IsAssignableTo(typeof(Task)))
            {
                var task = (Task)commandHandler.Invoke(this, args);
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                result = resultProperty.GetValue(task);
            }
            else
            {
                result = commandHandler.Invoke(this, args);
            }

            if (result is string message)
            {
                await SendMessageAsync(MessageInfo.PeerId, message);
            }
            else if (result is MessagesSendParams @params)
            {
                await ApiManager.GroupApi.Messages.SendAsync(@params);
            }
        }

        protected async virtual Task OnError(CommandOnErrorEventArgs eventArgs)
        {
            await SendMessageAsync(MessageInfo.PeerId, DefaultAnswer);
        }

        private async Task SendMessageAsync(long peerId, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            await ApiManager.
                GroupApi.
                Messages.
                SendAsync(new()
                {
                    RandomId = new DateTime().Millisecond,
                    Message = message,
                    PeerId = peerId,
                });
        }
    }
}
