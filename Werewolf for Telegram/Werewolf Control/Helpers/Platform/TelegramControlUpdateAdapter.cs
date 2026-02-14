using Shared.Platform;
using Telegram.Bot.Types;

namespace Werewolf_Control.Helpers.Platform
{
    internal sealed class TelegramControlUpdateAdapter : IPlatformUpdate
    {
        public TelegramControlUpdateAdapter(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
        public string Payload => Update?.CallbackQuery?.Data;
        public long UserId => Update?.CallbackQuery?.From?.Id ?? Update?.Message?.From?.Id ?? 0;
        public long ChatId => Update?.CallbackQuery?.Message?.Chat?.Id ?? Update?.Message?.Chat?.Id ?? 0;
        public int MessageId => Update?.CallbackQuery?.Message?.MessageId ?? Update?.Message?.MessageId ?? 0;
        public object RawUpdate => Update;
    }
}
