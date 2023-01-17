using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Enums;
using VkBotConstructor.Abstractions.Event.Message;
using VkBotConstructor.Abstractions.Model;
using VkBotConstructor.Attribute;
using VkBotConstructor.Exceptions;
using VkBotConstructor.Internal;
using VkNet.Model.RequestParams;

namespace VkBotConstructor.Handler
{
    /// <summary>
    /// Абстракция обработчиков
    /// </summary>
    public abstract class BaseHandler
    {
        /// <summary>
        /// Инициализация абстракции обработчиков
        /// </summary>
        /// <param name="serviceProvider">Зависимотси запроса</param>
        public BaseHandler(IVkApiManager manager, IServiceProvider serviceProvider, HandlerType handlerType)
        {
            RequestServices = serviceProvider;
            HandlerType = handlerType;
            ApiManager = manager;

        }

        /// <summary>
        /// Информация о поступавшей сообщении
        /// </summary>
        public IMessageInfo MessageInfo { get; internal set; }

        /// <summary>
        /// Ответ обработчика по умолчанию на случай, если аргументы не соответствуют критериям
        /// </summary>
        protected virtual string DefaultAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Зависимости запроса
        /// </summary>
        protected IServiceProvider RequestServices { get; }

        /// <summary>
        /// Тип обработчика
        /// </summary>
        protected HandlerType HandlerType { get; }

        /// <summary>
        /// Менеджер API
        /// </summary>
        protected IVkApiManager ApiManager { get; }

        /// <summary>
        /// Результат валидации модели команды
        /// </summary>
        protected ArgumentsValidationResult ModelState = ArgumentsValidationResult.Success;

        /// <summary>
        /// Обработчик поступавшей команды
        /// </summary>
        /// <param name="cmdtext">Название команды</param>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="payload">Полезная нагрузка</param>
        /// <param name="options">Опции обработчика команды</param>
        public virtual async Task HandleAsync(string[] arguments, string payload, IHandlerOptions options)
        {
            var handler = GetHandler(ref arguments, payload, options);

            if (handler is null)
            {
                OnError(new(null, arguments));
                return;
            }

            var parameters = handler.GetParameters();
            var validatedArgs = ValidateArguments(parameters, arguments, options);

            if (validatedArgs is null)
            {
                OnError(new(handler.Name, arguments));
                return;
            }

            object result;
            if (handler.ReturnType.IsAssignableTo(typeof(Task)))
            {
                var task = (Task)handler.Invoke(this, validatedArgs);
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                result = resultProperty.GetValue(task);
            }
            else
            {
                result = handler.Invoke(this, validatedArgs);
            }

            await SendResponseToClientAsync(result, options);
        }

        /// <summary>
        /// Определяет логику поиска обработчика
        /// </summary>
        /// <param name="arguments">Аргументы команды</param>
        /// <param name="paylaod">Полезная нагрузка</param>
        /// <param name="options">Параметры обработчика</param>
        /// <returns>В случае наличия нужного обработчика возвращает ссылку на него, иначе NULL</returns>
        protected virtual MethodInfo? GetHandler(ref string[] arguments, string paylaod, IHandlerOptions options)
        {
            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var commandHandlers = methods.Where(m => m.Name == "Invoke" || m.Name == "InvokeAsync").ToList();

            MethodInfo commandHandler;
            if (commandHandlers.Count > 1) // Overloading by parameters count
            {
                var argumentsCount = arguments.Length;
                commandHandler = commandHandlers.OrderBy(m => m.GetParameters().Length).FirstOrDefault(m => m.GetParameters().Length >= argumentsCount);
            }
            else commandHandler = commandHandlers.FirstOrDefault();

            if (commandHandler is null)
            {
                // Controller-type handlers

                var localCommandText = arguments.FirstOrDefault();
                arguments = arguments.Skip(1).ToArray();

                commandHandlers = methods.Where(m =>
                {
                    var attribute = m.GetCustomAttribute<CommandNameAttribute>();
                    var comparer = options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : default;

                    return attribute is null ? m.Name.Equals(localCommandText, comparer)
                    : attribute.Names.Any(x => x.Equals(localCommandText, comparer));
                }).ToList();

                if (commandHandlers.Count > 1) // Overloading by parameters count
                {
                    var argumentsCount = arguments.Length;
                    commandHandler = commandHandlers.OrderBy(m => m.GetParameters().Length).FirstOrDefault(m => m.GetParameters().Length >= argumentsCount);
                }
                else commandHandler = commandHandlers.FirstOrDefault();
            }

            return commandHandler;
        }

        /// <summary>
        /// Определяет логику валидации аргументов и их присваивание к параметрам обработчика
        /// </summary>
        /// <param name="parameters">Параметры обработчика</param>
        /// <param name="arguments">Аргументы</param>
        /// <param name="options">Параметры обработчика</param>
        /// <returns>В случае успешной валидации возвращает массив аргументов, которые можно передать обработчику, иначе NULL</returns>
        protected virtual object[] ValidateArguments(ParameterInfo[] parameters, string[] arguments, IHandlerOptions options)
        {
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsAssignableTo(typeof(IArgumentsValidator)))
                {
                    var instance = Activator.CreateInstance(parameters[i].ParameterType) as IArgumentsValidator;
                    var validationResult = instance.Validate(arguments, MessageInfo.Instance.Attachments);

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
                            return null;
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

            return args;
        }

        /// <summary>
        /// Определяет логику обработки результата выполнения обработчика
        /// </summary>
        /// <param name="handlerResult">Результат выполнения обработчика</param>
        /// <param name="options">Параметры обработчика</param>
        /// <returns>Выполненную задачу</returns>
        protected virtual Task SendResponseToClientAsync(object handlerResult, IHandlerOptions options)
        {
            if (handlerResult is string message)
            {
                return VkHelpers.SendMessaageAsync(ApiManager.GroupApi, MessageInfo.PeerId, message);
            }
            else if (handlerResult is MessagesSendParams @params)
            {
                return ApiManager.GroupApi.Messages.SendAsync(@params);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Делегирует вызов в другие обработчики
        /// </summary>
        /// <typeparam name="TAssembly">Обработчик</typeparam>
        /// <param name="handlerName">Название вложенного обработчика (для controller-type обработчиков)</param>
        /// <param name="args">Аргументы</param>
        /// <param name="payload">Полезная нагрузка</param>
        /// <returns></returns>
        /// <exception cref="HandlerNotFoundException">Обработчик не найден</exception>
        /// <exception cref="InvalidOperationException">Неизвестный тип обработчика</exception>
        protected Task DelegateTo<TAssembly>(string handlerName = null, string[] args = null, string payload = null) where TAssembly : BaseHandler
        {
            var handlerService = RequestServices.GetService<TAssembly>() 
                ?? throw new HandlerNotFoundException(typeof(TAssembly).Name);
            handlerName ??= GetHandlerDefaultCommandText<TAssembly>(handlerService.HandlerType);

            IHandlerOptions options = HandlerType switch
            {
                HandlerType.Command => RequestServices.GetService<ICommandHandlerOptions>(),
                HandlerType.Message => RequestServices.GetService<IMessageHandlerOptions>(),
                _ => throw new InvalidOperationException("Invalid handler type"),
            };

            var handlerArgs = new string[args.Length + 1];
            handlerArgs[0] = handlerName;
            Array.Copy(args, 0, handlerArgs, 1, args.Length);

            handlerService.MessageInfo = MessageInfo;
            return handlerService.HandleAsync(handlerArgs, payload, options);
        }

        /// <summary>
        /// Возникает при сбое вызова обработчика
        /// </summary>
        /// <param name="eventArgs">Аргументы события</param>
        protected async virtual void OnError(CommandOnErrorEventArgs eventArgs)
        {
            await VkHelpers.SendMessaageAsync(ApiManager.GroupApi, MessageInfo.PeerId, DefaultAnswer);
        }

        private static string GetHandlerDefaultCommandText<T>(HandlerType handlerType) where T : BaseHandler
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<CommandNameAttribute>();

            if (attribute is null) return type.Name.Split(handlerType + "Handler")[0];

            return attribute.Names[0];
        }
    }
}
