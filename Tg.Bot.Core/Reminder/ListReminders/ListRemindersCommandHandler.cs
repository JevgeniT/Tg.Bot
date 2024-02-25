using MediatR;
using Tg.Bot.Contracts.Reminder;
using Tg.Bot.Domain;

namespace Tg.Bot.Core.Reminder.ListReminders;

public class ListReminderHandler(IReminderRepository repository) : IRequestHandler<ListRemindersCommand, ICollection<ListRemindersResponse>>
{
    public async Task<ICollection<ListRemindersResponse>> Handle(ListRemindersCommand command, CancellationToken cancellationToken)
    {
        return (await repository.GetRemindersByIdAsync(command.ChatId)).Select(x => new ListRemindersResponse()
        {
            RemindOn = x.RemindOn,
            Text = x.Text
        }).ToList();
    }
}