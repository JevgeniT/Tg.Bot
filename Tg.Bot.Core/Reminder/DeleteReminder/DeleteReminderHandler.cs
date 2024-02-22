using MediatR;
using Tg.Bot.Domain;

namespace Tg.Bot.Core.Reminder.DeleteReminder;

public class DeleteReminderHandler(IReminderRepository reminderRepository) : IRequestHandler<DeleteReminderCommand>
{
    public async Task Handle(DeleteReminderCommand command, CancellationToken cancellationToken)
    {
        await reminderRepository.DeleteReminderAsync(Guid.Empty);//todo 
    }
}