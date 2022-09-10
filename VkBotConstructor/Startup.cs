using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Attribute;
using VkBotConstructor.Core;
using VkBotConstructor.Core.Middleware;
using VkBotConstructor.Exceptions;
using VkBotConstructor.Handler;
using VkBotConstructor.Internal;
using VkNet;

namespace VkBotConstructor
{
    public static class Startup
    {
        public static IServiceCollection AddVkBot(this IServiceCollection services, Action<IVkBotConstructorOptions> action)
        {
            var options = new VkBotConstructorOptions();
            action.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.GroupAccessToken))
            {
                throw new InvalidAccessTokenException();
            }

            services.AddSingleton((provider) => InitializeManager(options.UserAccessToken, options.GroupAccessToken));
            services.AddSingleton<IVkBotConstructorOptions>(options);
            
            if (options.CommandHandlerOptions is not null)
            {
                var handlerOptions = options.CommandHandlerOptions;
                var types = handlerOptions.Assemblies.SelectMany(x => x.DefinedTypes).Where(x => x.IsAssignableTo(typeof(CommandHandlerBase)));

                if (types.Any()) ScanAndRegisterCommandHandlers(types, services);

                services.AddSingleton(_ => options.CommandHandlerOptions);
            }

            return services;
        }

        public static IApplicationBuilder UseVkBotConstructor(this IApplicationBuilder app, string route)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new InvalidRouteException();
            }

            return app.UseMiddleware<VkBotConstructorMiddleware>(route);
        }

        private static IVkApiManager InitializeManager(string userToken, string groupToken)
        {
            var groupApi = new VkApi();
            groupApi.Authorize(new()
            {
                AccessToken = groupToken
            });

            VkApi userApi = null;

            if (!string.IsNullOrWhiteSpace(userToken))
            {
                userApi = new();
                userApi.Authorize(new()
                {
                    AccessToken = userToken
                });
            }

            return new VkApiManager(userApi, groupApi);
        }

        private static void ScanAndRegisterCommandHandlers(IEnumerable<TypeInfo> handlersAssemblies, IServiceCollection services)
        {
            foreach (var type in handlersAssemblies)
            {
                var commandNames = type.GetCustomAttributes<CommandNameAttribute>().SelectMany(x => x.Names);

                if (commandNames.Any())
                {
                    foreach (var name in commandNames)
                    {
                        CommandHandlersDictionary.Add(name, type);
                    }
                }
                else // Using handler name as command name
                {
                    var name = type.Name.Split("CommandHandler")[0];
                    CommandHandlersDictionary.Add(name, type);
                }

                services.AddTransient(type);
            }
        }

        private static void ScanAndRegisterMessageHandlers(IEnumerable<TypeInfo> handlersAssemblies, IServiceCollection services)
        {
            foreach (var type in handlersAssemblies)
            {
                var messageTexts = type.GetCustomAttributes<MessageTextAttribute>().SelectMany(x => x.Texts);
                var messageTextPatterns = type.GetCustomAttributes<MessageTextPatternAttribute>().SelectMany(x => x.Patterns);
                var messagePayloads = type.GetCustomAttributes<MessagePayloadAttribute>().SelectMany(x => x.Payloads);

                if (messageTexts.Any())
                {
                    foreach (var name in messageTexts)
                    {
                        MessageHandlersDictionary.Add(name, type);
                    }
                }

                if (messageTextPatterns.Any())
                {
                    foreach (var pattern in messageTextPatterns)
                    {
                        MessageHandlersDictionary.Add(pattern, type);
                    }
                }

                if (messagePayloads.Any())
                {
                    foreach (var paylaod in messagePayloads)
                    {
                        MessageHandlersDictionary.Add(paylaod, type);
                    }
                }

                services.AddTransient(type);
            }
        }
    }
}