using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Platform
{
    public sealed class PlatformAction : IPlatformAction
    {
        public PlatformAction(string text, string actionCode = null, string payload = null, string url = null)
        {
            Text = text;
            ActionCode = actionCode;
            Payload = payload;
            Url = url;
        }

        public string Text { get; }
        public string ActionCode { get; }
        public string Payload { get; }
        public string Url { get; }
        public string CallbackData => string.IsNullOrWhiteSpace(ActionCode)
            ? Payload
            : string.IsNullOrWhiteSpace(Payload) ? ActionCode : $"{ActionCode}|{Payload}";
    }

    public sealed class PlatformActionMenu : IPlatformActionMenu
    {
        public PlatformActionMenu(IReadOnlyCollection<IReadOnlyCollection<IPlatformAction>> rows)
        {
            Rows = rows;
        }

        public IReadOnlyCollection<IReadOnlyCollection<IPlatformAction>> Rows { get; }
    }

    public interface IPlatformUpdate
    {
        string Payload { get; }
        long UserId { get; }
        long ChatId { get; }
        int MessageId { get; }
        object RawUpdate { get; }
    }

    public interface IPlatformAction
    {
        string Text { get; }
        string ActionCode { get; }
        string Payload { get; }
        string CallbackData { get; }
        string Url { get; }
    }

    public interface IPlatformActionMenu
    {
        IReadOnlyCollection<IReadOnlyCollection<IPlatformAction>> Rows { get; }
    }

    public interface IPlatformClient
    {
        Task SendMessageAsync(long chatId, string message, IPlatformActionMenu menu = null);
        Task EditMessageAsync(long chatId, int messageId, string message, IPlatformActionMenu menu = null);
        Task DeleteMessageAsync(long chatId, int messageId);
        Task AnswerCallbackAsync(string callbackId, string text = null, bool showAlert = false);
        Task<int> GetChatMemberCountAsync(long chatId);
    }
}
