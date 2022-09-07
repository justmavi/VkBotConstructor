using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Handler;
using VkBotConstructor.Attribute;
using VkBotConstructor.Core;
using VkBotConstructor.Core.Middleware;
using VkBotConstructor.Exceptions;
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
                var types = handlerOptions.Assemblies.SelectMany(x => x.DefinedTypes).Where(x => x.IsAssignableTo(typeof(ICommandHandler)));
               
                if (types.Any())
                {
                    foreach (var type in types)
                    {
                        var cmdNameAttribute = type.GetCustomAttribute<CommandNameAttribute>();
                        
                        if (cmdNameAttribute is not null)
                        {
                            foreach (var name in cmdNameAttribute.Names)
                            {
                                HandlersDictionary.Add(name, type);
                            }
                        }
                        else
                        {
                            var name = type.Name.Split("CommandHandler")[0];
                            HandlersDictionary.Add(name, type);
                        }

                        services.AddTransient(type);
                    }
                }

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
    }
}