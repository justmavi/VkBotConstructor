using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using VkBotConstructor.Abstractions.Core;
using VkBotConstructor.Handler;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace VkBotConstructor.Core.Middleware
{
    internal class VkBotConstructorMiddleware
    {
        private readonly string _route;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        public VkBotConstructorMiddleware(RequestDelegate next, string route, IServiceProvider serviceProvider)
        {
            _route = route;
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, IVkBotConstructorOptions options)
        {
            var httpMethod = context.Request.Method;
            var path = context.Request.Path.Value;

            if (!(httpMethod == "POST" && Regex.IsMatch(path, $"^/?{Regex.Escape(_route)}/?$", RegexOptions.IgnoreCase)))
            {
                await _next(context);
                return;
            }

            var token = await ReadRequestBodyAsync(context.Request);
            var vkResponse = GroupUpdate.FromJson(new VkResponse(token));

#pragma warning disable CS0618 // Type or member is obsolete
            if (vkResponse.Type == GroupUpdateType.Confirmation)
            {
                await context.Response.WriteAsync(options.ConfirmationCode);
                return;
            }
#pragma warning restore CS0618

            await VkEventHandler.HandleAsync(vkResponse.Instance, _serviceProvider);

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("ok");
        }

        private static async Task<JToken> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();

            var bodyAsText = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            return JToken.Parse(bodyAsText);
        }
    }
}
