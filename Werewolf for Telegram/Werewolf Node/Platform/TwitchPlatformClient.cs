using System;
using System.Threading.Tasks;
using Shared.Platform;

namespace Werewolf_Node.Platform
{
    internal sealed class TwitchPlatformClient : IPlatformClient
    {
        public event Action<IPlatformUpdate> ExtensionEventReceived;

        public Task SendMessageAsync(long chatId, string message, IPlatformActionMenu menu = null)
        {
            return Task.CompletedTask;
        }

        public Task EditMessageAsync(long chatId, int messageId, string message, IPlatformActionMenu menu = null)
        {
            return Task.CompletedTask;
        }

        public Task DeleteMessageAsync(long chatId, int messageId)
        {
            return Task.CompletedTask;
        }

        public Task AnswerCallbackAsync(string callbackId, string text = null, bool showAlert = false)
        {
            return Task.CompletedTask;
        }

        public Task<int> GetChatMemberCountAsync(long chatId)
        {
            return Task.FromResult(0);
        }

        public void OnExtensionEvent(IPlatformUpdate update)
        {
            ExtensionEventReceived?.Invoke(update);
        }
    }
}
