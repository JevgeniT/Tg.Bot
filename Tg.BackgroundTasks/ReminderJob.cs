using MediatR;

namespace Tg.BackgroundTasks;

public class ReminderJob(IMediator mediator)
{
    public async Task<string> SendMsg()
    {
        //todo
        await Task.Delay(3000);
        // await mediator.Send(new ReminderEvent()
        // {
        //     ChatId = 4123123,
        //     Text = "hangfire message"
        // });
        return string.Empty;
    }
}