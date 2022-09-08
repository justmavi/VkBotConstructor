using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Abstractions.Model;
using VkBotConstructor.Event.Message;
using VkBotConstructor.Internal;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace VkBotConstructor.Handler
{
    static internal class VkEventHandler
    {
        public static async Task HandleAsync(IGroupUpdate @event, IServiceProvider serviceProvider)
        {
            if (@event is MessageNew obj) await HandleNewMessageEventAsync(obj.Message, serviceProvider.GetRequiredService<ICommandHandlerOptions>(), serviceProvider);
        }

        public static async Task HandleNewMessageEventAsync(Message message, ICommandHandlerOptions commandHandlerOptions, IServiceProvider serviceProvider)
        {
            if (Regex.IsMatch(message.Text, $"^{commandHandlerOptions.CommandToken}", commandHandlerOptions.IgnoreCase ? RegexOptions.IgnoreCase : default))
            {
                var cmdInfo = ParseMessageTextToCommand(message.Text[commandHandlerOptions.CommandToken.Length..], commandHandlerOptions.ArgumentsSeparator);
                if (cmdInfo is null) return;

                var handlerAssembly = HandlersDictionary.Get(cmdInfo.CommandText, commandHandlerOptions.IgnoreCase);

                if (handlerAssembly is not null && serviceProvider.GetService(handlerAssembly) is CommandHandlerBase handler)
                {
                    handler.MessageInfo = new MessageInfo(message);

                    await handler.HandleAsync(cmdInfo.CommandText, cmdInfo.Arguments, commandHandlerOptions);
                }
                else
                {
                    if (commandHandlerOptions.ResponseToUnknownCommand)
                    {
                        var apiManager = serviceProvider.GetRequiredService<IVkApiManager>();

                        await apiManager.
                            GroupApi.
                            Messages.
                            SendAsync(new()
                            {
                                RandomId = new DateTime().Millisecond,
                                Message = commandHandlerOptions.UnknownCommandResponseMessage,
                                PeerId = message.PeerId,
                            });
                    }
                }
            }
        }

        private static CommandInfo ParseMessageTextToCommand(string message, string separator)
        {
            var parts = message.Split(separator);

            return parts.Length switch
            {
                0 => null,
                1 => new(parts[0], Array.Empty<string>()),
                _ => new(parts[0], parts[1..])
            };
        }
    }
}
