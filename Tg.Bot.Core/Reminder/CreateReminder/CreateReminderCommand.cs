using MediatR;
using ErrorOr;
using Tg.Bot.Contracts.Reminder;
using static Tg.Bot.Core.Extensions.MessageParser;

namespace Tg.Bot.Core.Reminder.CreateReminder;

public class CreateReminderCommand : IRequest<ErrorOr<CreateReminderResponse>>
{
    public CreateReminderCommand(string msg, long chatId)
    {
        Text = ParseMessageBody(msg);
        RemindOn = ParseDate(msg);
        ChatId = chatId;
    }
    
    public Guid Id { get; set; }
        
    public long ChatId { get; set; }

    public string Text { get; set; }

    public DateTime RemindOn { get; set; }
}