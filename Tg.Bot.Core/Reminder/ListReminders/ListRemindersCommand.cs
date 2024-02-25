using MediatR;
using Tg.Bot.Contracts.Reminder;

namespace Tg.Bot.Core.Reminder.ListReminders;

public class ListRemindersCommand(long chatId) : IRequest<ICollection<ListRemindersResponse>>
{
    public long ChatId { get; set; } = chatId;
}