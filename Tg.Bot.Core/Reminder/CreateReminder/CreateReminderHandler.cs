using MediatR;
using Tg.Bot.Contracts.Reminder;
using ErrorOr;
using Tg.Bot.Domain;

namespace Tg.Bot.Core.Reminder.CreateReminder;


public class CreateReminderHandler(IReminderRepository reminderRepository) : IRequestHandler<CreateReminderCommand, ErrorOr<CreateReminderResponse>>
{
    public async Task<ErrorOr<CreateReminderResponse>> Handle(CreateReminderCommand command, CancellationToken cancellationToken)
    {
        await reminderRepository.SaveReminderAsync(new Domain.Reminder()
        {
            Text = command.Text,
            ChatId = command.ChatId,
            RemindOn = command.RemindOn
        });
        return default;
    }
}