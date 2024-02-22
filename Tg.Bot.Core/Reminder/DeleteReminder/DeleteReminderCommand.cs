using MediatR;

namespace Tg.Bot.Core.Reminder.DeleteReminder;

public class DeleteReminderCommand(Guid id) : IRequest
{
    public Guid Id { get; set; } = id;
}