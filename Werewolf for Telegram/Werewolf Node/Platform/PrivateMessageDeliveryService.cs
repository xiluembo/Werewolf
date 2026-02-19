using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Werewolf_Node.Platform
{
    internal sealed class PrivateMessagePrompt
    {
        public PrivateMessagePrompt(string instruction, InlineKeyboardMarkup menu = null)
        {
            Instruction = instruction;
            Menu = menu;
        }

        public string Instruction { get; }
        public InlineKeyboardMarkup Menu { get; }
    }

    internal interface IPrivateMessageAdapter
    {
        Task<Telegram.Bot.Types.Message> SendAsync(string message, long externalUserId, InlineKeyboardMarkup menu = null, bool notify = false, bool preview = false);
        PrivateMessagePrompt BuildAuthorizationPrompt();
    }

    internal sealed class TelegramPrivateMessageAdapter : IPrivateMessageAdapter
    {
        private readonly Func<string, long, bool, InlineKeyboardMarkup, bool, bool, Task<Telegram.Bot.Types.Message>> _send;
        private readonly string _botUsername;

        public TelegramPrivateMessageAdapter(Func<string, long, bool, InlineKeyboardMarkup, bool, bool, Task<Telegram.Bot.Types.Message>> send, string botUsername)
        {
            _send = send;
            _botUsername = botUsername;
        }

        public Task<Telegram.Bot.Types.Message> SendAsync(string message, long externalUserId, InlineKeyboardMarkup menu = null, bool notify = false, bool preview = false)
        {
            return _send(message, externalUserId, false, menu, notify, preview);
        }

        public PrivateMessagePrompt BuildAuthorizationPrompt()
        {
            var button = InlineKeyboardButton.WithUrl("Start Me", "http://t.me/" + _botUsername);
            return new PrivateMessagePrompt(null, new InlineKeyboardMarkup(new[] { button }));
        }
    }

    internal sealed class TwitchPrivateMessageAdapter : IPrivateMessageAdapter
    {
        private readonly TwitchPlatformClient _client;

        public TwitchPrivateMessageAdapter(TwitchPlatformClient client)
        {
            _client = client;
        }

        public Task<Telegram.Bot.Types.Message> SendAsync(string message, long externalUserId, InlineKeyboardMarkup menu = null, bool notify = false, bool preview = false)
        {
            _client.QueueDirectMessage(externalUserId, message, ConvertMenu(menu));
            return Task.FromResult<Telegram.Bot.Types.Message>(null);
        }

        public PrivateMessagePrompt BuildAuthorizationPrompt()
        {
            return new PrivateMessagePrompt("To receive private actions, open the Twitch extension panel and authorize it for your account.");
        }

        private static IReadOnlyCollection<IReadOnlyCollection<TwitchPlatformClient.TwitchAction>> ConvertMenu(InlineKeyboardMarkup menu)
        {
            if (menu?.InlineKeyboard == null)
            {
                return null;
            }

            return menu.InlineKeyboard
                .Select(row => (IReadOnlyCollection<TwitchPlatformClient.TwitchAction>)row.Select(button =>
                    new TwitchPlatformClient.TwitchAction(button.Text, button.CallbackData, button.Url)).ToList())
                .ToList();
        }
    }

    internal sealed class PrivateMessageDeliveryService
    {
        private readonly IPrivateMessageAdapter _adapter;

        public PrivateMessageDeliveryService(IPrivateMessageAdapter adapter)
        {
            _adapter = adapter;
        }

        public Task<Telegram.Bot.Types.Message> SendAsync(string message, long externalUserId, InlineKeyboardMarkup menu = null, bool notify = false, bool preview = false)
        {
            return _adapter.SendAsync(message, externalUserId, menu, notify, preview);
        }

        public PrivateMessagePrompt BuildAuthorizationPrompt()
        {
            return _adapter.BuildAuthorizationPrompt();
        }
    }
}
