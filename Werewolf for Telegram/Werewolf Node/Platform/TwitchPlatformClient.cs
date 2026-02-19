using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Platform;

namespace Werewolf_Node.Platform
{
    internal sealed class TwitchPlatformClient : IPlatformClient
    {
        internal sealed class TwitchAction
        {
            public TwitchAction(string text, string callbackData, string url)
            {
                Text = text;
                CallbackData = callbackData;
                Url = url;
            }

            public string Text { get; }
            public string CallbackData { get; }
            public string Url { get; }
        }

        internal sealed class TwitchDirectMessage
        {
            public TwitchDirectMessage(long userId, string message, IReadOnlyCollection<IReadOnlyCollection<TwitchAction>> actions)
            {
                UserId = userId;
                Message = message;
                Actions = actions;
            }

            public long UserId { get; }
            public string Message { get; }
            public IReadOnlyCollection<IReadOnlyCollection<TwitchAction>> Actions { get; }
        }

        public event Action<IPlatformUpdate> ExtensionEventReceived;
        public event Action<TwitchDirectMessage> DirectMessageQueued;

        private readonly ConcurrentDictionary<long, ConcurrentQueue<TwitchDirectMessage>> _directMessagesByUser =
            new ConcurrentDictionary<long, ConcurrentQueue<TwitchDirectMessage>>();

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

        public void QueueDirectMessage(long userId, string message, IReadOnlyCollection<IReadOnlyCollection<TwitchAction>> actions = null)
        {
            var directMessage = new TwitchDirectMessage(userId, message, actions);
            var queue = _directMessagesByUser.GetOrAdd(userId, _ => new ConcurrentQueue<TwitchDirectMessage>());
            queue.Enqueue(directMessage);
            DirectMessageQueued?.Invoke(directMessage);
        }

        public IReadOnlyCollection<TwitchDirectMessage> GetQueuedDirectMessages(long userId)
        {
            if (!_directMessagesByUser.TryGetValue(userId, out var queue))
            {
                return Array.Empty<TwitchDirectMessage>();
            }

            return queue.ToList();
        }

        public IReadOnlyCollection<TwitchDirectMessage> GetQueuedDirectMessages(long userId, IReadOnlyCollection<string> validPayloads)
        {
            var messages = GetQueuedDirectMessages(userId);
            if (validPayloads == null || validPayloads.Count == 0)
            {
                return messages;
            }

            var whitelist = new HashSet<string>(validPayloads);
            return messages
                .Select(message => new TwitchDirectMessage(
                    message.UserId,
                    message.Message,
                    message.Actions?
                        .Select(row => (IReadOnlyCollection<TwitchAction>)row.Where(action => string.IsNullOrWhiteSpace(action.CallbackData) || whitelist.Contains(action.CallbackData)).ToList())
                        .Where(row => row.Count > 0)
                        .ToList()))
                .Where(message => message.Actions == null || message.Actions.Count > 0)
                .ToList();
        }
    }
}
