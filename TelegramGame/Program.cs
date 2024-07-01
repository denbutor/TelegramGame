using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramGame
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("6983412958:AAHtvvUg6fXPDnRw1RxOd0EUqcPqqk2UzJM");
        private static int secretNumber = new Random().Next(1, 101);
        private static bool gameInProgress = false;

        static async Task Main(string[] args)
        {
            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Hello! My name is {me.Username}");

            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
            {
                var message = update.Message;

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привіт! Давай пограємо в гру 'Вгадай число'. Я загадав число від 1 до 100. Напиши своє припущення.");
                    gameInProgress = true;
                    secretNumber = new Random().Next(1, 101);
                }
                else if (gameInProgress)
                {
                    if (int.TryParse(message.Text, out int guessedNumber))
                    {
                        if (guessedNumber < secretNumber)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Загадане число більше.");
                        }
                        else if (guessedNumber > secretNumber)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Загадане число менше.");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Вітаю! Ви вгадали число {secretNumber}. Щоб зіграти ще раз, напишіть /start.");
                            gameInProgress = false;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Будь ласка, введіть дійсне число.");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Напишіть /start, щоб почати нову гру.");
                }
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}
