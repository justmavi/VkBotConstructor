using VkNet.Abstractions;

namespace VkBotConstructor.Internal
{
    internal static class VkHelpers
    {
        public static async Task SendMessaageAsync(IVkApi api, long peerId, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            await api.Messages.SendAsync(new()
            {
                RandomId = new DateTime().Millisecond,
                Message = message,
                PeerId = peerId
            });
        }
    }
}
