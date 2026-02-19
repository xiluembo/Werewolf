using System.Linq;
using System.Threading.Tasks;
using Shared.Platform;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Werewolf_Node.Platform
{
    internal sealed class TelegramPlatformUpdate : IPlatformUpdate
    {
        public TelegramPlatformUpdate(CallbackQuery query)
        {
            Query = query;
        }

        public CallbackQuery Query { get; }
        public string Payload => Query?.Data;
        public long UserId => Query?.From?.Id ?? 0;
        public long ChatId => Query?.Message?.Chat?.Id ?? 0;
        public int MessageId => Query?.Message?.MessageId ?? 0;
        public object RawUpdate => Query;
    }

    internal static class TelegramMenuMapper
    {
        public static InlineKeyboardMarkup ToInlineKeyboardMarkup(this IPlatformActionMenu menu)
        {
            if (menu?.Rows == null)
            {
                return null;
            }

            var rows = menu.Rows.Select(row => row.Select(action =>
                !string.IsNullOrWhiteSpace(action.Url)
                    ? InlineKeyboardButton.WithUrl(action.Text, action.Url)
                    : InlineKeyboardButton.WithCallbackData(action.Text, action.CallbackData)).ToArray()).ToArray();
            return new InlineKeyboardMarkup(rows);
        }
    }

    internal sealed class TelegramPlatformClient : IPlatformClient
    {
        private readonly ITelegramBotClient _client;

        public TelegramPlatformClient(ITelegramBotClient client)
        {
            _client = client;
        }

        public Task SendMessageAsync(long chatId, string message, IPlatformActionMenu menu = null)
        {
            return _client.SendTextMessageAsync(chatId: chatId, text: message, replyMarkup: menu.ToInlineKeyboardMarkup());
        }

        public Task EditMessageAsync(long chatId, int messageId, string message, IPlatformActionMenu menu = null)
        {
            return _client.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: message, replyMarkup: menu.ToInlineKeyboardMarkup());
        }

        public Task DeleteMessageAsync(long chatId, int messageId)
        {
            return _client.DeleteMessageAsync(chatId: chatId, messageId: messageId);
        }

        public Task AnswerCallbackAsync(string callbackId, string text = null, bool showAlert = false)
        {
            return _client.AnswerCallbackQueryAsync(callbackQueryId: callbackId, text: text, showAlert: showAlert);
        }

        public Task<int> GetChatMemberCountAsync(long chatId)
        {
            return _client.GetChatMemberCountAsync(chatId: chatId);
        }
    }
}
