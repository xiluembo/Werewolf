using System.Collections.Generic;
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

    internal sealed class TelegramPlatformAction : IPlatformAction
    {
        public TelegramPlatformAction(string text, string callbackData = null, string url = null)
        {
            Text = text;
            CallbackData = callbackData;
            Url = url;
        }

        public string Text { get; }
        public string CallbackData { get; }
        public string Url { get; }
    }

    internal sealed class TelegramPlatformActionMenu : IPlatformActionMenu
    {
        public TelegramPlatformActionMenu(IReadOnlyCollection<IReadOnlyCollection<IPlatformAction>> rows)
        {
            Rows = rows;
        }

        public IReadOnlyCollection<IReadOnlyCollection<IPlatformAction>> Rows { get; }

        public InlineKeyboardMarkup ToInlineKeyboardMarkup()
        {
            var rows = Rows.Select(row => row.Select(action =>
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
            return _client.SendTextMessageAsync(chatId: chatId, text: message, replyMarkup: (menu as TelegramPlatformActionMenu)?.ToInlineKeyboardMarkup());
        }

        public Task EditMessageAsync(long chatId, int messageId, string message, IPlatformActionMenu menu = null)
        {
            return _client.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: message, replyMarkup: (menu as TelegramPlatformActionMenu)?.ToInlineKeyboardMarkup());
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
