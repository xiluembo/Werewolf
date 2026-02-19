using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Platform
{
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
