using System.Threading.Channels;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tg.Bot.Contracts.Reminder;
using Tg.Bot.Core.Reminder.CreateReminder;
using Tg.Bot.Core.Reminder.DeleteReminder;
using Tg.Bot.Core.Reminder.ListReminders;
using Tg.Bot.Domain;
using Tg.Bot.Domain.Services;

namespace Tg.Bot;


public class BotController(ILogger<BotController> logger, ITelegramBotClient bot, IMediator mediator, Channel<Reminder> channel) : IBotController
{
    public void StartReceiving(CancellationToken cts)
    {
        bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions : new ()
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            },
            cancellationToken: cts
        );
        Task.Run(() => ReadRemindersChannel(cts), cts);
    }

    private async Task ReadRemindersChannel(CancellationToken cts)
    {
        while (!cts.IsCancellationRequested)
        {
            if (channel.Reader.TryRead(out var reminder) && reminder is not null)
            {
                await RespondAsync(reminder.ChatId, reminder.Text, cts);
                logger.LogInformation($"Sending {reminder.Text} to {reminder.ChatId }");
            }
            await Task.Delay(5000, cts);
        }
    }
    
    public async Task RespondAsync(long chatId, string msg, CancellationToken ct)
    {
        // todo map to response obj
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
                InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
            },
          
        });
        await bot.SendTextMessageAsync(
            chatId: chatId,
            text: msg,
            // replyMarkup: inlineKeyboard,
            cancellationToken: ct);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message )
            return;
        if (message.Text is not { } messageText)
            return;
        
        var chatId = message.Chat.Id;

        logger.LogInformation($"Received a '{messageText}' message in chat {chatId}.");
        
        object command = message.Text.Split(" ")[0] switch
        {
            "new" => new CreateReminderCommand(message.Text, chatId),
            "list" => new ListRemindersCommand(chatId),
            "rm" => new DeleteReminderCommand(Guid.Empty), //todo
            _ => null!
        };

        if (command is null)
        {
            await RespondAsync(chatId: chatId, msg: "Invalid command", cancellationToken);
            return;
        }
        
        var response = await mediator.Send(command, cancellationToken);
        
        var responseBody = response switch
        {
            ICollection<ListRemindersResponse> list when list.Count != 0 => string.Join("\n", list.Select(x => $"{x.Text} {x.RemindOn}")),
            ErrorOr<CreateReminderResponse> { IsError: true } r => string.Join("\n", r.Errors.Select(x => $"{x.Description}")),
            ErrorOr<CreateReminderResponse> { IsError: false } => "saved",
            _ => "ok"
        };

        await RespondAsync(chatId: chatId, msg: responseBody, cancellationToken);
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var error = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogError(error);

        return Task.CompletedTask;
    }
}