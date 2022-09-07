using VkBotConstructor.Abstractions.Handler;
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
    public abstract class CommandHandlerBase : ICommandHandler
    {
        protected readonly IVkApiManager ApiManager;
        public IMessageInfo MessageInfo { get; set; }

        protected ArgumentsValidationResult ModelState = ArgumentsValidationResult.Success;

        /// <summary>
        /// Инициализация обработчика команд
        /// </summary>
        /// <param name="apiManager">Менеджер API</param>
        public CommandHandlerBase(IVkApiManager apiManager)
        {
            ApiManager = apiManager;
        }

        /// <inheritdoc />
        public async virtual Task HandleAsync(string cmdtext, string[] arguments, ICommandHandlerOptions options)
        {
            var commandHandler = GetType().GetMethods().FirstOrDefault(m =>
            {
                if (m.Name == "Invoke" || m.Name == "InvokeAsync") return true;

                var attribute = m.GetCustomAttribute<CommandNameAttribute>();
                return attribute is null ? m.Name.ToLower() == cmdtext : attribute.Names.Contains(cmdtext);
            });

            if (commandHandler is null && options.ResponseToUnknownCommand)
            {
                await ApiManager.
                    GroupApi.
                    Messages.
                    SendAsync(new()
                    {
                        RandomId = new DateTime().Millisecond,
                        Message = options.UnknownCommandResponseMessage,
                        PeerId = MessageInfo.PeerId,
                    });
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
                    if (arguments[i] is null)
                    {
                        if (Nullable.GetUnderlyingType(parameters[i].ParameterType) is null) return;
                        args[i] = null;
                    }
                    else
                    {
                        args[i] = Convert.ChangeType(arguments[i], parameters[i].ParameterType);
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
                await ApiManager.GroupApi.Messages.SendAsync(new()
                {
                    RandomId = new DateTime().Millisecond,
                    PeerId = MessageInfo.PeerId,
                    Message = message
                });
            }
            else if (result is MessagesSendParams @params)
            {
                await ApiManager.GroupApi.Messages.SendAsync(@params);
            }
        }
    }
}
